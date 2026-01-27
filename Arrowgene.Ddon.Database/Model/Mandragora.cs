#nullable enable
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Database.Model;

/// <summary>
/// Represents a mandragora owned by a character.
/// </summary>
public class Mandragora
{
    public uint CharacterId { get; set; }
    public uint MandragoraId { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint SpeciesIndex { get; set; }
    public MandragoraSpeciesCategory SpeciesCategory { get; set; }
    public uint FurnitureItemId { get; set; }
    public uint GrowthLevel { get; set; }

    /// <summary>
    /// Converts to CDataMyMandragora for packet serialization.
    /// </summary>
    public CDataMyMandragora ToCDataMyMandragora()
    {
        return new CDataMyMandragora
        {
            SpeciesIndex = SpeciesIndex,
            SpeciesCategory = SpeciesCategory,
            MandragoraId = MandragoraId,
            MandragoraName = Name,
            Unk4 = 0,
            Unk5 = 0,
            Unk6 = GrowthLevel,
            Unk7 = new CDataMyMandragoraUnk1Unk7
            {
                Unk0 = MandragoraId,
                Unk1 = 0,
                Unk2 = new(),
                Unk3 = 0
            }
        };
    }

    /// <summary>
    /// Converts to CDataMyMandragoraFurnitureItem for packet serialization.
    /// </summary>
    public CDataMyMandragoraFurnitureItem ToCDataMyMandragoraFurnitureItem()
    {
        return new CDataMyMandragoraFurnitureItem
        {
            MandragoraId = MandragoraId,
            FurnitureItemId = (ItemId)FurnitureItemId
        };
    }
}

/// <summary>
/// Represents a mandragora species discovered by a character.
/// </summary>
public class MandragoraSpeciesDiscovery
{
    public uint CharacterId { get; set; }
    public uint SpeciesIndex { get; set; }
    public MandragoraSpeciesCategory SpeciesCategory { get; set; }
    public MandragoraRarity Rarity { get; set; }
    public bool IsNew { get; set; }
    public long DiscoveredDate { get; set; }

    /// <summary>
    /// Converts to CDataMyMandragoraSpecies for packet serialization.
    /// </summary>
    public CDataMyMandragoraSpecies ToCDataMyMandragoraSpecies(string firstDiscovery = "")
    {
        return new CDataMyMandragoraSpecies
        {
            Index = SpeciesIndex,
            Unk1 = 0,
            Rarity = Rarity,
            Unk3 = 0,
            Visible = true,
            Unk5 = DiscoveredDate > 0,
            FirstDiscovery = string.IsNullOrEmpty(firstDiscovery) ? SpeciesIndex.ToString() : firstDiscovery,
            DiscoveredDate = DiscoveredDate,
            New = IsNew
        };
    }
}
