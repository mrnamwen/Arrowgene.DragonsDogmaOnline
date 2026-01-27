#nullable enable
using System;

namespace Arrowgene.Ddon.Database.Model;

public class CharacterAvailableCourse
{
    public ulong Id { get; set; }
    public uint CharacterId { get; set; }
    public uint CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public uint DurationSec { get; set; }
    public uint LineupId { get; set; }
    public uint BackIconId { get; set; }
    public uint FrameIconId { get; set; }
    public DateTime PurchaseTime { get; set; }
}
