#nullable enable
using System;

namespace Arrowgene.Ddon.Database.Model;

public class BoxGachaState
{
    public uint CharacterId { get; set; }
    public uint BoxGachaId { get; set; }
    public uint LineupId { get; set; }
    public DateTime DrawnTime { get; set; }
}
