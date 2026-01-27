#nullable enable
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Database.Model;

/// <summary>
/// Represents a saved equipment preset for a character.
/// Stores equipment UIDs for both performance and visual equipment slots.
/// </summary>
public class EquipPreset
{
    public uint CharacterId { get; set; }
    public JobId Job { get; set; }
    public byte PresetNo { get; set; }
    public string PresetName { get; set; } = string.Empty;

    // Performance equipment slots (UIDs)
    public string? PPrimaryWeapon { get; set; }
    public string? PSecondaryWeapon { get; set; }
    public string? PHead { get; set; }
    public string? PBody { get; set; }
    public string? PClothing { get; set; }
    public string? PArm { get; set; }
    public string? PLeg { get; set; }
    public string? PLegWear { get; set; }
    public string? POverWear { get; set; }
    public string? PJewelry1 { get; set; }
    public string? PJewelry2 { get; set; }
    public string? PJewelry3 { get; set; }
    public string? PJewelry4 { get; set; }
    public string? PJewelry5 { get; set; }
    public string? PLantern { get; set; }

    // Visual equipment slots (UIDs)
    public string? VPrimaryWeapon { get; set; }
    public string? VSecondaryWeapon { get; set; }
    public string? VHead { get; set; }
    public string? VBody { get; set; }
    public string? VClothing { get; set; }
    public string? VArm { get; set; }
    public string? VLeg { get; set; }
    public string? VLegWear { get; set; }
    public string? VOverWear { get; set; }

    /// <summary>
    /// Converts this preset to the CDataEquipPreset structure for packet serialization.
    /// </summary>
    public CDataEquipPreset ToCDataEquipPreset()
    {
        return new CDataEquipPreset()
        {
            PresetNo = PresetNo,
            PresetName = PresetName,
            Job = Job
        };
    }

    /// <summary>
    /// Gets the equipment UID for a given slot from this preset.
    /// </summary>
    public string? GetEquipmentUId(EquipType equipType, byte slot)
    {
        if (equipType == EquipType.Performance)
        {
            return slot switch
            {
                1 => PPrimaryWeapon,
                2 => PSecondaryWeapon,
                3 => PHead,
                4 => PBody,
                5 => PClothing,
                6 => PArm,
                7 => PLeg,
                8 => PLegWear,
                9 => POverWear,
                10 => PJewelry1,
                11 => PJewelry2,
                12 => PJewelry3,
                13 => PJewelry4,
                14 => PJewelry5,
                15 => PLantern,
                _ => null
            };
        }
        else // Visual
        {
            return slot switch
            {
                1 => VPrimaryWeapon,
                2 => VSecondaryWeapon,
                3 => VHead,
                4 => VBody,
                5 => VClothing,
                6 => VArm,
                7 => VLeg,
                8 => VLegWear,
                9 => VOverWear,
                _ => null
            };
        }
    }

    /// <summary>
    /// Sets the equipment UID for a given slot in this preset.
    /// </summary>
    public void SetEquipmentUId(EquipType equipType, byte slot, string? uid)
    {
        if (equipType == EquipType.Performance)
        {
            switch (slot)
            {
                case 1: PPrimaryWeapon = uid; break;
                case 2: PSecondaryWeapon = uid; break;
                case 3: PHead = uid; break;
                case 4: PBody = uid; break;
                case 5: PClothing = uid; break;
                case 6: PArm = uid; break;
                case 7: PLeg = uid; break;
                case 8: PLegWear = uid; break;
                case 9: POverWear = uid; break;
                case 10: PJewelry1 = uid; break;
                case 11: PJewelry2 = uid; break;
                case 12: PJewelry3 = uid; break;
                case 13: PJewelry4 = uid; break;
                case 14: PJewelry5 = uid; break;
                case 15: PLantern = uid; break;
            }
        }
        else // Visual
        {
            switch (slot)
            {
                case 1: VPrimaryWeapon = uid; break;
                case 2: VSecondaryWeapon = uid; break;
                case 3: VHead = uid; break;
                case 4: VBody = uid; break;
                case 5: VClothing = uid; break;
                case 6: VArm = uid; break;
                case 7: VLeg = uid; break;
                case 8: VLegWear = uid; break;
                case 9: VOverWear = uid; break;
            }
        }
    }
}
