using System.Linq;
using System.Text.Json;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class DailyMissionAssetDeserializer : IAssetDeserializer<DailyMissionAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(DailyMissionAssetDeserializer));

        public DailyMissionAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            DailyMissionAsset asset = new DailyMissionAsset();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            var missions = document.RootElement.GetProperty("missions").EnumerateArray().ToList();
            foreach (var missionElement in missions)
            {
                var mission = new DailyMission
                {
                    MissionId = missionElement.GetProperty("mission_id").GetUInt32(),
                    Category = (DailyMissionCategory)missionElement.GetProperty("category").GetUInt32(),
                    Type = (DailyMissionType)missionElement.GetProperty("type").GetUInt32(),
                    TargetCount = missionElement.GetProperty("target_count").GetUInt32(),
                    SortId = missionElement.GetProperty("sort_id").GetUInt32(),
                    Title = missionElement.GetProperty("title").GetString() ?? string.Empty,
                    IconUrl = missionElement.TryGetProperty("icon_url", out var iconUrl) ? iconUrl.GetString() ?? string.Empty : string.Empty,
                    ImageUrl = missionElement.TryGetProperty("image_url", out var imageUrl) ? imageUrl.GetString() ?? string.Empty : string.Empty
                };

                if (missionElement.TryGetProperty("rewards", out var rewardsElement))
                {
                    foreach (var rewardElement in rewardsElement.EnumerateArray())
                    {
                        var reward = new DailyMissionReward
                        {
                            ItemId = rewardElement.TryGetProperty("item_id", out var itemId) ? itemId.GetUInt32() : 0,
                            Num = rewardElement.TryGetProperty("num", out var num) ? num.GetUInt32() : 0,
                            WalletType = rewardElement.TryGetProperty("wallet_type", out var walletType) ? (WalletType)walletType.GetByte() : WalletType.None,
                            WalletAmount = rewardElement.TryGetProperty("wallet_amount", out var walletAmount) ? walletAmount.GetUInt32() : 0
                        };
                        mission.Rewards.Add(reward);
                    }
                }

                asset.Missions.Add(mission);
            }

            Logger.Info($"Loaded {asset.Missions.Count} daily missions");

            // Parse milestone rewards if present
            if (document.RootElement.TryGetProperty("milestone_rewards", out var milestonesElement))
            {
                foreach (var milestoneElement in milestonesElement.EnumerateArray())
                {
                    var milestone = new MilestoneReward
                    {
                        RequiredMissionCount = milestoneElement.GetProperty("required_mission_count").GetUInt32(),
                        Category = milestoneElement.TryGetProperty("category", out var category)
                            ? (DailyMissionCategory)category.GetUInt32()
                            : DailyMissionCategory.Daily
                    };

                    if (milestoneElement.TryGetProperty("rewards", out var rewardsElement))
                    {
                        foreach (var rewardElement in rewardsElement.EnumerateArray())
                        {
                            var reward = new DailyMissionReward
                            {
                                ItemId = rewardElement.TryGetProperty("item_id", out var itemId) ? itemId.GetUInt32() : 0,
                                Num = rewardElement.TryGetProperty("num", out var num) ? num.GetUInt32() : 0,
                                WalletType = rewardElement.TryGetProperty("wallet_type", out var walletType) ? (WalletType)walletType.GetByte() : WalletType.None,
                                WalletAmount = rewardElement.TryGetProperty("wallet_amount", out var walletAmount) ? walletAmount.GetUInt32() : 0
                            };
                            milestone.Rewards.Add(reward);
                        }
                    }

                    asset.MilestoneRewards.Add(milestone);
                }
                Logger.Info($"Loaded {asset.MilestoneRewards.Count} milestone rewards");
            }

            return asset;
        }
    }
}
