using Arrowgene.Ddon.Shared.Model;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class DailyMissionAsset
    {
        public DailyMissionAsset()
        {
            Missions = new List<DailyMission>();
        }

        public List<DailyMission> Missions { get; set; }
    }

    public class DailyMission
    {
        public DailyMission()
        {
            Title = string.Empty;
            IconUrl = string.Empty;
            ImageUrl = string.Empty;
            Rewards = new List<DailyMissionReward>();
        }

        public uint MissionId { get; set; }
        public DailyMissionCategory Category { get; set; }
        public DailyMissionType Type { get; set; }
        public uint TargetCount { get; set; }
        public uint SortId { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public List<DailyMissionReward> Rewards { get; set; }
    }

    public class DailyMissionReward
    {
        public uint ItemId { get; set; }
        public uint Num { get; set; }
        public WalletType WalletType { get; set; }
        public uint WalletAmount { get; set; }
    }

    public enum DailyMissionCategory : uint
    {
        Daily = 1,
        Weekly = 2
    }

    public enum DailyMissionType : uint
    {
        KillEnemy = 1,
        GatherItem = 2,
        ClearQuest = 3,
        ClearDungeon = 4
    }
}
