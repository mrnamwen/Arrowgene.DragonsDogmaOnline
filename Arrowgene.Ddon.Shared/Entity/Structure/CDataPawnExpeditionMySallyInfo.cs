using Arrowgene.Buffers;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataPawnExpeditionMySallyInfo
    {
        public uint PawnId { get; set; }
        public uint AreaId { get; set; }
        public uint SpotId { get; set; }
        public ulong StartTime { get; set; }
        public ulong EndTime { get; set; }
        public bool IsGolden { get; set; }
        public byte Status { get; set; }

        public class Serializer : EntitySerializer<CDataPawnExpeditionMySallyInfo>
        {
            public override void Write(IBuffer buffer, CDataPawnExpeditionMySallyInfo obj)
            {
                WriteUInt32(buffer, obj.PawnId);
                WriteUInt32(buffer, obj.AreaId);
                WriteUInt32(buffer, obj.SpotId);
                WriteUInt64(buffer, obj.StartTime);
                WriteUInt64(buffer, obj.EndTime);
                WriteBool(buffer, obj.IsGolden);
                WriteByte(buffer, obj.Status);
            }

            public override CDataPawnExpeditionMySallyInfo Read(IBuffer buffer)
            {
                CDataPawnExpeditionMySallyInfo obj = new CDataPawnExpeditionMySallyInfo();
                obj.PawnId = ReadUInt32(buffer);
                obj.AreaId = ReadUInt32(buffer);
                obj.SpotId = ReadUInt32(buffer);
                obj.StartTime = ReadUInt64(buffer);
                obj.EndTime = ReadUInt64(buffer);
                obj.IsGolden = ReadBool(buffer);
                obj.Status = ReadByte(buffer);
                return obj;
            }
        }
    }
}
