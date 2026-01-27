using System.Linq;
using System.Text.Json;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model.Quest;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class QuestBonusAssetDeserializer : IAssetDeserializer<QuestBonusAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(QuestBonusAssetDeserializer));

        public QuestBonusAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            QuestBonusAsset asset = new QuestBonusAsset();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            // Parse area bonuses
            var areaBonuses = document.RootElement.GetProperty("area_bonuses").EnumerateArray().ToList();
            foreach (var areaBonus in areaBonuses)
            {
                var areaId = (QuestAreaId)areaBonus.GetProperty("area_id").GetUInt32();
                var bonus = new CDataAreaBonus()
                {
                    AreaID = areaId,
                    Unk0 = areaBonus.GetProperty("gold_ratio").GetUInt16(),
                    Unk1 = areaBonus.GetProperty("exp_ratio").GetUInt16(),
                    Unk2 = areaBonus.GetProperty("rim_ratio").GetUInt16(),
                    Unk3 = areaBonus.GetProperty("area_point_ratio").GetUInt16(),
                    Unk4 = areaBonus.TryGetProperty("unk4", out var unk4) ? unk4.GetUInt16() : (ushort)0
                };

                asset.AreaBonuses[areaId] = bonus;
            }

            // Parse level bonuses
            var levelBonuses = document.RootElement.GetProperty("level_bonuses").EnumerateArray().ToList();
            foreach (var levelBonus in levelBonuses)
            {
                var bonus = new CDataLevelBonus()
                {
                    Unk0 = levelBonus.GetProperty("category").GetUInt32()
                };

                var elements = levelBonus.GetProperty("elements").EnumerateArray().ToList();
                foreach (var element in elements)
                {
                    bonus.BonusList.Add(new CDataLevelBonusElement()
                    {
                        MinLevel = element.GetProperty("min_level").GetUInt32(),
                        MaxLevel = element.GetProperty("max_level").GetUInt32(),
                        GoldRatio = element.GetProperty("gold_ratio").GetUInt16(),
                        ExpRatio = element.GetProperty("exp_ratio").GetUInt16(),
                        RimRatio = element.GetProperty("rim_ratio").GetUInt16(),
                        AreaPointRatio = element.GetProperty("area_point_ratio").GetUInt16()
                    });
                }

                asset.LevelBonuses.Add(bonus);
            }

            // Parse party bonuses
            if (document.RootElement.TryGetProperty("party_bonuses", out var partyBonuses))
            {
                foreach (var partyBonus in partyBonuses.EnumerateArray())
                {
                    var bonus = new CDataSetQuestBonusList()
                    {
                        Unk0 = partyBonus.GetProperty("unk0").GetUInt32()
                    };
                    // QuestInfoList would need more parsing if we have data for it
                    asset.PartyBonuses.Add(bonus);
                }
            }

            // Parse quest party bonuses
            if (document.RootElement.TryGetProperty("quest_party_bonuses", out var questPartyBonuses))
            {
                foreach (var questPartyBonus in questPartyBonuses.EnumerateArray())
                {
                    var bonus = new CDataQuestPartyBonusInfo()
                    {
                        QuestScheduleId = questPartyBonus.GetProperty("quest_schedule_id").GetUInt32(),
                        QuestId = questPartyBonus.GetProperty("quest_id").GetUInt32(),
                        GoldRatio = questPartyBonus.GetProperty("gold_ratio").GetUInt16(),
                        ExpRatio = questPartyBonus.GetProperty("exp_ratio").GetUInt16(),
                        RimRatio = questPartyBonus.GetProperty("rim_ratio").GetUInt16(),
                        AreaPointRatio = questPartyBonus.GetProperty("area_point_ratio").GetUInt16(),
                        Dorb = questPartyBonus.TryGetProperty("dorb", out var dorb) ? dorb.GetUInt32() : 0,
                        IsReceived = questPartyBonus.TryGetProperty("is_received", out var isReceived) && isReceived.GetBoolean()
                    };
                    asset.QuestPartyBonuses.Add(bonus);
                }
            }

            Logger.Info($"Loaded {asset.AreaBonuses.Count} area bonuses, {asset.LevelBonuses.Count} level bonus categories");

            return asset;
        }
    }
}
