using Arrowgene.Ddon.Shared.Entity.Structure;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Model
{
    /// <summary>
    /// Represents an official pawn definition loaded from the OfficialPawns.json asset file.
    /// These are auto-generated pawns available in the pawn search system.
    /// </summary>
    public class OfficialPawn
    {
        public OfficialPawn()
        {
            Name = string.Empty;
            EditInfo = new CDataEditInfo();
            Equipment = new Dictionary<byte, uint>();
            VisualEquipment = new Dictionary<byte, uint>();
            JobItems = new List<uint>();
            CustomSkills = new List<OfficialPawnSkill>();
            Abilities = new List<OfficialPawnAbility>();
            PawnReactions = new List<CDataPawnReaction>();
            SpSkills = new List<CDataSpSkill>();
        }

        public uint PawnId { get; set; }
        public string Name { get; set; }
        public JobId Job { get; set; }
        public uint Level { get; set; }
        public byte HmType { get; set; }
        public PawnType PawnType { get; set; }

        public CDataEditInfo EditInfo { get; set; }

        /// <summary>
        /// Equipment slots mapped by slot number (1-15).
        /// </summary>
        public Dictionary<byte, uint> Equipment { get; set; }

        /// <summary>
        /// Visual equipment slots mapped by slot number (1-15).
        /// </summary>
        public Dictionary<byte, uint> VisualEquipment { get; set; }

        public List<uint> JobItems { get; set; }

        public List<OfficialPawnSkill> CustomSkills { get; set; }
        public List<OfficialPawnAbility> Abilities { get; set; }
        public List<CDataPawnReaction> PawnReactions { get; set; }
        public List<CDataSpSkill> SpSkills { get; set; }

        public uint CraftRank { get; set; }
        public List<CDataPawnCraftSkill> CraftSkills { get; set; } = new();

        public bool HideEquipHead { get; set; }
        public bool HideEquipLantern { get; set; }
    }

    public class OfficialPawnSkill
    {
        public byte SlotNo { get; set; }
        public uint SkillId { get; set; }
        public byte SkillLv { get; set; }
    }

    public class OfficialPawnAbility
    {
        public byte SlotNo { get; set; }
        public JobId Job { get; set; }
        public uint AbilityId { get; set; }
        public byte AbilityLv { get; set; }
    }
}
