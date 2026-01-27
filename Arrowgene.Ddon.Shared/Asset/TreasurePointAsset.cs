using Arrowgene.Ddon.Shared.Entity.Structure;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class TreasurePointAsset
    {
        public TreasurePointAsset()
        {
            Categories = new List<CDataTreasurePointCategory>();
            Points = new Dictionary<uint, List<CDataTreasurePoint>>();
        }

        /// <summary>
        /// List of treasure point categories (e.g., "Trades of an Explorer" lore entries)
        /// </summary>
        public List<CDataTreasurePointCategory> Categories { get; set; }

        /// <summary>
        /// Dictionary mapping category ID to treasure points within that category
        /// </summary>
        public Dictionary<uint, List<CDataTreasurePoint>> Points { get; set; }
    }
}
