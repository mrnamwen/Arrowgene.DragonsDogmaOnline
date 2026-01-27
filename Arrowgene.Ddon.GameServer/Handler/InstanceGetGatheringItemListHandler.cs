using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Server.Network;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class InstanceGetGatheringItemListHandler : GameRequestPacketQueueHandler<C2SInstanceGetGatheringItemListReq, S2CInstanceGetGatheringItemListRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(InstanceGetGatheringItemListHandler));

        private static readonly Dictionary<ItemId, double> BreakChance = new Dictionary<ItemId, double>()
        {

        };

        public InstanceGetGatheringItemListHandler(DdonGameServer server) : base(server)
        {
        }

        public override PacketQueue Handle(GameClient client, C2SInstanceGetGatheringItemListReq request)
        {
            PacketQueue queue = new();

            var (isNew, gatheringItems) = client.InstanceGatheringItemManager.FetchOrGenerate(request.LayoutId, request.PosId);

            bool isGatheringItemBreak = false;
            bool autolootGathering = Server.GameSettings.GameServerSettings.AutolootGatheringItems;
            bool materialsToStorage = Server.GameSettings.GameServerSettings.AutolootMaterialsToStorage;

            Server.Database.ExecuteInTransaction(connection =>
            {
                if (isNew && request.GatheringItemUId.Any())
                {
                    var gatheringItem = Server.ItemManager.LookupInfoByUID(Server, request.GatheringItemUId, connection);
                    switch ((ItemId)gatheringItem.ItemId)
                    {
                        case ItemId.Pickaxe:
                        case ItemId.ArtisansPickaxe:
                        case ItemId.EnhancedPickaxe:
                            queue.AddRange(Server.AchievementManager.HandleCollect(client, AchievementCollectParam.Ore, connection));
                            break;
                        case ItemId.Lockpick:
                        case ItemId.AllPurposeLockpick:
                        case ItemId.EnhancedLockpick:
                            queue.AddRange(Server.AchievementManager.HandleCollect(client, AchievementCollectParam.Chest, connection));
                            break;
                        case ItemId.LumberKnife:
                        case ItemId.ArtisansLumberKnife:
                        case ItemId.EnhancedLumberKnife:
                            queue.AddRange(Server.AchievementManager.HandleCollect(client, AchievementCollectParam.Wood, connection));
                            break;
                    }

                    double breakChance = 0.3; // Set the default break chance in case an item is missing
                    var itemId = gatheringItem.ItemId;
                    if (Server.GameSettings.GameServerSettings.ToolBreakChance.ContainsKey((ItemId)itemId))
                    {
                        breakChance = Server.GameSettings.GameServerSettings.ToolBreakChance[(ItemId)itemId];
                    }

                    if (Random.Shared.NextDouble() < breakChance)
                    {
                        isGatheringItemBreak = true;

                        S2CItemUpdateCharacterItemNtc ntc = new S2CItemUpdateCharacterItemNtc();
                        ntc.UpdateItemList.AddRange(Server.ItemManager.ConsumeItemByUIdFromMultipleStorages(Server, client.Character, ItemManager.ItemBagStorageTypes, request.GatheringItemUId, 1, connection));
                        client.Enqueue(ntc, queue);
                    }
                }

                // Autoloot gathering items (treasure chests, mining nodes, etc.)
                if (autolootGathering && gatheringItems.Any())
                {
                    S2CItemUpdateCharacterItemNtc autolootNtc = new S2CItemUpdateCharacterItemNtc()
                    {
                        UpdateType = ItemNoticeType.Gather
                    };
                    List<(string Name, uint Count)> storageItems = new List<(string, uint)>();

                    foreach (var item in gatheringItems.Where(x => x.ItemNum > 0))
                    {
                        var clientItemInfo = Server.AssetRepository.ClientItemInfos.ContainsKey(item.ItemId)
                            ? Server.AssetRepository.ClientItemInfos[item.ItemId]
                            : null;

                        if (clientItemInfo == null)
                        {
                            continue;
                        }

                        // Category 1 = consumable, Category 2 = material
                        // Autoloot consumables and materials; equipment must still be manually picked up
                        bool isAutolootable = clientItemInfo.Category == 1 || clientItemInfo.Category == 2;

                        if (isAutolootable)
                        {
                            // Determine destination: materials go to storage if setting enabled, otherwise item bag
                            bool toItemBag = !(materialsToStorage && clientItemInfo.Category == 2);
                            var destinationStorageType = toItemBag ? clientItemInfo.StorageType : StorageType.StorageBoxNormal;

                            if (Server.ItemManager.CanAddItem(client.Character, destinationStorageType, (uint)item.ItemId, item.ItemNum))
                            {
                                uint itemCount = item.ItemNum;
                                queue.AddRange(Server.ItemManager.GatherItem(client, autolootNtc, item, itemCount, toItemBag, connection));

                                // Track items sent to storage for notification
                                if (!toItemBag)
                                {
                                    storageItems.Add((clientItemInfo.Name, itemCount));
                                }
                            }
                            // If destination is full, item remains for manual pickup
                        }
                        // Equipment and other items remain for manual pickup
                    }

                    if (autolootNtc.UpdateItemList.Count > 0 || autolootNtc.UpdateWalletList.Count > 0)
                    {
                        client.Enqueue(autolootNtc, queue);
                    }

                    // Send chat notification for items sent to storage
                    if (storageItems.Any())
                    {
                        var storageMessage = string.Join(", ", storageItems.Select(x => $"{x.Name} x{x.Count}"));
                        var chatType = (LobbyChatMsgType)Server.GameSettings.GameServerSettings.AutolootNotificationChatType;
                        Server.ChatManager.SendMessage($"[Storage] {storageMessage}", string.Empty, string.Empty, chatType, new List<GameClient> { client });
                    }
                }
            });

            // Build the item list response - only include items that weren't autolooted (ItemNum > 0)
            S2CInstanceGetGatheringItemListRes res = new()
            {
                LayoutId = request.LayoutId,
                PosId = request.PosId,
                GatheringItemUId = request.GatheringItemUId,
                IsGatheringItemBreak = isGatheringItemBreak,
                ItemList = gatheringItems
                .Select((asset, index) => new CDataGatheringItemElement()
                {
                    SlotNo = (uint)index,
                    ItemId = (uint) asset.ItemId,
                    ItemNum = asset.ItemNum,
                    Quality = asset.Quality,
                    IsHidden = asset.IsHidden
                })
                .ToList()
            };

            client.Enqueue(res, queue);

            return queue;
        }
    }
}
