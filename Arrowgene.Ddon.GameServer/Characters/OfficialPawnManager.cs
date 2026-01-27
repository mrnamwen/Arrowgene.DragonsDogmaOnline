using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Characters
{
    public class OfficialPawnManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(OfficialPawnManager));

        private readonly DdonGameServer _server;
        private readonly Dictionary<uint, Pawn> _officialPawns;

        public OfficialPawnManager(DdonGameServer server)
        {
            _server = server;
            _officialPawns = new Dictionary<uint, Pawn>();
        }

        public void Initialize()
        {
            _officialPawns.Clear();

            foreach (var officialPawnAsset in _server.AssetRepository.OfficialPawnsAsset)
            {
                var pawn = CreatePawnFromAsset(officialPawnAsset);
                _officialPawns[pawn.PawnId] = pawn;
                Logger.Info($"Loaded official pawn: {pawn.Name} (PawnId: {pawn.PawnId}, Job: {pawn.Job}, Level: {pawn.ActiveCharacterJobData?.Lv})");
            }

            Logger.Info($"Loaded {_officialPawns.Count} official pawns");
        }

        public Pawn GetOfficialPawn(uint pawnId)
        {
            return _officialPawns.TryGetValue(pawnId, out var pawn) ? pawn : null;
        }

        public bool IsOfficialPawn(uint pawnId)
        {
            return _officialPawns.ContainsKey(pawnId);
        }

        public List<Pawn> GetAllOfficialPawns()
        {
            return _officialPawns.Values.ToList();
        }

        public List<CDataRegisterdPawnList> GetOfficialPawnsForSearch(CDataPawnSearchParameter searchParams)
        {
            var results = new List<CDataRegisterdPawnList>();

            foreach (var pawn in _officialPawns.Values)
            {
                if (!MatchesSearchCriteria(pawn, searchParams))
                {
                    continue;
                }

                results.Add(pawn.CDataRegisterdPawnList);
            }

            return results;
        }

        private bool MatchesSearchCriteria(Pawn pawn, CDataPawnSearchParameter searchParams)
        {
            // Filter by pawn name
            if (!string.IsNullOrEmpty(searchParams.PawnName) &&
                !pawn.Name.Contains(searchParams.PawnName, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Filter by sex
            if (searchParams.Sex != PawnSex.Any && (byte)searchParams.Sex != pawn.EditInfo.Sex)
            {
                return false;
            }

            // Filter by job
            if (searchParams.CharacterParam.Job != 0)
            {
                uint jobBitfield = searchParams.CharacterParam.Job;
                uint pawnJobBit = 1u << (byte)pawn.Job;
                if ((jobBitfield & pawnJobBit) == 0)
                {
                    return false;
                }
            }

            // Filter by level
            var pawnLevel = pawn.ActiveCharacterJobData?.Lv ?? 0;
            if (searchParams.CharacterParam.VocationMin > 0 || searchParams.CharacterParam.VocationMax > 0)
            {
                if (pawnLevel < searchParams.CharacterParam.VocationMin ||
                    (searchParams.CharacterParam.VocationMax > 0 && pawnLevel > searchParams.CharacterParam.VocationMax))
                {
                    return false;
                }
            }

            // Filter by craft rank
            if (searchParams.CraftRankMin > 0 || searchParams.CraftRankMax > 0)
            {
                var craftRank = pawn.CraftData.CraftRank;
                if (craftRank < searchParams.CraftRankMin ||
                    (searchParams.CraftRankMax > 0 && craftRank > searchParams.CraftRankMax))
                {
                    return false;
                }
            }

            return true;
        }

        private Pawn CreatePawnFromAsset(OfficialPawn asset)
        {
            var pawn = new Pawn
            {
                PawnId = asset.PawnId,
                CharacterId = 0,
                Name = asset.Name,
                Job = asset.Job,
                HmType = asset.HmType,
                PawnType = asset.PawnType,
                EditInfo = asset.EditInfo,
                HideEquipHead = asset.HideEquipHead,
                HideEquipLantern = asset.HideEquipLantern,
                IsOfficialPawn = true,
                PawnReactionList = asset.PawnReactions,
            };

            // Set up job data
            var jobData = new CDataCharacterJobData
            {
                Job = asset.Job,
                Lv = (byte)asset.Level,
                Exp = 0,
                JobPoint = 0,
                // Set reasonable default stats for the level
                Atk = (ushort)(100 + asset.Level * 5),
                Def = (ushort)(100 + asset.Level * 3),
                MAtk = (ushort)(100 + asset.Level * 4),
                MDef = (ushort)(100 + asset.Level * 3),
                Strength = (ushort)(50 + asset.Level),
                DownPower = (ushort)(50 + asset.Level),
                ShakePower = (ushort)(50 + asset.Level),
                StunPower = (ushort)(50 + asset.Level),
                Constitution = (ushort)(50 + asset.Level),
                Guts = (ushort)(50 + asset.Level),
            };
            pawn.CharacterJobDataList = [jobData];

            // Set up status info
            pawn.StatusInfo = new CDataStatusInfo
            {
                HP = (uint)(500 + asset.Level * 50),
                MaxHP = (uint)(500 + asset.Level * 50),
                WhiteHP = 0,
                Stamina = (uint)(300 + asset.Level * 20),
                MaxStamina = (uint)(300 + asset.Level * 20),
                RevivePoint = 3,
            };

            // Set up craft data
            pawn.CraftData = new CDataPawnCraftData
            {
                CraftExp = 0,
                CraftRank = asset.CraftRank,
                CraftRankLimit = 100,
                CraftPoint = 0,
                PawnCraftSkillList = asset.CraftSkills.Count > 0
                    ? asset.CraftSkills
                    : GetDefaultCraftSkills()
            };

            // Set up equipment using an in-memory Storage
            // Official pawns need 30 slots (15 for performance + 15 for visual equipment)
            var equipmentStorage = new Storage(StorageType.PawnEquipment, (ushort)(EquipmentTemplate.TOTAL_EQUIP_SLOTS * 2));
            pawn.EquipmentTemplate = new EquipmentTemplate();
            pawn.Equipment = new Equipment(equipmentStorage, 0);

            foreach (var (slot, itemId) in asset.Equipment)
            {
                if (itemId > 0)
                {
                    var item = new Item { ItemId = itemId };
                    pawn.EquipmentTemplate.SetEquipItem(item, asset.Job, EquipType.Performance, slot);
                    // Also set in Storage for Equipment class to work
                    equipmentStorage.SetItem(item, 1, slot);
                }
            }

            foreach (var (slot, itemId) in asset.VisualEquipment)
            {
                if (itemId > 0)
                {
                    var item = new Item { ItemId = itemId };
                    pawn.EquipmentTemplate.SetEquipItem(item, asset.Job, EquipType.Visual, slot);
                    // Visual equipment goes after performance equipment in storage
                    equipmentStorage.SetItem(item, 1, (ushort)(slot + EquipmentTemplate.TOTAL_EQUIP_SLOTS));
                }
            }

            // Set up custom skills
            foreach (var skillAsset in asset.CustomSkills)
            {
                var skill = new CustomSkill
                {
                    Job = asset.Job,
                    SkillId = skillAsset.SkillId,
                    SkillLv = skillAsset.SkillLv
                };
                pawn.LearnedCustomSkills.Add(skill);

                if (skillAsset.SlotNo > 0 && skillAsset.SlotNo <= pawn.EquippedCustomSkillsDictionary[asset.Job].Count)
                {
                    pawn.EquippedCustomSkillsDictionary[asset.Job][skillAsset.SlotNo - 1] = skill;
                }
            }

            // Set up abilities
            foreach (var abilityAsset in asset.Abilities)
            {
                var ability = new Ability
                {
                    AbilityId = (AbilityId)abilityAsset.AbilityId,
                    AbilityLv = abilityAsset.AbilityLv
                };
                pawn.LearnedAbilities.Add(ability);

                if (abilityAsset.SlotNo > 0 && abilityAsset.SlotNo <= pawn.EquippedAbilitiesDictionary[asset.Job].Count)
                {
                    pawn.EquippedAbilitiesDictionary[asset.Job][abilityAsset.SlotNo - 1] = ability;
                }
            }

            // Set up SP skills
            if (asset.SpSkills.Count > 0)
            {
                pawn.SpSkills[asset.Job] = asset.SpSkills;
            }

            return pawn;
        }

        private List<CDataPawnCraftSkill> GetDefaultCraftSkills()
        {
            return
            [
                new() { Type = CraftSkillType.ProductionSpeed, Level = 0 },
                new() { Type = CraftSkillType.EquipmentEnhancement, Level = 0 },
                new() { Type = CraftSkillType.EquipmentQuality, Level = 0 },
                new() { Type = CraftSkillType.ConsumableQuantity, Level = 0 },
                new() { Type = CraftSkillType.CostPerformance, Level = 0 },
                new() { Type = CraftSkillType.ConsumableProductionIsAlwaysGreatSuccess, Level = 0 },
                new() { Type = CraftSkillType.CreatingHighQualityEquipmentIsAlwaysGreatSuccess, Level = 0 },
                new() { Type = CraftSkillType.CostPerformanceEffectUpFactor1, Level = 0 },
                new() { Type = CraftSkillType.CostPerformanceEffectUpFactor2, Level = 0 },
                new() { Type = CraftSkillType.UnknownEffect10, Level = 0 }
            ];
        }
    }
}
