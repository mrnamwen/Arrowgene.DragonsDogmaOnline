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
            SecondList = new List<CDataDailyMissionSecondEntry>();
        }

        // Core mission fields (5 x uint32 = 20 bytes)
        public uint MissionId { get; set; }
        public uint Category { get; set; }
        public uint Type { get; set; }
        public uint TargetCount { get; set; }
        public uint CurrentCount { get; set; }

        // Unknown fields before RewardList (4 x uint32 = 16 bytes, all zeros in packet dump)
        public uint Unknown1 { get; set; }
        public uint Unknown2 { get; set; }
        public uint Unknown3 { get; set; }
        public uint Unknown4 { get; set; }

        // Reward list (count + entries, each entry = 16 bytes)
        public List<CDataDailyMissionReward> RewardList { get; set; }

        // Second list - purpose unknown (count + entries, each entry = 16 bytes: 4 x uint32)
        public List<CDataDailyMissionSecondEntry> SecondList { get; set; }

        // Unknown fields after SecondList, before strings (10 bytes: 2 x uint32 + 1 x uint16)
        public uint Unknown5 { get; set; }
        public uint Unknown6 { get; set; }
        public ushort Unknown7 { get; set; }

        // String fields
        public string Title { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }

        // Trailing fields after strings (25 bytes: uint32 + byte + uint64 + uint64 + uint32)
        public uint Unknown10 { get; set; }
        public byte Flags { get; set; }  // Contains IsComplete (bit 0) and IsReceived (bit 1)
        public ulong StartTime { get; set; }
        public ulong EndTime { get; set; }
        public uint SortId { get; set; }

        // Helper properties for backwards compatibility
        public bool IsComplete
        {
            get => (Flags & 0x01) != 0;
            set => Flags = (byte)(value ? (Flags | 0x01) : (Flags & ~0x01));
        }

        public bool IsReceived
        {
            get => (Flags & 0x02) != 0;
            set => Flags = (byte)(value ? (Flags | 0x02) : (Flags & ~0x02));
        }

        public class Serializer : EntitySerializer<CDataDailyMissionInfo>
        {
            public override void Write(IBuffer buffer, CDataDailyMissionInfo obj)
            {
                // Core mission fields (5 x uint32 = 20 bytes)
                WriteUInt32(buffer, obj.MissionId);
                WriteUInt32(buffer, obj.Category);
                WriteUInt32(buffer, obj.Type);
                WriteUInt32(buffer, obj.TargetCount);
                WriteUInt32(buffer, obj.CurrentCount);

                // Unknown fields before RewardList (4 x uint32 = 16 bytes)
                WriteUInt32(buffer, obj.Unknown1);
                WriteUInt32(buffer, obj.Unknown2);
                WriteUInt32(buffer, obj.Unknown3);
                WriteUInt32(buffer, obj.Unknown4);

                // RewardList
                WriteEntityList<CDataDailyMissionReward>(buffer, obj.RewardList);

                // SecondList (purpose unknown)
                WriteEntityList<CDataDailyMissionSecondEntry>(buffer, obj.SecondList);

                // Unknown fields after SecondList (10 bytes: 2 x uint32 + 1 x uint16)
                WriteUInt32(buffer, obj.Unknown5);
                WriteUInt32(buffer, obj.Unknown6);
                WriteUInt16(buffer, obj.Unknown7);

                // String fields
                WriteMtString(buffer, obj.Title);
                WriteMtString(buffer, obj.IconUrl);
                WriteMtString(buffer, obj.ImageUrl);

                // Trailing fields (25 bytes: uint32 + byte + uint64 + uint64 + uint32)
                WriteUInt32(buffer, obj.Unknown10);
                WriteByte(buffer, obj.Flags);
                WriteUInt64(buffer, obj.StartTime);
                WriteUInt64(buffer, obj.EndTime);
                WriteUInt32(buffer, obj.SortId);
            }

            public override CDataDailyMissionInfo Read(IBuffer buffer)
            {
                CDataDailyMissionInfo obj = new CDataDailyMissionInfo();

                // Core mission fields (5 x uint32 = 20 bytes)
                obj.MissionId = ReadUInt32(buffer);
                obj.Category = ReadUInt32(buffer);
                obj.Type = ReadUInt32(buffer);
                obj.TargetCount = ReadUInt32(buffer);
                obj.CurrentCount = ReadUInt32(buffer);

                // Unknown fields before RewardList (4 x uint32 = 16 bytes)
                obj.Unknown1 = ReadUInt32(buffer);
                obj.Unknown2 = ReadUInt32(buffer);
                obj.Unknown3 = ReadUInt32(buffer);
                obj.Unknown4 = ReadUInt32(buffer);

                // RewardList
                obj.RewardList = ReadEntityList<CDataDailyMissionReward>(buffer);

                // SecondList (purpose unknown)
                obj.SecondList = ReadEntityList<CDataDailyMissionSecondEntry>(buffer);

                // Unknown fields after SecondList (10 bytes: 2 x uint32 + 1 x uint16)
                obj.Unknown5 = ReadUInt32(buffer);
                obj.Unknown6 = ReadUInt32(buffer);
                obj.Unknown7 = ReadUInt16(buffer);

                // String fields
                obj.Title = ReadMtString(buffer);
                obj.IconUrl = ReadMtString(buffer);
                obj.ImageUrl = ReadMtString(buffer);

                // Trailing fields (25 bytes: uint32 + byte + uint64 + uint64 + uint32)
                obj.Unknown10 = ReadUInt32(buffer);
                obj.Flags = ReadByte(buffer);
                obj.StartTime = ReadUInt64(buffer);
                obj.EndTime = ReadUInt64(buffer);
                obj.SortId = ReadUInt32(buffer);

                return obj;
            }
        }
    }
}
