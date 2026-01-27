using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model.Quest;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class QuestBonusAsset
    {
        public QuestBonusAsset()
        {
            AreaBonuses = new Dictionary<QuestAreaId, CDataAreaBonus>();
            LevelBonuses = new List<CDataLevelBonus>();
            PartyBonuses = new List<CDataSetQuestBonusList>();
            QuestPartyBonuses = new List<CDataQuestPartyBonusInfo>();
        }

        public Dictionary<QuestAreaId, CDataAreaBonus> AreaBonuses { get; set; }
        public List<CDataLevelBonus> LevelBonuses { get; set; }
        public List<CDataSetQuestBonusList> PartyBonuses { get; set; }
        public List<CDataQuestPartyBonusInfo> QuestPartyBonuses { get; set; }
    }
}
