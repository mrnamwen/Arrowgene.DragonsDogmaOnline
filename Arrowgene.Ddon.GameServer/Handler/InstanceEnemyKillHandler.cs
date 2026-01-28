using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.GameServer.GatheringItems.Generators;
using Arrowgene.Ddon.GameServer.Party;
using Arrowgene.Ddon.GameServer.Quests;
using Arrowgene.Ddon.GameServer.Scripting.Interfaces;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Server.Network;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class InstanceEnemyKillHandler : GameRequestPacketHandler<C2SInstanceEnemyKillReq, S2CInstanceEnemyKillRes>
    {

        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(InstanceEnemyKillHandler));

        private readonly DdonGameServer _gameServer;

        private readonly HashSet<uint> _ignoreKillsInStageIds = new HashSet<uint>()
        {
            Stage.TrainingRoom.StageId,
        };

        public InstanceEnemyKillHandler(DdonGameServer server) : base(server)
        {
            _gameServer = server;
        }

        public override S2CInstanceEnemyKillRes Handle(GameClient client, C2SInstanceEnemyKillReq packet)
        {
            CDataStageLayoutId layoutId = packet.LayoutId;
            StageLayoutId stageId = layoutId.AsStageLayoutId();

            PacketQueue queuedPackets = new();

            // The training room uses special handling to produce enemies that don't exist in the QuestState or InstanceEnemyManager.
            // Return an empty response here to not break the rest of the handling.
            if (_ignoreKillsInStageIds.Contains(stageId.Id))
            {
                return new();
            }

            InstancedEnemy enemyKilled = client.Party.InstanceEnemyManager.GetInstanceEnemy(stageId, (byte)packet.SetId);
            if (enemyKilled is null)
            {
                Logger.Error(client, $"Enemy killed data missing; {layoutId}.{packet.SetId}");
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_INSTANCE_AREA_ENEMY_UNIT_DATA_NONE);
            }

            Quest quest = null;
            bool isQuestControlled = false;
            if (enemyKilled.QuestScheduleId != 0)
            {
                quest = QuestManager.GetQuestByScheduleId(enemyKilled.QuestScheduleId);
                isQuestControlled = (quest != null);
            }

            if (enemyKilled.RepopCount > 0 && enemyKilled.RepopNum < enemyKilled.RepopCount)
            {
                enemyKilled.RepopNum += 1;

                S2CInstanceEnemyRepopNtc repopNtc = new S2CInstanceEnemyRepopNtc()
                {
                    LayoutId = layoutId,
                    WaitSecond = enemyKilled.RepopWaitSecond,
                    EnemyData = new CDataLayoutEnemyData()
                    {
                        PositionIndex = (byte)packet.SetId,
                        EnemyInfo = enemyKilled.AsCDataStageLayoutEnemyPresetEnemyInfoClient()
                    }
                };
                client.Party.EnqueueToAll(repopNtc, queuedPackets);
            }
            else
            {
                enemyKilled.IsKilled = true;
            }

            bool isEpitaphEnemy = false;
            if (_gameServer.EpitaphRoadManager.TrialInProgress(client.Party))
            {
                isEpitaphEnemy = _gameServer.EpitaphRoadManager.TrialHasEnemies(client.Party, stageId, 0);
                _gameServer.EpitaphRoadManager.EvaluateEnemyKilled(client.Party, stageId, packet.SetId, enemyKilled);
            }

            Server.Database.ExecuteInTransaction(connectionIn =>
            {
                List<InstancedEnemy> group = client.Party.InstanceEnemyManager.GetInstancedEnemies(stageId);

                bool groupDestroyed;
                if (isQuestControlled)
                {
                    groupDestroyed = group.Where(x => x.IsRequired && x.QuestScheduleId == quest.QuestScheduleId).All(x => x.IsKilled);
                }
                else
                {
                    groupDestroyed = group.Where(x => x.IsRequired).All(x => x.IsKilled);
                }

                if (groupDestroyed)
                {
                    bool isAreaBoss = group.Any(x => x.IsAreaBoss);
                    bool isDungeon = StageManager.IsDungeon(stageId);
                    bool isCautionSpot = Server.ScriptManager.MonsterCautionSpotModule.IsEnabledCautionSpotGroup(Server, client.Party, stageId, client.Character.AreaId);

                    if (isQuestControlled)
                    {
                        var ntcs = QuestManager.GetQuestStateManager(client, quest).HandleDestroyGroupWorkNotice(client.Party, quest, stageId, enemyKilled, connectionIn);
                        queuedPackets.AddRange(ntcs);
                    }

                    // This is used for quests and things like key door monsters
                    S2CInstanceEnemyGroupDestroyNtc groupDestroyedNtc = new S2CInstanceEnemyGroupDestroyNtc()
                    {
                        LayoutId = packet.LayoutId,
                        IsAreaBoss = isCautionSpot && (client.GameMode == GameMode.Normal)
                    };
                    client.Party.EnqueueToAll(groupDestroyedNtc, queuedPackets);

                    if (isAreaBoss && client.GameMode == GameMode.BitterblackMaze)
                    {
                        foreach (var memberClient in client.Party.Clients)
                        {
                            var ntcs = BitterblackMazeManager.HandleTierClear(_gameServer, memberClient, memberClient.Character, stageId, connectionIn);
                            queuedPackets.AddRange(ntcs);
                        }
                    }
                    else if (isAreaBoss && isDungeon && client.GameMode == GameMode.Normal)
                    {
                        var boss = group.Where(x => x.IsAreaBoss).First();
                        client.Party.EnqueueToAll(new S2CInstanceEnemyStageBossAnnihilateNtc()
                        {
                            LayoutId = boss.StageLayoutId.ToCDataStageLayoutId(),
                        }, queuedPackets);

                        // Update reward mission progress for dungeon clear
                        foreach (var memberClient in client.Party.Clients)
                        {
                            queuedPackets.AddRange(Server.RewardMissionManager.UpdateMissionProgress(memberClient, DailyMissionType.ClearDungeon, 1, connectionIn));
                        }
                    }

                    if (isAreaBoss && client.GameMode == GameMode.Normal)
                    {
                        foreach (var memberClient in client.Party.Clients)
                        {
                            queuedPackets.AddRange(Server.AreaRankManager.AddAreaPoint(memberClient, client.Character.AreaId, (Server.GameSettings.GameServerSettings.AreaBossApReward, 0), connectionIn));
                        }
                    }
                }

                if (!packet.IsNoBattleReward && !client.QuestState.IsQuestActive(QuestId.ResolutionsAndOmens))
                {
                    bool autolootEnabled = Server.GameSettings.GameServerSettings.EnableAutoloot;
                    bool materialsToStorage = Server.GameSettings.GameServerSettings.AutolootMaterialsToStorage;

                    foreach (var partyMemberClient in client.Party.Clients)
                    {
                        var instancedGatheringItems = partyMemberClient.InstanceDropItemManager.Generate(enemyKilled);
                        var allDropItems = instancedGatheringItems.Values.SelectMany(x => x).ToList();

                        // Handle autoloot for consumables and materials
                        List<InstancedGatheringItem> remainingDropItems = new List<InstancedGatheringItem>();
                        List<(string Name, uint Count)> storageItems = new List<(string, uint)>();

                        if (autolootEnabled && allDropItems.Any())
                        {
                            S2CItemUpdateCharacterItemNtc autolootNtc = new S2CItemUpdateCharacterItemNtc()
                            {
                                UpdateType = ItemNoticeType.Drop
                            };

                            foreach (var dropItem in allDropItems)
                            {
                                var clientItemInfo = Server.AssetRepository.ClientItemInfos.ContainsKey(dropItem.ItemId)
                                    ? Server.AssetRepository.ClientItemInfos[dropItem.ItemId]
                                    : null;

                                // Category 1 = consumable, Category 2 = material
                                bool isAutolootable = clientItemInfo != null && (clientItemInfo.Category == 1 || clientItemInfo.Category == 2);

                                if (isAutolootable)
                                {
                                    // Determine destination: materials go to storage if setting enabled, otherwise item bag
                                    bool toItemBag = !(materialsToStorage && clientItemInfo.Category == 2);
                                    var destinationStorageType = toItemBag ? clientItemInfo.StorageType : StorageType.StorageBoxNormal;

                                    if (Server.ItemManager.CanAddItem(partyMemberClient.Character, destinationStorageType, (uint)dropItem.ItemId, dropItem.ItemNum))
                                    {
                                        // Capture item count before GatherItem modifies it
                                        uint itemCount = dropItem.ItemNum;

                                        // Auto-loot the item to inventory or storage
                                        queuedPackets.AddRange(Server.ItemManager.GatherItem(partyMemberClient, autolootNtc, dropItem, itemCount, toItemBag, connectionIn));

                                        // Track items sent to storage for notification
                                        if (!toItemBag)
                                        {
                                            storageItems.Add((clientItemInfo.Name, itemCount));
                                        }
                                    }
                                    else
                                    {
                                        // Destination is full, keep it as a drop
                                        remainingDropItems.Add(dropItem);
                                    }
                                }
                                else
                                {
                                    // Item is not autolootable, keep it as a drop
                                    remainingDropItems.Add(dropItem);
                                }
                            }

                            if (autolootNtc.UpdateItemList.Count > 0 || autolootNtc.UpdateWalletList.Count > 0)
                            {
                                partyMemberClient.Enqueue(autolootNtc, queuedPackets);
                            }

                            // Send chat notification for items sent to storage
                            if (storageItems.Any())
                            {
                                var storageMessage = string.Join(", ", storageItems.Select(x => $"{x.Name} x{x.Count}"));
                                var chatType = (LobbyChatMsgType)Server.GameSettings.GameServerSettings.AutolootNotificationChatType;
                                Server.ChatManager.SendMessage($"[Storage] {storageMessage}", string.Empty, string.Empty, chatType, new List<GameClient> { partyMemberClient });
                            }
                        }
                        else
                        {
                            remainingDropItems = allDropItems;
                        }

                        // Only assign and show drops for items that weren't auto-looted
                        if (remainingDropItems.Any())
                        {
                            uint offsetSetId = partyMemberClient.InstanceDropItemManager.Assign(layoutId, packet.SetId, remainingDropItems);
                            var dropItemNtc = new S2CInstancePopDropItemNtc()
                            {
                                LayoutId = packet.LayoutId,
                                SetId = offsetSetId,
                                MdlType = enemyKilled.DropsTable?.MdlType ?? 0,
                                PosX = packet.DropPosX,
                                PosY = packet.DropPosY,
                                PosZ = packet.DropPosZ
                            };

                            if (instancedGatheringItems[typeof(EnemyEpitaphRoadDropGenerator)].Any())
                            {
                                dropItemNtc.MdlType = 1; // Make the bag appear as golden
                            }

                            partyMemberClient.Enqueue(dropItemNtc, queuedPackets);
                        }
                    }

                    // TODO: This will be revisited so we can properly handle EXP assigned by tool and
                    // TODO: EXP determined by the mixin. For now, the default behavior of the mixin
                    // TODO: is the same as the original server behavior.
                    var enemyExpMixin = Server.ScriptManager.MixinModule.Get<IExpMixin>("enemy_exp");

                    foreach (PartyMember member in client.Party.Members)
                    {
                        if (member.JoinState != JoinState.On) continue; // Only fully joined members get rewards.

                        GameClient memberClient;
                        CharacterCommon memberCharacter;
                        if (member is PlayerPartyMember playerMember)
                        {
                            var gainedExp = _gameServer.ExpManager.GetAdjustedPoints(client, RewardSource.Enemy, client.Character, client.Party, PointType.ExperiencePoints, enemyExpMixin.GetExpValue(playerMember.Client.Character, enemyKilled), enemyKilled);
                            var gainedPP = _gameServer.ExpManager.GetAdjustedPoints(client, RewardSource.Enemy, client.Character, client.Party, PointType.PlayPoints, enemyKilled.GetDroppedPlayPoints(), enemyKilled);

                            memberClient = playerMember.Client;
                            memberCharacter = memberClient.Character;

                            if (memberCharacter.Stage.Id != stageId.Id) continue; // Only nearby allies get XP.

                            if (memberClient.Character.ActiveCharacterPlayPointData.PlayPoint.ExpMode == ExpMode.Experience && !isQuestControlled && !isEpitaphEnemy)
                            {
                                gainedPP = (0, 0);
                            }
                            else if (!isQuestControlled && !isEpitaphEnemy)
                            {
                                gainedExp = (0, 0);
                            }

                            var huntPackets = playerMember.QuestState.HandleEnemyHuntRequests(enemyKilled, connectionIn);
                            queuedPackets.AddRange(huntPackets);

                            S2CItemUpdateCharacterItemNtc updateCharacterItemNtc = new S2CItemUpdateCharacterItemNtc();

                            if (enemyKilled.BloodOrbs > 0)
                            {
                                // Drop BO
                                uint gainedBo = (uint)(enemyKilled.BloodOrbs * _gameServer.GameSettings.GameServerSettings.BoModifier);
                                uint bonusBo = (uint)(gainedBo * _gameServer.GpCourseManager.EnemyBloodOrbBonus());
                                CDataUpdateWalletPoint boUpdateWalletPoint = _gameServer.WalletManager.AddToWallet(memberClient.Character, WalletType.BloodOrbs, gainedBo + bonusBo, bonusBo, connectionIn: connectionIn);
                                updateCharacterItemNtc.UpdateWalletList.Add(boUpdateWalletPoint);
                            }

                            if (enemyKilled.HighOrbs > 0)
                            {
                                // Drop HO
                                uint gainedHo = (uint)(enemyKilled.HighOrbs * _gameServer.GameSettings.GameServerSettings.HoModifier);
                                CDataUpdateWalletPoint hoUpdateWalletPoint = _gameServer.WalletManager.AddToWallet(memberClient.Character, WalletType.HighOrbs, gainedHo, connectionIn: connectionIn);
                                updateCharacterItemNtc.UpdateWalletList.Add(hoUpdateWalletPoint);
                            }

                            if (updateCharacterItemNtc.UpdateItemList.Count != 0 || updateCharacterItemNtc.UpdateWalletList.Count != 0)
                            {
                                memberClient.Enqueue(updateCharacterItemNtc, queuedPackets);
                            }

                            if ((gainedPP.BasePoints + gainedPP.BonusPoints) > 0)
                            {
                                var ntc = _gameServer.PPManager.AddPlayPoint(memberClient, gainedPP, type: 1, connectionIn: connectionIn);
                                memberClient.Enqueue(ntc, queuedPackets);
                            }

                            if ((gainedExp.BasePoints + gainedExp.BonusPoints) > 0)
                            {
                                var ntcs = _gameServer.ExpManager.AddExp(memberClient, memberCharacter, gainedExp, RewardSource.Enemy, connectionIn: connectionIn);
                                queuedPackets.AddRange(ntcs);
                            }

                            queuedPackets.AddRange(Server.AchievementManager.HandleKillEnemy(memberClient, enemyKilled, connectionIn: connectionIn));
                            queuedPackets.AddRange(Server.JobMasterManager.HandleEnemyKill(memberClient, enemyKilled, connectionIn));
                            queuedPackets.AddRange(Server.RewardMissionManager.UpdateMissionProgress(memberClient, DailyMissionType.KillEnemy, 1, connectionIn));
                        }
                        else if (member is PawnPartyMember pawnMember)
                        {
                            Pawn pawn = pawnMember.Pawn;
                            memberClient = _gameServer.ClientLookup.GetClientByCharacterId(pawn.CharacterId);
                            memberCharacter = pawn;

                            if (memberClient is null || memberClient.Character.Stage.Id != stageId.Id || pawn.IsRented)
                            {
                                // Only nearby allies get XP
                                // and non-rented pawns
                                continue;
                            }

                            var pawnExp = _gameServer.ExpManager.GetAdjustedPoints(client, RewardSource.Enemy, pawn, client.Party, PointType.ExperiencePoints, enemyExpMixin.GetExpValue(memberCharacter, enemyKilled), enemyKilled);
                            if ((pawnExp.BasePoints + pawnExp.BonusPoints) > 0)
                            {
                                var ntcs = _gameServer.ExpManager.AddExp(memberClient, memberCharacter, pawnExp, RewardSource.Enemy, connectionIn: connectionIn);
                                queuedPackets.AddRange(ntcs);
                            }

                            if (pawn is RentalPawn rentalPawn)
                            {
                                Server.RentalPawnManager.HandleEnemyKill(rentalPawn, connectionIn);
                            }
                        }
                        else
                        {
                            throw new Exception("Unknown member type");
                        }
                    }
                }
            });

            queuedPackets.Send();

            // TODO: EnemyId and KillNum
            return new S2CInstanceEnemyKillRes()
            {
                EnemyId = packet.IsNoBattleReward ? 0u : enemyKilled.EnemyId,
                KillNum = packet.IsNoBattleReward ? 0u : 1u
            };

        }
    }
}
