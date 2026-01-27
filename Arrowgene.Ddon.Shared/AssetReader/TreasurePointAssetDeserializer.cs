using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class TreasurePointAssetDeserializer : IAssetDeserializer<TreasurePointAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(TreasurePointAssetDeserializer));

        public TreasurePointAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            TreasurePointAsset asset = new TreasurePointAsset();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            // Parse categories
            var categories = document.RootElement.GetProperty("categories").EnumerateArray().ToList();
            foreach (var category in categories)
            {
                uint categoryId = category.GetProperty("category_id").GetUInt32();
                string categoryName = category.GetProperty("category_name").GetString() ?? string.Empty;

                // Parse points within this category
                List<CDataTreasurePoint> points = new List<CDataTreasurePoint>();
                if (category.TryGetProperty("points", out var pointsArray))
                {
                    foreach (var point in pointsArray.EnumerateArray())
                    {
                        points.Add(new CDataTreasurePoint()
                        {
                            Index = point.GetProperty("index").GetUInt32(),
                            Name = point.GetProperty("name").GetString() ?? string.Empty,
                            Unk2 = point.TryGetProperty("discovered", out var discovered) && discovered.GetBoolean()
                        });
                    }
                }

                asset.Categories.Add(new CDataTreasurePointCategory()
                {
                    CategoryId = categoryId,
                    CategoryName = categoryName,
                    EntryCount = (byte)points.Count
                });

                asset.Points[categoryId] = points;
            }

            Logger.Info($"Loaded {asset.Categories.Count} treasure point categories with {asset.Points.Values.Sum(p => p.Count)} total points");

            return asset;
        }
    }
}
