using System;

namespace Arrowgene.Ddon.Shared.Model;

public class RewardMissionState
{
    private static readonly DateTime DateTimeSafeMinValue = new(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public RewardMissionState()
    {
        LastResetTime = DateTimeSafeMinValue;
        MissionsCompleted = 0;
    }

    public DateTime LastResetTime { get; set; }
    public uint MissionsCompleted { get; set; }
}
