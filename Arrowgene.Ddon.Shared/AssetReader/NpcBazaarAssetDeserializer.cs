using System.Text.Json;
using Arrowgene.Logging;
using Arrowgene.Ddon.Shared.Asset;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class NpcBazaarAssetDeserializer : IAssetDeserializer<NpcBazaarAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(NpcBazaarAssetDeserializer));

        public NpcBazaarAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            NpcBazaarAsset asset = new NpcBazaarAsset();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            if (document.RootElement.TryGetProperty("excluded_item_ids", out JsonElement excludedItemIds))
            {
                foreach (var itemId in excludedItemIds.EnumerateArray())
                {
                    asset.ExcludedItemIds.Add(itemId.GetUInt32());
                }
            }

            if (document.RootElement.TryGetProperty("excluded_categories", out JsonElement excludedCategories))
            {
                asset.ExcludedCategories.Clear();
                foreach (var category in excludedCategories.EnumerateArray())
                {
                    asset.ExcludedCategories.Add(category.GetByte());
                }
            }

            if (document.RootElement.TryGetProperty("item_overrides", out JsonElement itemOverrides))
            {
                foreach (var property in itemOverrides.EnumerateObject())
                {
                    if (!uint.TryParse(property.Name, out uint itemId))
                    {
                        Logger.Error($"Invalid item ID '{property.Name}' in item_overrides.");
                        continue;
                    }

                    var overrideObj = new NpcBazaarItemOverride();

                    if (property.Value.TryGetProperty("buy_multiplier", out JsonElement buyMultiplier))
                    {
                        overrideObj.BuyMultiplier = buyMultiplier.GetDouble();
                    }

                    if (property.Value.TryGetProperty("sell_multiplier", out JsonElement sellMultiplier))
                    {
                        overrideObj.SellMultiplier = sellMultiplier.GetDouble();
                    }

                    asset.ItemOverrides[itemId] = overrideObj;
                }
            }

            return asset;
        }
    }
}
