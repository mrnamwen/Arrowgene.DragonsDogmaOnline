using Arrowgene.Ddon.Shared.Model;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class MandragoraAsset
    {
        public MandragoraAsset()
        {
            Species = new List<MandragoraSpeciesDefinition>();
            CraftRecipes = new List<MandragoraCraftRecipeDefinition>();
            CraftCategories = new List<MandragoraCraftCategoryDefinition>();
            FertilizerItems = new List<MandragoraFertilizerDefinition>();
            FurnitureItems = new List<MandragoraFurnitureDefinition>();
        }

        /// <summary>
        /// List of all mandragora species definitions.
        /// </summary>
        public List<MandragoraSpeciesDefinition> Species { get; set; }

        /// <summary>
        /// List of all craft recipes.
        /// </summary>
        public List<MandragoraCraftRecipeDefinition> CraftRecipes { get; set; }

        /// <summary>
        /// List of all craft categories.
        /// </summary>
        public List<MandragoraCraftCategoryDefinition> CraftCategories { get; set; }

        /// <summary>
        /// List of fertilizer items.
        /// </summary>
        public List<MandragoraFertilizerDefinition> FertilizerItems { get; set; }

        /// <summary>
        /// List of furniture items that can house mandragoras.
        /// </summary>
        public List<MandragoraFurnitureDefinition> FurnitureItems { get; set; }

        /// <summary>
        /// Max cultivation materials a character can hold.
        /// </summary>
        public uint CultivationMaterialMax { get; set; } = 20;
    }

    public class MandragoraSpeciesDefinition
    {
        public uint Index { get; set; }
        public MandragoraSpeciesCategory Category { get; set; }
        public MandragoraRarity Rarity { get; set; }
    }

    public class MandragoraCraftRecipeDefinition
    {
        public MandragoraCraftRecipeDefinition()
        {
            Materials = new List<MandragoraCraftMaterialDefinition>();
        }

        public uint RecipeId { get; set; }
        public uint CategoryId { get; set; }
        public uint ResultItemId { get; set; }
        public uint CraftTime { get; set; }
        public uint Cost { get; set; }
        public bool IsLocked { get; set; }
        public List<MandragoraCraftMaterialDefinition> Materials { get; set; }
    }

    public class MandragoraCraftMaterialDefinition
    {
        public uint ItemId { get; set; }
        public uint Quantity { get; set; }
        public uint SortNo { get; set; }
        public bool IsSpecial { get; set; }
    }

    public class MandragoraCraftCategoryDefinition
    {
        public byte CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }

    public class MandragoraFertilizerDefinition
    {
        public uint ItemId { get; set; }
        public uint MaxQuantity { get; set; }
    }

    public class MandragoraFurnitureDefinition
    {
        public uint MandragoraId { get; set; }
        public uint ItemId { get; set; }
    }
}
