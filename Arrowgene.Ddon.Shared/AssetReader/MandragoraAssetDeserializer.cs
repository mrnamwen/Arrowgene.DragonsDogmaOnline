using System.Linq;
using System.Text.Json;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class MandragoraAssetDeserializer : IAssetDeserializer<MandragoraAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(MandragoraAssetDeserializer));

        public MandragoraAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            MandragoraAsset asset = new MandragoraAsset();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            // Parse species definitions
            if (document.RootElement.TryGetProperty("species", out var speciesArray))
            {
                foreach (var species in speciesArray.EnumerateArray())
                {
                    var def = new MandragoraSpeciesDefinition
                    {
                        Index = species.GetProperty("index").GetUInt32(),
                        Category = (MandragoraSpeciesCategory)species.GetProperty("category").GetByte(),
                        Rarity = (MandragoraRarity)species.GetProperty("rarity").GetByte()
                    };
                    asset.Species.Add(def);
                }
            }

            // Parse craft recipes
            if (document.RootElement.TryGetProperty("craft_recipes", out var recipesArray))
            {
                foreach (var recipe in recipesArray.EnumerateArray())
                {
                    var def = new MandragoraCraftRecipeDefinition
                    {
                        RecipeId = recipe.GetProperty("recipe_id").GetUInt32(),
                        CategoryId = recipe.GetProperty("category_id").GetUInt32(),
                        ResultItemId = recipe.GetProperty("result_item_id").GetUInt32(),
                        CraftTime = recipe.GetProperty("craft_time").GetUInt32(),
                        Cost = recipe.TryGetProperty("cost", out var cost) ? cost.GetUInt32() : 0,
                        IsLocked = recipe.TryGetProperty("is_locked", out var locked) && locked.GetBoolean()
                    };

                    if (recipe.TryGetProperty("materials", out var materialsArray))
                    {
                        uint sortNo = 1;
                        foreach (var material in materialsArray.EnumerateArray())
                        {
                            def.Materials.Add(new MandragoraCraftMaterialDefinition
                            {
                                ItemId = material.GetProperty("item_id").GetUInt32(),
                                Quantity = material.GetProperty("quantity").GetUInt32(),
                                SortNo = material.TryGetProperty("sort_no", out var sort) ? sort.GetUInt32() : sortNo,
                                IsSpecial = material.TryGetProperty("is_special", out var isSp) && isSp.GetBoolean()
                            });
                            sortNo++;
                        }
                    }

                    asset.CraftRecipes.Add(def);
                }
            }

            // Parse craft categories
            if (document.RootElement.TryGetProperty("craft_categories", out var categoriesArray))
            {
                foreach (var category in categoriesArray.EnumerateArray())
                {
                    asset.CraftCategories.Add(new MandragoraCraftCategoryDefinition
                    {
                        CategoryId = (byte)category.GetProperty("category_id").GetUInt32(),
                        CategoryName = category.GetProperty("category_name").GetString() ?? string.Empty
                    });
                }
            }

            // Parse fertilizer items
            if (document.RootElement.TryGetProperty("fertilizer_items", out var fertilizerArray))
            {
                foreach (var fertilizer in fertilizerArray.EnumerateArray())
                {
                    asset.FertilizerItems.Add(new MandragoraFertilizerDefinition
                    {
                        ItemId = fertilizer.GetProperty("item_id").GetUInt32(),
                        MaxQuantity = fertilizer.TryGetProperty("max_quantity", out var max) ? max.GetUInt32() : 99
                    });
                }
            }

            // Parse furniture items
            if (document.RootElement.TryGetProperty("furniture_items", out var furnitureArray))
            {
                foreach (var furniture in furnitureArray.EnumerateArray())
                {
                    asset.FurnitureItems.Add(new MandragoraFurnitureDefinition
                    {
                        MandragoraId = furniture.GetProperty("mandragora_id").GetUInt32(),
                        ItemId = furniture.GetProperty("item_id").GetUInt32()
                    });
                }
            }

            // Parse cultivation material max
            if (document.RootElement.TryGetProperty("cultivation_material_max", out var cultivationMax))
            {
                asset.CultivationMaterialMax = cultivationMax.GetUInt32();
            }

            Logger.Info($"Loaded {asset.Species.Count} species, {asset.CraftRecipes.Count} recipes, {asset.CraftCategories.Count} categories");

            return asset;
        }
    }
}
