using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class NpcBazaarAsset
    {
        public NpcBazaarAsset()
        {
            ExcludedItemIds = new HashSet<uint>();
            ExcludedCategories = new HashSet<byte> { 4 }; // Key items excluded by default
            ItemOverrides = new Dictionary<uint, NpcBazaarItemOverride>();
        }

        /// <summary>
        /// Specific item IDs to exclude from NPC Bazaar.
        /// </summary>
        public HashSet<uint> ExcludedItemIds { get; set; }

        /// <summary>
        /// Item categories to exclude from NPC Bazaar.
        /// Default: [4] (Key Items)
        /// </summary>
        public HashSet<byte> ExcludedCategories { get; set; }

        /// <summary>
        /// Per-item price multiplier overrides.
        /// </summary>
        public Dictionary<uint, NpcBazaarItemOverride> ItemOverrides { get; set; }
    }

    public class NpcBazaarItemOverride
    {
        /// <summary>
        /// Override multiplier for NPC sell price (price player pays to buy from NPC).
        /// If null, uses the global NpcBazaarBuyMultiplier setting.
        /// </summary>
        public double? BuyMultiplier { get; set; }

        /// <summary>
        /// Override multiplier for NPC buy price (price player receives when selling to NPC).
        /// If null, uses the global NpcBazaarSellMultiplier setting.
        /// </summary>
        public double? SellMultiplier { get; set; }
    }
}
