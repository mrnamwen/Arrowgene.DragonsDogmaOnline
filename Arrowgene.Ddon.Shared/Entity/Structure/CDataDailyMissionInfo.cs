using Arrowgene.Buffers;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataDailyMissionInfo
    {
        public CDataDailyMissionInfo()
        {
            Title = string.Empty;
            IconUrl = string.Empty;
            ImageUrl = string.Empty;
            RewardList = new List<CDataDailyMissionReward>();
        }

        public uint MissionId { get; set; }
        public uint Category { get; set; }
        public uint Type { get; set; }
        public uint TargetCount { get; set; }
        public uint CurrentCount { get; set; }
        public bool IsComplete { get; set; }
        public bool IsReceived { get; set; }
        public uint SortId { get; set; }
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public ulong StartTime { get; set; }
        public ulong EndTime { get; set; }
        public List<CDataDailyMissionReward> RewardList { get; set; }

        public class Serializer : EntitySerializer<CDataDailyMissionInfo>
        {
            public override void Write(IBuffer buffer, CDataDailyMissionInfo obj)
            {
                WriteUInt32(buffer, obj.MissionId);
                WriteUInt32(buffer, obj.Category);
                WriteUInt32(buffer, obj.Type);
                WriteUInt32(buffer, obj.TargetCount);
                WriteUInt32(buffer, obj.CurrentCount);
                WriteBool(buffer, obj.IsComplete);
                WriteBool(buffer, obj.IsReceived);
                WriteUInt32(buffer, obj.SortId);
                WriteMtString(buffer, obj.Title);
                WriteMtString(buffer, obj.IconUrl);
                WriteMtString(buffer, obj.ImageUrl);
                WriteUInt64(buffer, obj.StartTime);
                WriteUInt64(buffer, obj.EndTime);
                WriteEntityList<CDataDailyMissionReward>(buffer, obj.RewardList);
            }

            public override CDataDailyMissionInfo Read(IBuffer buffer)
            {
                CDataDailyMissionInfo obj = new CDataDailyMissionInfo();
                obj.MissionId = ReadUInt32(buffer);
                obj.Category = ReadUInt32(buffer);
                obj.Type = ReadUInt32(buffer);
                obj.TargetCount = ReadUInt32(buffer);
                obj.CurrentCount = ReadUInt32(buffer);
                obj.IsComplete = ReadBool(buffer);
                obj.IsReceived = ReadBool(buffer);
                obj.SortId = ReadUInt32(buffer);
                obj.Title = ReadMtString(buffer);
                obj.IconUrl = ReadMtString(buffer);
                obj.ImageUrl = ReadMtString(buffer);
                obj.StartTime = ReadUInt64(buffer);
                obj.EndTime = ReadUInt64(buffer);
                obj.RewardList = ReadEntityList<CDataDailyMissionReward>(buffer);
                return obj;
            }
        }
    }
}
