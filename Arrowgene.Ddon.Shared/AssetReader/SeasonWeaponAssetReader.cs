using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Logging;
using System.Text.Json;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class SeasonWeaponAssetReader : IAssetDeserializer<SeasonWeaponAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(SeasonWeaponAssetReader));

        public SeasonWeaponAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            SeasonWeaponAsset asset = new();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            if (document.RootElement.TryGetProperty("weapons", out JsonElement jWeapons))
            {
                foreach (var jWeapon in jWeapons.EnumerateArray())
                {
                    SeasonWeaponEntry entry = new();

                    if (jWeapon.TryGetProperty("item_id", out JsonElement jItemId))
                    {
                        entry.ItemId = jItemId.GetUInt32();
                    }

                    if (jWeapon.TryGetProperty("season", out JsonElement jSeason))
                    {
                        entry.Season = jSeason.GetUInt32();
                    }

                    if (jWeapon.TryGetProperty("comment", out JsonElement jComment))
                    {
                        entry.Comment = jComment.GetString() ?? string.Empty;
                    }

                    asset.Weapons.Add(entry);
                }
            }

            Logger.Info($"Loaded {asset.Weapons.Count} season weapons");

            return asset;
        }
    }
}
