namespace Arrowgene.Ddon.Shared.Model;

public class RewardMissionProgress
{
    public uint MissionId { get; set; }
    public uint CurrentCount { get; set; }
    public bool IsComplete { get; set; }
    public bool IsReceived { get; set; }
}
