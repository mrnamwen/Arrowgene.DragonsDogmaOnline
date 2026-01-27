using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class OfficialPawnDeserializer : IAssetDeserializer<List<OfficialPawn>>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(OfficialPawnDeserializer));

        public List<OfficialPawn> ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            List<OfficialPawn> asset = new();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            var pawnElements = document.RootElement.EnumerateArray().ToList();
            foreach (var pawn in pawnElements)
            {
                var officialPawn = new OfficialPawn
                {
                    PawnId = pawn.GetProperty("PawnId").GetUInt32(),
                    Name = pawn.GetProperty("Name").GetString() ?? string.Empty,
                    Job = (JobId)pawn.GetProperty("Job").GetByte(),
                    Level = pawn.GetProperty("Level").GetUInt32(),
                    HmType = pawn.GetProperty("HmType").GetByte(),
                    PawnType = (PawnType)pawn.GetProperty("PawnType").GetByte(),
                    CraftRank = pawn.TryGetProperty("CraftRank", out var craftRank) ? craftRank.GetUInt32() : 1,
                    HideEquipHead = pawn.TryGetProperty("HideEquipHead", out var hideHead) && hideHead.GetBoolean(),
                    HideEquipLantern = pawn.TryGetProperty("HideEquipLantern", out var hideLantern) && hideLantern.GetBoolean()
                };

                // Parse EditInfo
                if (pawn.TryGetProperty("EditInfo", out var editInfo))
                {
                    officialPawn.EditInfo = ParseEditInfo(editInfo);
                }

                // Parse Equipment
                if (pawn.TryGetProperty("Equipment", out var equipment))
                {
                    officialPawn.Equipment = ParseEquipment(equipment);
                }

                // Parse Visual Equipment
                if (pawn.TryGetProperty("VisualEquipment", out var visualEquipment))
                {
                    officialPawn.VisualEquipment = ParseEquipment(visualEquipment);
                }

                // Parse Job Items
                if (pawn.TryGetProperty("JobItems", out var jobItems))
                {
                    foreach (var item in jobItems.EnumerateArray())
                    {
                        officialPawn.JobItems.Add(item.GetUInt32());
                    }
                }

                // Parse Custom Skills
                if (pawn.TryGetProperty("CustomSkills", out var customSkills))
                {
                    foreach (var skill in customSkills.EnumerateArray())
                    {
                        officialPawn.CustomSkills.Add(new OfficialPawnSkill
                        {
                            SlotNo = skill.GetProperty("SlotNo").GetByte(),
                            SkillId = skill.GetProperty("SkillId").GetUInt32(),
                            SkillLv = skill.GetProperty("SkillLv").GetByte()
                        });
                    }
                }

                // Parse Abilities
                if (pawn.TryGetProperty("Abilities", out var abilities))
                {
                    foreach (var ability in abilities.EnumerateArray())
                    {
                        officialPawn.Abilities.Add(new OfficialPawnAbility
                        {
                            SlotNo = ability.GetProperty("SlotNo").GetByte(),
                            Job = (JobId)ability.GetProperty("Job").GetByte(),
                            AbilityId = ability.GetProperty("AbilityId").GetUInt32(),
                            AbilityLv = ability.GetProperty("AbilityLv").GetByte()
                        });
                    }
                }

                // Parse Pawn Reactions
                if (pawn.TryGetProperty("PawnReactions", out var reactions))
                {
                    foreach (var reaction in reactions.EnumerateArray())
                    {
                        officialPawn.PawnReactions.Add(new CDataPawnReaction
                        {
                            ReactionType = reaction.GetProperty("ReactionType").GetByte(),
                            MotionNo = reaction.GetProperty("MotionNo").GetUInt32()
                        });
                    }
                }

                // Parse SP Skills
                if (pawn.TryGetProperty("SpSkills", out var spSkills))
                {
                    foreach (var spSkill in spSkills.EnumerateArray())
                    {
                        officialPawn.SpSkills.Add(new CDataSpSkill
                        {
                            SpSkillId = spSkill.GetProperty("SpSkillId").GetByte(),
                            SpSkillLv = spSkill.GetProperty("SpSkillLv").GetByte()
                        });
                    }
                }

                // Parse Craft Skills
                if (pawn.TryGetProperty("CraftSkills", out var craftSkills))
                {
                    foreach (var craftSkill in craftSkills.EnumerateArray())
                    {
                        officialPawn.CraftSkills.Add(new CDataPawnCraftSkill
                        {
                            Type = (CraftSkillType)craftSkill.GetProperty("Type").GetByte(),
                            Level = craftSkill.GetProperty("Level").GetUInt32()
                        });
                    }
                }

                asset.Add(officialPawn);
            }

            return asset;
        }

        private CDataEditInfo ParseEditInfo(JsonElement editInfo)
        {
            var info = new CDataEditInfo();

            if (editInfo.TryGetProperty("Sex", out var sex)) info.Sex = sex.GetByte();
            if (editInfo.TryGetProperty("Voice", out var voice)) info.Voice = voice.GetByte();
            if (editInfo.TryGetProperty("VoicePitch", out var voicePitch)) info.VoicePitch = voicePitch.GetUInt16();
            if (editInfo.TryGetProperty("Personality", out var personality)) info.Personality = (PawnPersonality)personality.GetByte();
            if (editInfo.TryGetProperty("SpeechFreq", out var speechFreq)) info.SpeechFreq = speechFreq.GetByte();
            if (editInfo.TryGetProperty("BodyType", out var bodyType)) info.BodyType = bodyType.GetByte();
            if (editInfo.TryGetProperty("Hair", out var hair)) info.Hair = hair.GetByte();
            if (editInfo.TryGetProperty("Beard", out var beard)) info.Beard = beard.GetByte();
            if (editInfo.TryGetProperty("Makeup", out var makeup)) info.Makeup = makeup.GetByte();
            if (editInfo.TryGetProperty("Scar", out var scar)) info.Scar = scar.GetByte();
            if (editInfo.TryGetProperty("EyePresetNo", out var eyePreset)) info.EyePresetNo = eyePreset.GetByte();
            if (editInfo.TryGetProperty("NosePresetNo", out var nosePreset)) info.NosePresetNo = nosePreset.GetByte();
            if (editInfo.TryGetProperty("MouthPresetNo", out var mouthPreset)) info.MouthPresetNo = mouthPreset.GetByte();
            if (editInfo.TryGetProperty("EyebrowTexNo", out var eyebrowTex)) info.EyebrowTexNo = eyebrowTex.GetByte();
            if (editInfo.TryGetProperty("ColorSkin", out var colorSkin)) info.ColorSkin = colorSkin.GetByte();
            if (editInfo.TryGetProperty("ColorHair", out var colorHair)) info.ColorHair = colorHair.GetByte();
            if (editInfo.TryGetProperty("ColorBeard", out var colorBeard)) info.ColorBeard = colorBeard.GetByte();
            if (editInfo.TryGetProperty("ColorEyebrow", out var colorEyebrow)) info.ColorEyebrow = colorEyebrow.GetByte();
            if (editInfo.TryGetProperty("ColorREye", out var colorREye)) info.ColorREye = colorREye.GetByte();
            if (editInfo.TryGetProperty("ColorLEye", out var colorLEye)) info.ColorLEye = colorLEye.GetByte();
            if (editInfo.TryGetProperty("ColorMakeup", out var colorMakeup)) info.ColorMakeup = colorMakeup.GetByte();
            if (editInfo.TryGetProperty("Height", out var height)) info.Height = height.GetUInt16();
            if (editInfo.TryGetProperty("HeadSize", out var headSize)) info.HeadSize = headSize.GetUInt16();

            return info;
        }

        private Dictionary<byte, uint> ParseEquipment(JsonElement equipment)
        {
            var result = new Dictionary<byte, uint>();

            // Slot numbers based on EquipSlot enum: WepMain=1, WepSub=2, ArmorHelm=3, etc.
            if (equipment.TryGetProperty("Primary", out var primary)) result[1] = primary.GetUInt32();
            if (equipment.TryGetProperty("Secondary", out var secondary)) result[2] = secondary.GetUInt32();
            if (equipment.TryGetProperty("Head", out var head)) result[3] = head.GetUInt32();
            if (equipment.TryGetProperty("Body", out var body)) result[4] = body.GetUInt32();
            if (equipment.TryGetProperty("BodyClothing", out var bodyClothing)) result[5] = bodyClothing.GetUInt32();
            if (equipment.TryGetProperty("Arm", out var arm)) result[6] = arm.GetUInt32();
            if (equipment.TryGetProperty("Leg", out var leg)) result[7] = leg.GetUInt32();
            if (equipment.TryGetProperty("LegWear", out var legWear)) result[8] = legWear.GetUInt32();
            if (equipment.TryGetProperty("OverWear", out var overWear)) result[9] = overWear.GetUInt32();
            if (equipment.TryGetProperty("Jewelry1", out var jewelry1)) result[10] = jewelry1.GetUInt32();
            if (equipment.TryGetProperty("Jewelry2", out var jewelry2)) result[11] = jewelry2.GetUInt32();
            if (equipment.TryGetProperty("Jewelry3", out var jewelry3)) result[12] = jewelry3.GetUInt32();
            if (equipment.TryGetProperty("Jewelry4", out var jewelry4)) result[13] = jewelry4.GetUInt32();
            if (equipment.TryGetProperty("Jewelry5", out var jewelry5)) result[14] = jewelry5.GetUInt32();
            if (equipment.TryGetProperty("Lantern", out var lantern)) result[15] = lantern.GetUInt32();

            return result;
        }
    }
}
