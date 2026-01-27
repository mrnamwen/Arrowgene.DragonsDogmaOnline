using System.Collections.Generic;
using Arrowgene.Ddon.Shared.Entity.Structure;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class GpShopAsset
    {
        public Dictionary<uint, CDataGPShopLineupItem> LineupItems { get; set; }
        public Dictionary<uint, List<uint>> CategoryLineups { get; set; }

        public GpShopAsset()
        {
            LineupItems = new Dictionary<uint, CDataGPShopLineupItem>();
            CategoryLineups = new Dictionary<uint, List<uint>>();
        }

        public List<CDataGPShopLineupItem> GetItemsForCategory(uint categoryId)
        {
            var result = new List<CDataGPShopLineupItem>();
            if (CategoryLineups.TryGetValue(categoryId, out var lineupIds))
            {
                foreach (var lineupId in lineupIds)
                {
                    if (LineupItems.TryGetValue(lineupId, out var item))
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        public CDataGPShopLineupItem GetItem(uint lineupId)
        {
            LineupItems.TryGetValue(lineupId, out var item);
            return item;
        }
    }
}
