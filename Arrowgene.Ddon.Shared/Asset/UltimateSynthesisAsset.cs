using System.Collections.Generic;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class UltimateSynthesisAsset
    {
        public UltimateSynthesisAsset()
        {
            ListTitle = string.Empty;
            Categories = new List<UltimateSynthesisCategory>();
            SpecialBonus = new UltimateSynthesisSpecialBonus();
        }

        public string ListTitle { get; set; }
        public List<UltimateSynthesisCategory> Categories { get; set; }

        /// <summary>
        /// Special Bonus configuration - triggered when possessing previous season's weapon at ★4.
        /// </summary>
        public UltimateSynthesisSpecialBonus SpecialBonus { get; set; }
    }

    /// <summary>
    /// Special Bonus configuration for Ultimate Synthesis.
    /// Triggered when the player possesses the previous season's strongest weapon at ★4 enhancement.
    /// </summary>
    public class UltimateSynthesisSpecialBonus
    {
        public UltimateSynthesisSpecialBonus()
        {
            StatLottery = new List<UltimateSynthesisStatLottery>();
        }

        /// <summary>
        /// Whether the Special Bonus system is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Stat lottery options for the special bonus (Physical/Magick Attack +1-20).
        /// </summary>
        public List<UltimateSynthesisStatLottery> StatLottery { get; set; }
    }

    public class UltimateSynthesisRecipe
    {
        public UltimateSynthesisRecipe()
        {
            MaterialItemIds = new List<uint>();
        }

        /// <summary>
        /// The source item ID that can be evolved.
        /// </summary>
        public uint SourceItemId { get; set; }

        /// <summary>
        /// The resulting item ID after evolution.
        /// </summary>
        public uint ResultItemId { get; set; }

        /// <summary>
        /// Required material item IDs for the synthesis.
        /// </summary>
        public List<uint> MaterialItemIds { get; set; }

        /// <summary>
        /// Gold cost for the synthesis.
        /// </summary>
        public uint GoldCost { get; set; }
    }

    public class UltimateSynthesisStatLottery
    {
        public UltimateSynthesisStatLottery()
        {
            Rolls = new List<ushort>();
        }

        public string Name { get; set; } = string.Empty;
        public uint MinGreatSuccessIndex { get; set; }
        public List<ushort> Rolls { get; set; }
    }

    /// <summary>
    /// Payment option for Ultimate Synthesis with first-attempt tracking.
    /// </summary>
    public class UltimateSynthesisPaymentOption
    {
        public WalletType WalletType { get; set; }
        public string Label { get; set; } = string.Empty;
        public uint Cost { get; set; }

        /// <summary>
        /// If true, this payment option is only available for the first attempt.
        /// Subsequent attempts must use a different payment option (typically premium currency).
        /// </summary>
        public bool IsFirstAttemptOnly { get; set; }
    }

    public class UltimateSynthesisCategory
    {
        public UltimateSynthesisCategory()
        {
            ShopListings = new List<byte>();
            PaymentOptions = new List<UltimateSynthesisPaymentOption>();
            Recipes = new List<UltimateSynthesisRecipe>();
            StatLottery = new List<UltimateSynthesisStatLottery>();
        }

        /// <summary>
        /// Category key identifier.
        /// </summary>
        public ushort Key { get; set; }

        /// <summary>
        /// Display index for the category.
        /// </summary>
        public byte Index { get; set; }

        /// <summary>
        /// Shop listing IDs where this category applies.
        /// </summary>
        public List<byte> ShopListings { get; set; }

        /// <summary>
        /// Payment types that guarantee great success.
        /// </summary>
        public HashSet<WalletType> PremiumCurrencies { get; set; } = new();

        /// <summary>
        /// Available payment options for this category.
        /// </summary>
        public List<UltimateSynthesisPaymentOption> PaymentOptions { get; set; }

        /// <summary>
        /// Available synthesis recipes for this category.
        /// </summary>
        public List<UltimateSynthesisRecipe> Recipes { get; set; }

        /// <summary>
        /// Stat lottery options for this category (Fusion Bonuses).
        /// </summary>
        public List<UltimateSynthesisStatLottery> StatLottery { get; set; }
    }
}
