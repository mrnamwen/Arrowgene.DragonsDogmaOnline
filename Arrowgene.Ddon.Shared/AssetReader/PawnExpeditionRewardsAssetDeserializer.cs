using System;
using System.Linq;
using System.Text.Json;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class PawnExpeditionRewardsAssetDeserializer : IAssetDeserializer<PawnExpeditionRewardsAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(PawnExpeditionRewardsAssetDeserializer));

        public PawnExpeditionRewardsAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            PawnExpeditionRewardsAsset asset = new PawnExpeditionRewardsAsset();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            foreach (var areaElement in document.RootElement.EnumerateArray())
            {
                var areaIdStr = areaElement.GetProperty("AreaId").GetString();
                if (!Enum.TryParse<QuestAreaId>(areaIdStr, out var areaId))
                {
                    Logger.Error($"Unknown area ID: {areaIdStr}");
                    continue;
                }

                var areaRewards = new PawnExpeditionAreaRewards
                {
                    AreaId = areaId
                };

                // Parse common rewards
                if (areaElement.TryGetProperty("CommonRewards", out var commonRewards))
                {
                    foreach (var rewardElement in commonRewards.EnumerateArray())
                    {
                        areaRewards.CommonRewards.Add(ParseRewardItem(rewardElement));
                    }
                }

                // Parse rare rewards
                if (areaElement.TryGetProperty("RareRewards", out var rareRewards))
                {
                    foreach (var rewardElement in rareRewards.EnumerateArray())
                    {
                        areaRewards.RareRewards.Add(ParseRewardItem(rewardElement));
                    }
                }

                // Parse hot spot rewards
                if (areaElement.TryGetProperty("HotSpotRewards", out var hotSpotRewards))
                {
                    foreach (var rewardElement in hotSpotRewards.EnumerateArray())
                    {
                        areaRewards.HotSpotRewards.Add(ParseRewardItem(rewardElement));
                    }
                }

                asset.AreaRewards[areaId] = areaRewards;
            }

            Logger.Info($"Loaded expedition rewards for {asset.AreaRewards.Count} areas");

            return asset;
        }

        private PawnExpeditionRewardItem ParseRewardItem(JsonElement element)
        {
            return new PawnExpeditionRewardItem
            {
                ItemId = element.GetProperty("ItemId").GetUInt32(),
                MinNum = element.GetProperty("MinNum").GetUInt32(),
                MaxNum = element.GetProperty("MaxNum").GetUInt32()
            };
        }
    }
}
