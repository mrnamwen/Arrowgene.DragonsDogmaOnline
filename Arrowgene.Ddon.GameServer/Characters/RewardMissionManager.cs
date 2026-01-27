using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Server.Network;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Characters
{
    public class RewardMissionManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(RewardMissionManager));

        /// <summary>
        /// JST hour of the daily reset (5 AM JST like stamps)
        /// </summary>
        private static readonly int DAILY_RESET_HOUR = 5;

        /// <summary>
        /// Reward Medal item ID
        /// </summary>
        public static readonly uint REWARD_MEDAL_ITEM_ID = 24727;

        private readonly DdonGameServer Server;

        public RewardMissionManager(DdonGameServer server)
        {
            Server = server;
        }

        /// <summary>
        /// Get the daily missions from the asset repository
        /// </summary>
        public List<DailyMission> GetDailyMissions()
        {
            return Server.AssetRepository.DailyMissionAsset?.Missions
                .Where(m => m.Category == DailyMissionCategory.Daily)
                .OrderBy(m => m.SortId)
                .ToList() ?? new List<DailyMission>();
        }

        /// <summary>
        /// Get the weekly missions from the asset repository
        /// </summary>
        public List<DailyMission> GetWeeklyMissions()
        {
            return Server.AssetRepository.DailyMissionAsset?.Missions
                .Where(m => m.Category == DailyMissionCategory.Weekly)
                .OrderBy(m => m.SortId)
                .ToList() ?? new List<DailyMission>();
        }

        /// <summary>
        /// Get all missions
        /// </summary>
        public List<DailyMission> GetAllMissions()
        {
            return Server.AssetRepository.DailyMissionAsset?.Missions
                .OrderBy(m => m.SortId)
                .ToList() ?? new List<DailyMission>();
        }

        /// <summary>
        /// Get milestone rewards from the asset repository
        /// </summary>
        public List<MilestoneReward> GetMilestoneRewards()
        {
            return Server.AssetRepository.DailyMissionAsset?.MilestoneRewards ?? new List<MilestoneReward>();
        }

        /// <summary>
        /// Get milestone rewards for a specific category
        /// </summary>
        public List<MilestoneReward> GetMilestoneRewardsForCategory(DailyMissionCategory category)
        {
            return GetMilestoneRewards()
                .Where(m => m.Category == category)
                .OrderBy(m => m.RequiredMissionCount)
                .ToList();
        }

        /// <summary>
        /// Check if daily missions need to be reset based on last reset time
        /// </summary>
        public bool NeedsDailyReset(DateTime lastResetTime)
        {
            return GetTimeSinceReset(lastResetTime).TotalSeconds > 0;
        }

        /// <summary>
        /// Check if weekly missions need to be reset
        /// </summary>
        public bool NeedsWeeklyReset(DateTime lastResetTime)
        {
            TimeZoneInfo jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            DateTime jstNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jstZone);

            // Calculate time since last reset
            DateTime lastJstReset = GetLastDailyResetTime(lastResetTime);

            // Check if more than 7 days have passed
            return (jstNow - lastJstReset).TotalDays >= 7;
        }

        /// <summary>
        /// Calculate time span since/until the daily reset point
        /// </summary>
        private TimeSpan GetTimeSinceReset(DateTime lastTime)
        {
            DateTime utcNow = DateTime.UtcNow;
            TimeZoneInfo jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            DateTime jstNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, jstZone);

            DateTime lastResetPoint;
            if (lastTime.Hour < DAILY_RESET_HOUR)
            {
                lastResetPoint = lastTime.Date.AddHours(DAILY_RESET_HOUR);
            }
            else
            {
                lastResetPoint = lastTime.Date.AddDays(1).AddHours(DAILY_RESET_HOUR);
            }

            return jstNow - lastResetPoint;
        }

        private DateTime GetLastDailyResetTime(DateTime lastTime)
        {
            if (lastTime.Hour < DAILY_RESET_HOUR)
            {
                return lastTime.Date.AddHours(DAILY_RESET_HOUR);
            }
            else
            {
                return lastTime.Date.AddDays(1).AddHours(DAILY_RESET_HOUR);
            }
        }

        /// <summary>
        /// Reset daily mission progress for a character
        /// </summary>
        public void ResetDailyProgress(GameClient client, DbConnection? connectionIn = null)
        {
            uint characterId = client.Character.CharacterId;

            Server.Database.ExecuteQuerySafe(connectionIn, connection =>
            {
                // Delete existing progress
                Server.Database.DeleteRewardMissionProgress(characterId, connection);

                // Update state with current time
                var state = new RewardMissionState
                {
                    LastResetTime = DateTime.UtcNow,
                    MissionsCompleted = 0
                };
                Server.Database.UpsertRewardMissionState(characterId, state, connection);

                // Delete milestone claim status
                Server.Database.DeleteRewardMissionMilestones(characterId, connection);
            });

            Logger.Info($"Reset daily mission progress for character {characterId}");
        }

        /// <summary>
        /// Get mission progress for a character, resetting if necessary
        /// </summary>
        public List<CDataDailyMissionInfo> GetMissionInfoList(GameClient client, DbConnection? connectionIn = null)
        {
            uint characterId = client.Character.CharacterId;
            var result = new List<CDataDailyMissionInfo>();

            // Check if reset is needed
            var state = Server.Database.SelectRewardMissionState(characterId, connectionIn);
            if (NeedsDailyReset(state.LastResetTime))
            {
                ResetDailyProgress(client, connectionIn);
                state = new RewardMissionState
                {
                    LastResetTime = DateTime.UtcNow,
                    MissionsCompleted = 0
                };
            }

            // Get all progress entries
            var progressList = Server.Database.SelectRewardMissionProgress(characterId, connectionIn);
            var progressDict = progressList.ToDictionary(p => p.MissionId);

            // Build mission info list
            var missions = GetAllMissions();
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var dailyEndTime = GetNextDailyResetTimeUnix();
            var weeklyEndTime = GetNextWeeklyResetTimeUnix();

            foreach (var mission in missions)
            {
                var progress = progressDict.GetValueOrDefault(mission.MissionId) ?? new RewardMissionProgress
                {
                    MissionId = mission.MissionId,
                    CurrentCount = 0,
                    IsComplete = false,
                    IsReceived = false
                };

                var info = new CDataDailyMissionInfo
                {
                    MissionId = mission.MissionId,
                    Category = (uint)mission.Category,
                    Type = (uint)mission.Type,
                    TargetCount = mission.TargetCount,
                    CurrentCount = progress.CurrentCount,
                    IsComplete = progress.IsComplete,
                    IsReceived = progress.IsReceived,
                    SortId = mission.SortId,
                    Title = mission.Title,
                    IconUrl = mission.IconUrl,
                    ImageUrl = mission.ImageUrl,
                    StartTime = (ulong)now,
                    EndTime = mission.Category == DailyMissionCategory.Daily ? (ulong)dailyEndTime : (ulong)weeklyEndTime
                };

                foreach (var reward in mission.Rewards)
                {
                    info.RewardList.Add(new CDataDailyMissionReward
                    {
                        ItemId = reward.ItemId,
                        Num = reward.Num,
                        WalletType = reward.WalletType,
                        WalletAmount = reward.WalletAmount
                    });
                }

                result.Add(info);
            }

            return result;
        }

        /// <summary>
        /// Get next daily reset time as Unix timestamp
        /// </summary>
        private long GetNextDailyResetTimeUnix()
        {
            TimeZoneInfo jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            DateTime jstNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jstZone);

            DateTime nextReset;
            if (jstNow.Hour < DAILY_RESET_HOUR)
            {
                nextReset = jstNow.Date.AddHours(DAILY_RESET_HOUR);
            }
            else
            {
                nextReset = jstNow.Date.AddDays(1).AddHours(DAILY_RESET_HOUR);
            }

            DateTime nextResetUtc = TimeZoneInfo.ConvertTimeToUtc(nextReset, jstZone);
            return new DateTimeOffset(nextResetUtc).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Get next weekly reset time as Unix timestamp
        /// </summary>
        private long GetNextWeeklyResetTimeUnix()
        {
            TimeZoneInfo jstZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            DateTime jstNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jstZone);

            // Weekly reset is on Monday at 5 AM JST
            int daysUntilMonday = ((int)DayOfWeek.Monday - (int)jstNow.DayOfWeek + 7) % 7;
            if (daysUntilMonday == 0 && jstNow.Hour >= DAILY_RESET_HOUR)
            {
                daysUntilMonday = 7;
            }

            DateTime nextReset = jstNow.Date.AddDays(daysUntilMonday).AddHours(DAILY_RESET_HOUR);
            DateTime nextResetUtc = TimeZoneInfo.ConvertTimeToUtc(nextReset, jstZone);
            return new DateTimeOffset(nextResetUtc).ToUnixTimeSeconds();
        }

        /// <summary>
        /// Update mission progress when a relevant game event occurs
        /// </summary>
        public PacketQueue UpdateMissionProgress(GameClient client, DailyMissionType missionType, uint count = 1, DbConnection? connectionIn = null)
        {
            PacketQueue queue = new();
            uint characterId = client.Character.CharacterId;

            var missions = GetAllMissions().Where(m => m.Type == missionType).ToList();
            if (!missions.Any())
            {
                return queue;
            }

            Server.Database.ExecuteQuerySafe(connectionIn, connection =>
            {
                foreach (var mission in missions)
                {
                    var progress = Server.Database.SelectRewardMissionProgressByMissionId(characterId, mission.MissionId, connection);
                    if (progress == null)
                    {
                        progress = new RewardMissionProgress
                        {
                            MissionId = mission.MissionId,
                            CurrentCount = 0,
                            IsComplete = false,
                            IsReceived = false
                        };
                    }

                    // Skip if already complete
                    if (progress.IsComplete)
                    {
                        continue;
                    }

                    // Update count
                    progress.CurrentCount += count;
                    if (progress.CurrentCount >= mission.TargetCount)
                    {
                        progress.CurrentCount = mission.TargetCount;
                        progress.IsComplete = true;
                    }

                    Server.Database.UpsertRewardMissionProgress(characterId, progress, connection);
                }
            });

            return queue;
        }

        /// <summary>
        /// Claim reward for a completed mission
        /// </summary>
        public PacketQueue ClaimMissionReward(GameClient client, uint missionId, DbConnection? connectionIn = null)
        {
            PacketQueue queue = new();
            uint characterId = client.Character.CharacterId;

            var mission = GetAllMissions().FirstOrDefault(m => m.MissionId == missionId);
            if (mission == null)
            {
                Logger.Error($"Mission {missionId} not found");
                return queue;
            }

            Server.Database.ExecuteQuerySafe(connectionIn, connection =>
            {
                var progress = Server.Database.SelectRewardMissionProgressByMissionId(characterId, missionId, connection);
                if (progress == null || !progress.IsComplete || progress.IsReceived)
                {
                    Logger.Error($"Cannot claim reward for mission {missionId}: progress={progress?.CurrentCount}, complete={progress?.IsComplete}, received={progress?.IsReceived}");
                    return;
                }

                // Mark as received
                progress.IsReceived = true;
                Server.Database.UpsertRewardMissionProgress(characterId, progress, connection);

                // Update missions completed count
                var state = Server.Database.SelectRewardMissionState(characterId, connection);
                state.MissionsCompleted++;
                Server.Database.UpsertRewardMissionState(characterId, state, connection);

                // Grant rewards
                S2CItemUpdateCharacterItemNtc itemNtc = new();

                foreach (var reward in mission.Rewards)
                {
                    if (reward.WalletType != WalletType.None && reward.WalletAmount > 0)
                    {
                        itemNtc.UpdateWalletList.Add(Server.WalletManager.AddToWallet(client.Character, reward.WalletType, reward.WalletAmount));
                    }

                    if (reward.ItemId > 0 && reward.Num > 0)
                    {
                        itemNtc.UpdateItemList.AddRange(Server.ItemManager.AddItem(Server, client.Character, StorageType.ItemPost, reward.ItemId, reward.Num));
                    }
                }

                if (itemNtc.UpdateWalletList.Any() || itemNtc.UpdateItemList.Any())
                {
                    client.Enqueue(itemNtc, queue);
                }
            });

            return queue;
        }

        /// <summary>
        /// Claim milestone reward
        /// </summary>
        public PacketQueue ClaimMilestoneReward(GameClient client, uint milestoneIndex, DbConnection? connectionIn = null)
        {
            PacketQueue queue = new();
            uint characterId = client.Character.CharacterId;

            var milestones = GetMilestoneRewards();
            if (milestoneIndex >= milestones.Count)
            {
                Logger.Error($"Milestone index {milestoneIndex} out of range");
                return queue;
            }

            var milestone = milestones[(int)milestoneIndex];

            Server.Database.ExecuteQuerySafe(connectionIn, connection =>
            {
                // Check if already received
                var existingMilestones = Server.Database.SelectRewardMissionMilestones(characterId, connection);
                var existing = existingMilestones.FirstOrDefault(m => m.MilestoneId == milestoneIndex);
                if (existing != null && existing.IsReceived)
                {
                    Logger.Error($"Milestone {milestoneIndex} already received");
                    return;
                }

                // Check if requirements are met
                var state = Server.Database.SelectRewardMissionState(characterId, connection);
                if (state.MissionsCompleted < milestone.RequiredMissionCount)
                {
                    Logger.Error($"Not enough missions completed for milestone {milestoneIndex}: {state.MissionsCompleted} < {milestone.RequiredMissionCount}");
                    return;
                }

                // Mark as received
                var milestoneRecord = new RewardMissionMilestone
                {
                    MilestoneId = milestoneIndex,
                    IsReceived = true
                };
                Server.Database.UpsertRewardMissionMilestone(characterId, milestoneRecord, connection);

                // Grant rewards
                S2CItemUpdateCharacterItemNtc itemNtc = new();

                foreach (var reward in milestone.Rewards)
                {
                    if (reward.WalletType != WalletType.None && reward.WalletAmount > 0)
                    {
                        itemNtc.UpdateWalletList.Add(Server.WalletManager.AddToWallet(client.Character, reward.WalletType, reward.WalletAmount));
                    }

                    if (reward.ItemId > 0 && reward.Num > 0)
                    {
                        itemNtc.UpdateItemList.AddRange(Server.ItemManager.AddItem(Server, client.Character, StorageType.ItemPost, reward.ItemId, reward.Num));
                    }
                }

                if (itemNtc.UpdateWalletList.Any() || itemNtc.UpdateItemList.Any())
                {
                    client.Enqueue(itemNtc, queue);
                }
            });

            return queue;
        }

        /// <summary>
        /// Get the count of missions completed today
        /// </summary>
        public uint GetMissionsCompletedCount(GameClient client, DbConnection? connectionIn = null)
        {
            var state = Server.Database.SelectRewardMissionState(client.Character.CharacterId, connectionIn);
            return state.MissionsCompleted;
        }

        /// <summary>
        /// Get claimed milestone indices
        /// </summary>
        public List<uint> GetClaimedMilestones(GameClient client, DbConnection? connectionIn = null)
        {
            var milestones = Server.Database.SelectRewardMissionMilestones(client.Character.CharacterId, connectionIn);
            return milestones.Where(m => m.IsReceived).Select(m => m.MilestoneId).ToList();
        }
    }
}
