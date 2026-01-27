using System;
using System.Linq;
using System.Text.Json;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class InfinityDeliveryAssetDeserializer : IAssetDeserializer<InfinityDeliveryAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(InfinityDeliveryAssetDeserializer));

        public InfinityDeliveryAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            InfinityDeliveryAsset asset = new InfinityDeliveryAsset();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            asset.Enabled = document.RootElement.TryGetProperty("enabled", out var enabled) && enabled.GetBoolean();
            asset.EventId = document.RootElement.TryGetProperty("event_id", out var eventId) ? eventId.GetUInt32() : 1;

            if (document.RootElement.TryGetProperty("reset_day", out var resetDay))
            {
                asset.ResetDay = Enum.Parse<DayOfWeek>(resetDay.GetString() ?? "Monday", true);
            }
            else
            {
                asset.ResetDay = DayOfWeek.Monday;
            }

            asset.ResetHour = document.RootElement.TryGetProperty("reset_hour", out var resetHour) ? resetHour.GetInt32() : 5;

            if (document.RootElement.TryGetProperty("categories", out var categoriesElement))
            {
                foreach (var categoryElement in categoriesElement.EnumerateArray())
                {
                    var category = new InfinityDeliveryCategory
                    {
                        CategoryId = categoryElement.GetProperty("category_id").GetUInt32(),
                        Name = categoryElement.TryGetProperty("name", out var name) ? name.GetString() ?? string.Empty : string.Empty
                    };

                    if (categoryElement.TryGetProperty("items", out var itemsElement))
                    {
                        foreach (var itemElement in itemsElement.EnumerateArray())
                        {
                            var item = new InfinityDeliveryItemEntry
                            {
                                ItemId = itemElement.GetProperty("item_id").GetUInt32(),
                                PointValue = itemElement.GetProperty("point_value").GetUInt32()
                            };
                            category.Items.Add(item);
                        }
                    }

                    asset.Categories.Add(category);
                }
            }

            Logger.Info($"Loaded {asset.Categories.Count} infinity delivery categories");

            if (document.RootElement.TryGetProperty("borders", out var bordersElement))
            {
                foreach (var borderElement in bordersElement.EnumerateArray())
                {
                    var border = new InfinityDeliveryBorderReward
                    {
                        BorderId = borderElement.GetProperty("border_id").GetUInt32(),
                        RequiredPoints = borderElement.GetProperty("required_points").GetUInt32()
                    };

                    if (borderElement.TryGetProperty("rewards", out var rewardsElement))
                    {
                        foreach (var rewardElement in rewardsElement.EnumerateArray())
                        {
                            var reward = new InfinityDeliveryReward
                            {
                                ItemId = rewardElement.TryGetProperty("item_id", out var itemId) ? itemId.GetUInt32() : 0,
                                Num = rewardElement.TryGetProperty("num", out var num) ? num.GetUInt32() : 0,
                                WalletType = rewardElement.TryGetProperty("wallet_type", out var walletType) ? (WalletType)walletType.GetByte() : WalletType.None,
                                WalletAmount = rewardElement.TryGetProperty("wallet_amount", out var walletAmount) ? walletAmount.GetUInt32() : 0
                            };
                            border.Rewards.Add(reward);
                        }
                    }

                    asset.Borders.Add(border);
                }
            }

            Logger.Info($"Loaded {asset.Borders.Count} infinity delivery borders");

            return asset;
        }
    }
}
