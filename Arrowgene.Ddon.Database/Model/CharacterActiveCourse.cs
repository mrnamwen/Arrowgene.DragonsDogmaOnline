#nullable enable
using System;

namespace Arrowgene.Ddon.Database.Model;

public class CharacterActiveCourse
{
    public ulong Id { get; set; }
    public uint CharacterId { get; set; }
    public uint CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public long StartTime { get; set; }
    public long EndTime { get; set; }
}
