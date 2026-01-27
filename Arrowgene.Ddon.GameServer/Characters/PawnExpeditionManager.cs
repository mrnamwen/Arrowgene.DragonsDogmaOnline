using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Characters
{
    public class PawnExpeditionManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnExpeditionManager));

        public const int DEFAULT_SALLY_COUNT = 3;
        public const int MAX_SALLY_COUNT = 5;
        public const int EXPEDITION_DURATION_HOURS = 4;
        public const int GOLDEN_EXPEDITION_DURATION_HOURS = 2;
        public const byte CHARGE_SALLY_PRICE = 10; // Golden gems to recharge
        public const byte GOLDEN_SALLY_PRICE = 5; // Golden gems for golden expedition
        public const int HOT_SPOT_DISCOVERY_CHANCE = 15; // % chance to discover hot spot
        public const int HOT_SPOT_ROTATION_DAYS = 7; // Days before hot spots rotate

        private readonly DdonGameServer _Server;
        private readonly object _Lock = new object();

        // Character ID -> List of active expeditions
        private readonly Dictionary<uint, List<PawnExpedition>> _ActiveExpeditions;
        // Character ID -> Sally count
        private readonly Dictionary<uint, int> _SallyCount;
        // Character ID -> Unclaimed rewards
        private readonly Dictionary<uint, List<CDataPawnExpeditionReward>> _UnclaimedRewards;
        // Character ID -> Discovered hot spots (persists until weekly reset)
        private readonly Dictionary<uint, List<CDataAreaSpotSet>> _DiscoveredHotSpots;

        // Available areas for expeditions
        private readonly List<uint> _AvailableAreaIds;
        // Current global hot spots (rotates weekly, seeded by date)
        private List<CDataAreaSpotSet> _CurrentHotSpots;
        private DateTime _LastHotSpotRotation;

        public class PawnExpedition
        {
            public uint PawnId { get; set; }
            public uint AreaId { get; set; }
            public uint SpotId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public bool IsGolden { get; set; }
            public PawnExpeditionStatus Status { get; set; }
        }

        public PawnExpeditionManager(DdonGameServer server)
        {
            _Server = server;
            _ActiveExpeditions = new Dictionary<uint, List<PawnExpedition>>();
            _SallyCount = new Dictionary<uint, int>();
            _UnclaimedRewards = new Dictionary<uint, List<CDataPawnExpeditionReward>>();
            _DiscoveredHotSpots = new Dictionary<uint, List<CDataAreaSpotSet>>();

            // Initialize available areas (common areas in the game)
            _AvailableAreaIds = new List<uint>
            {
                (uint)QuestAreaId.HidellPlains,
                (uint)QuestAreaId.BreyaCoast,
                (uint)QuestAreaId.MysreeForest,
                (uint)QuestAreaId.VoldenMines,
                (uint)QuestAreaId.DoweValley,
                (uint)QuestAreaId.BetlandPlains,
                (uint)QuestAreaId.ZandoraWastelands,
                (uint)QuestAreaId.MergodaRuins,
                (uint)QuestAreaId.BloodbaneIsle,
                (uint)QuestAreaId.ElanWaterGrove,
                (uint)QuestAreaId.FeryanaWilderness,
                (uint)QuestAreaId.MorrowForest
            };

            // Initialize hot spots with weekly rotation
            _CurrentHotSpots = new List<CDataAreaSpotSet>();
            _LastHotSpotRotation = DateTime.MinValue;
            RotateHotSpotsIfNeeded();
        }

        /// <summary>
        /// Rotates hot spots weekly based on the current date.
        /// Uses a seeded random so all servers get the same hot spots for the same week.
        /// </summary>
        private void RotateHotSpotsIfNeeded()
        {
            // Calculate the current "week" since a reference date
            var referenceDate = new DateTime(2024, 1, 1, 5, 0, 0, DateTimeKind.Utc); // 5:00 AM JST reference
            var daysSinceReference = (int)(DateTime.UtcNow - referenceDate).TotalDays;
            var currentWeek = daysSinceReference / HOT_SPOT_ROTATION_DAYS;
            var weekStart = referenceDate.AddDays(currentWeek * HOT_SPOT_ROTATION_DAYS);

            if (_LastHotSpotRotation >= weekStart)
            {
                return; // Already rotated this week
            }

            _LastHotSpotRotation = DateTime.UtcNow;

            // Create a seeded random based on the week number for consistent rotation
            var seededRandom = new Random(currentWeek * 12345);
            var shuffledAreas = _AvailableAreaIds.OrderBy(x => seededRandom.Next()).ToList();

            // Select 3 areas as hot spots for this week
            _CurrentHotSpots = new List<CDataAreaSpotSet>();
            for (int i = 0; i < Math.Min(3, shuffledAreas.Count); i++)
            {
                _CurrentHotSpots.Add(new CDataAreaSpotSet
                {
                    AreaId = (QuestAreaId)shuffledAreas[i],
                    SpotId = 1 // Default spot
                });
            }

            Logger.Info($"Hot spots rotated for week {currentWeek}: {string.Join(", ", _CurrentHotSpots.Select(h => h.AreaId))}");

            // Clear discovered hot spots on rotation
            lock (_Lock)
            {
                _DiscoveredHotSpots.Clear();
            }
        }

        public int GetSallyCount(uint characterId)
        {
            lock (_Lock)
            {
                if (!_SallyCount.ContainsKey(characterId))
                {
                    _SallyCount[characterId] = DEFAULT_SALLY_COUNT;
                }
                return _SallyCount[characterId];
            }
        }

        public List<uint> GetAvailableAreaIds()
        {
            return _AvailableAreaIds;
        }

        public List<CDataAreaSpotSet> GetHotSpots()
        {
            RotateHotSpotsIfNeeded();
            return _CurrentHotSpots;
        }

        /// <summary>
        /// Gets the hot spots discovered by a specific character.
        /// </summary>
        public List<CDataAreaSpotSet> GetDiscoveredHotSpots(uint characterId)
        {
            lock (_Lock)
            {
                if (!_DiscoveredHotSpots.ContainsKey(characterId))
                {
                    return new List<CDataAreaSpotSet>();
                }
                return _DiscoveredHotSpots[characterId].ToList();
            }
        }

        /// <summary>
        /// Attempts to discover a hot spot when an expedition completes.
        /// Returns true if a new hot spot was discovered.
        /// </summary>
        public bool TryDiscoverHotSpot(uint characterId, uint areaId, out CDataAreaSpotSet discoveredSpot)
        {
            discoveredSpot = null;
            RotateHotSpotsIfNeeded();

            lock (_Lock)
            {
                // Check if this area has a hot spot this week
                var hotSpot = _CurrentHotSpots.FirstOrDefault(h => (uint)h.AreaId == areaId);
                if (hotSpot == null)
                {
                    return false;
                }

                // Check if already discovered
                if (!_DiscoveredHotSpots.ContainsKey(characterId))
                {
                    _DiscoveredHotSpots[characterId] = new List<CDataAreaSpotSet>();
                }

                if (_DiscoveredHotSpots[characterId].Any(h => (uint)h.AreaId == areaId))
                {
                    return false; // Already discovered this hot spot
                }

                // Random chance to discover
                var random = new Random();
                if (random.Next(100) < HOT_SPOT_DISCOVERY_CHANCE)
                {
                    discoveredSpot = new CDataAreaSpotSet
                    {
                        AreaId = hotSpot.AreaId,
                        SpotId = hotSpot.SpotId
                    };
                    _DiscoveredHotSpots[characterId].Add(discoveredSpot);
                    Logger.Info($"Character {characterId} discovered hot spot in {hotSpot.AreaId}!");
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Checks if the given area and spot is a hot spot for the character.
        /// Either globally active or discovered by the character.
        /// </summary>
        public bool IsHotSpot(uint characterId, uint areaId, uint spotId)
        {
            RotateHotSpotsIfNeeded();

            // Check global hot spots
            if (_CurrentHotSpots.Any(h => (uint)h.AreaId == areaId))
            {
                // Check if character has discovered it
                lock (_Lock)
                {
                    if (_DiscoveredHotSpots.ContainsKey(characterId))
                    {
                        return _DiscoveredHotSpots[characterId].Any(h => (uint)h.AreaId == areaId);
                    }
                }
            }
            return false;
        }

        public List<CDataPawnExpeditionMySallyInfo> GetMyExpeditions(uint characterId)
        {
            lock (_Lock)
            {
                if (!_ActiveExpeditions.ContainsKey(characterId))
                {
                    return new List<CDataPawnExpeditionMySallyInfo>();
                }

                var result = new List<CDataPawnExpeditionMySallyInfo>();
                foreach (var exp in _ActiveExpeditions[characterId])
                {
                    result.Add(new CDataPawnExpeditionMySallyInfo
                    {
                        PawnId = exp.PawnId,
                        AreaId = exp.AreaId,
                        SpotId = exp.SpotId,
                        StartTime = (ulong)((DateTimeOffset)exp.StartTime).ToUnixTimeSeconds(),
                        EndTime = (ulong)((DateTimeOffset)exp.EndTime).ToUnixTimeSeconds(),
                        IsGolden = exp.IsGolden,
                        Status = (byte)GetExpeditionStatus(exp)
                    });
                }
                return result;
            }
        }

        private PawnExpeditionStatus GetExpeditionStatus(PawnExpedition exp)
        {
            if (DateTime.UtcNow >= exp.EndTime)
            {
                return PawnExpeditionStatus.Returned;
            }
            return PawnExpeditionStatus.OnSally;
        }

        public bool StartExpedition(uint characterId, uint pawnId, uint areaId, uint spotId, bool isGolden, out DateTime endTime)
        {
            lock (_Lock)
            {
                endTime = DateTime.MinValue;

                // Check sally count
                int sallyCount = GetSallyCount(characterId);
                if (sallyCount <= 0)
                {
                    Logger.Error($"Character {characterId} has no sally count remaining");
                    return false;
                }

                // Check if pawn is already on expedition
                if (IsPawnOnExpedition(characterId, pawnId))
                {
                    Logger.Error($"Pawn {pawnId} is already on expedition");
                    return false;
                }

                // Create expedition
                var startTime = DateTime.UtcNow;
                var duration = isGolden ? TimeSpan.FromHours(GOLDEN_EXPEDITION_DURATION_HOURS) : TimeSpan.FromHours(EXPEDITION_DURATION_HOURS);
                endTime = startTime + duration;

                var expedition = new PawnExpedition
                {
                    PawnId = pawnId,
                    AreaId = areaId,
                    SpotId = spotId,
                    StartTime = startTime,
                    EndTime = endTime,
                    IsGolden = isGolden,
                    Status = PawnExpeditionStatus.OnSally
                };

                if (!_ActiveExpeditions.ContainsKey(characterId))
                {
                    _ActiveExpeditions[characterId] = new List<PawnExpedition>();
                }
                _ActiveExpeditions[characterId].Add(expedition);

                // Decrease sally count
                _SallyCount[characterId] = sallyCount - 1;

                Logger.Info($"Started expedition for pawn {pawnId} in area {areaId}, ends at {endTime}");
                return true;
            }
        }

        public bool CancelExpedition(uint characterId, uint pawnId, out int newSallyCount)
        {
            lock (_Lock)
            {
                newSallyCount = GetSallyCount(characterId);

                if (!_ActiveExpeditions.ContainsKey(characterId))
                {
                    return false;
                }

                var expedition = _ActiveExpeditions[characterId].FirstOrDefault(e => e.PawnId == pawnId);
                if (expedition == null)
                {
                    return false;
                }

                _ActiveExpeditions[characterId].Remove(expedition);

                // Refund sally count if cancelled early
                newSallyCount = Math.Min(MAX_SALLY_COUNT, newSallyCount + 1);
                _SallyCount[characterId] = newSallyCount;

                Logger.Info($"Cancelled expedition for pawn {pawnId}");
                return true;
            }
        }

        public bool ChangeToGoldenExpedition(uint characterId, uint pawnId)
        {
            lock (_Lock)
            {
                if (!_ActiveExpeditions.ContainsKey(characterId))
                {
                    return false;
                }

                var expedition = _ActiveExpeditions[characterId].FirstOrDefault(e => e.PawnId == pawnId);
                if (expedition == null || expedition.IsGolden)
                {
                    return false;
                }

                // Change to golden (halves remaining time, better rewards)
                expedition.IsGolden = true;
                var remaining = expedition.EndTime - DateTime.UtcNow;
                expedition.EndTime = DateTime.UtcNow + TimeSpan.FromTicks(remaining.Ticks / 2);

                Logger.Info($"Changed expedition for pawn {pawnId} to golden");
                return true;
            }
        }

        public bool ChargeSallyCount(uint characterId, out int newSallyCount)
        {
            lock (_Lock)
            {
                newSallyCount = GetSallyCount(characterId);
                if (newSallyCount >= MAX_SALLY_COUNT)
                {
                    return false;
                }

                newSallyCount = MAX_SALLY_COUNT;
                _SallyCount[characterId] = newSallyCount;
                return true;
            }
        }

        public List<CDataPawnExpeditionReward> GetExpeditionRewards(uint characterId, uint pawnId, out bool discoveredHotSpot, out CDataAreaSpotSet discoveredSpotInfo)
        {
            discoveredHotSpot = false;
            discoveredSpotInfo = null;

            lock (_Lock)
            {
                if (!_ActiveExpeditions.ContainsKey(characterId))
                {
                    return new List<CDataPawnExpeditionReward>();
                }

                var expedition = _ActiveExpeditions[characterId].FirstOrDefault(e => e.PawnId == pawnId);
                if (expedition == null || DateTime.UtcNow < expedition.EndTime)
                {
                    return new List<CDataPawnExpeditionReward>();
                }

                // Check if the expedition was at a hot spot the character has discovered
                bool wasAtHotSpot = IsHotSpot(characterId, expedition.AreaId, expedition.SpotId);

                // Generate rewards based on area, golden status, and hot spot
                var rewards = GenerateRewards(characterId, expedition, wasAtHotSpot);

                // Try to discover a hot spot in this area (if not already discovered)
                if (TryDiscoverHotSpot(characterId, expedition.AreaId, out discoveredSpotInfo))
                {
                    discoveredHotSpot = true;
                }

                // Remove completed expedition
                _ActiveExpeditions[characterId].Remove(expedition);

                // Store rewards for claiming
                if (!_UnclaimedRewards.ContainsKey(characterId))
                {
                    _UnclaimedRewards[characterId] = new List<CDataPawnExpeditionReward>();
                }
                _UnclaimedRewards[characterId].AddRange(rewards);

                return rewards;
            }
        }

        public bool HasUnclaimedRewards(uint characterId)
        {
            lock (_Lock)
            {
                return _UnclaimedRewards.ContainsKey(characterId) && _UnclaimedRewards[characterId].Count > 0;
            }
        }

        public List<CDataPawnExpeditionReward> GetUnclaimedRewards(uint characterId)
        {
            lock (_Lock)
            {
                if (!_UnclaimedRewards.ContainsKey(characterId))
                {
                    return new List<CDataPawnExpeditionReward>();
                }
                return _UnclaimedRewards[characterId].ToList();
            }
        }

        public CDataPawnExpeditionReward ClaimReward(uint characterId, int index)
        {
            lock (_Lock)
            {
                if (!_UnclaimedRewards.ContainsKey(characterId) || index >= _UnclaimedRewards[characterId].Count)
                {
                    return null;
                }

                var reward = _UnclaimedRewards[characterId][index];
                _UnclaimedRewards[characterId].RemoveAt(index);
                return reward;
            }
        }

        private bool IsPawnOnExpedition(uint characterId, uint pawnId)
        {
            if (!_ActiveExpeditions.ContainsKey(characterId))
            {
                return false;
            }
            return _ActiveExpeditions[characterId].Any(e => e.PawnId == pawnId);
        }

        private List<CDataPawnExpeditionReward> GenerateRewards(uint characterId, PawnExpedition expedition, bool wasAtHotSpot)
        {
            var rewards = new List<CDataPawnExpeditionReward>();
            var random = new Random();

            // Get area-specific rewards from asset
            var areaRewards = _Server.AssetRepository.PawnExpeditionRewardsAsset.GetAreaRewards(expedition.AreaId);
            if (areaRewards == null)
            {
                Logger.Error($"No reward data found for area {expedition.AreaId}, using default rewards");
                // Fallback: just give some basic materials
                rewards.Add(new CDataPawnExpeditionReward { ItemId = 7790, Num = 1 }); // Coin Pouch 10G
                return rewards;
            }

            // Determine reward counts based on golden status
            int commonRewardCount = expedition.IsGolden ? random.Next(3, 5) : random.Next(2, 4);
            int rareChance = expedition.IsGolden ? 40 : 15; // % chance for rare reward

            // Add common rewards
            if (areaRewards.CommonRewards.Count > 0)
            {
                for (int i = 0; i < commonRewardCount; i++)
                {
                    var rewardItem = areaRewards.CommonRewards[random.Next(areaRewards.CommonRewards.Count)];
                    uint quantity = (uint)random.Next((int)rewardItem.MinNum, (int)rewardItem.MaxNum + 1);
                    if (expedition.IsGolden)
                    {
                        quantity = Math.Min(quantity * 2, rewardItem.MaxNum * 2); // Double for golden
                    }
                    rewards.Add(new CDataPawnExpeditionReward
                    {
                        ItemId = rewardItem.ItemId,
                        Num = quantity
                    });
                }
            }

            // Add rare rewards based on chance
            if (areaRewards.RareRewards.Count > 0 && random.Next(100) < rareChance)
            {
                var rewardItem = areaRewards.RareRewards[random.Next(areaRewards.RareRewards.Count)];
                uint quantity = (uint)random.Next((int)rewardItem.MinNum, (int)rewardItem.MaxNum + 1);
                rewards.Add(new CDataPawnExpeditionReward
                {
                    ItemId = rewardItem.ItemId,
                    Num = quantity
                });
            }

            // If this expedition was at a discovered hot spot, give special rewards
            if (wasAtHotSpot && areaRewards.HotSpotRewards.Count > 0)
            {
                // Hot spots always give special rewards
                var rewardItem = areaRewards.HotSpotRewards[random.Next(areaRewards.HotSpotRewards.Count)];
                uint quantity = (uint)random.Next((int)rewardItem.MinNum, (int)rewardItem.MaxNum + 1);
                rewards.Add(new CDataPawnExpeditionReward
                {
                    ItemId = rewardItem.ItemId,
                    Num = quantity
                });
                Logger.Info($"Pawn found hot spot reward in area {expedition.AreaId}!");
            }

            return rewards;
        }
    }
}
