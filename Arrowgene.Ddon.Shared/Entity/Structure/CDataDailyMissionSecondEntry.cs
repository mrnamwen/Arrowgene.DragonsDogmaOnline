using Arrowgene.Buffers;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataDailyMissionSecondEntry
    {
        public CDataDailyMissionSecondEntry()
        {
        }

        // 4 x uint32 = 16 bytes per entry (purpose unknown)
        public uint Value1 { get; set; }
        public uint Value2 { get; set; }
        public uint Value3 { get; set; }
        public uint Value4 { get; set; }

        public class Serializer : EntitySerializer<CDataDailyMissionSecondEntry>
        {
            public override void Write(IBuffer buffer, CDataDailyMissionSecondEntry obj)
            {
                WriteUInt32(buffer, obj.Value1);
                WriteUInt32(buffer, obj.Value2);
                WriteUInt32(buffer, obj.Value3);
                WriteUInt32(buffer, obj.Value4);
            }

            public override CDataDailyMissionSecondEntry Read(IBuffer buffer)
            {
                CDataDailyMissionSecondEntry obj = new CDataDailyMissionSecondEntry();
                obj.Value1 = ReadUInt32(buffer);
                obj.Value2 = ReadUInt32(buffer);
                obj.Value3 = ReadUInt32(buffer);
                obj.Value4 = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
