using Arrowgene.Ddon.Shared.Model.Quest;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class PawnExpeditionRewardItem
    {
        public uint ItemId { get; set; }
        public uint MinNum { get; set; }
        public uint MaxNum { get; set; }
    }

    public class PawnExpeditionAreaRewards
    {
        public QuestAreaId AreaId { get; set; }
        public List<PawnExpeditionRewardItem> CommonRewards { get; set; }
        public List<PawnExpeditionRewardItem> RareRewards { get; set; }
        public List<PawnExpeditionRewardItem> HotSpotRewards { get; set; }

        public PawnExpeditionAreaRewards()
        {
            CommonRewards = new List<PawnExpeditionRewardItem>();
            RareRewards = new List<PawnExpeditionRewardItem>();
            HotSpotRewards = new List<PawnExpeditionRewardItem>();
        }
    }

    public class PawnExpeditionRewardsAsset
    {
        public Dictionary<QuestAreaId, PawnExpeditionAreaRewards> AreaRewards { get; set; }

        public PawnExpeditionRewardsAsset()
        {
            AreaRewards = new Dictionary<QuestAreaId, PawnExpeditionAreaRewards>();
        }

        public PawnExpeditionAreaRewards GetAreaRewards(QuestAreaId areaId)
        {
            if (AreaRewards.TryGetValue(areaId, out var rewards))
            {
                return rewards;
            }
            return null;
        }

        public PawnExpeditionAreaRewards GetAreaRewards(uint areaId)
        {
            return GetAreaRewards((QuestAreaId)areaId);
        }
    }
}
