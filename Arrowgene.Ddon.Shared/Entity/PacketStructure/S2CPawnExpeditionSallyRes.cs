using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CPawnExpeditionSallyRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_PAWN_EXPEDITION_PAWN_EXPEDITION_SALLY_RES;

        public S2CPawnExpeditionSallyRes()
        {
        }

        public byte SallyCount { get; set; }
        public ulong EndTime { get; set; }

        public class Serializer : PacketEntitySerializer<S2CPawnExpeditionSallyRes>
        {
            public override void Write(IBuffer buffer, S2CPawnExpeditionSallyRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteByte(buffer, obj.SallyCount);
                WriteUInt64(buffer, obj.EndTime);
            }

            public override S2CPawnExpeditionSallyRes Read(IBuffer buffer)
            {
                S2CPawnExpeditionSallyRes obj = new S2CPawnExpeditionSallyRes();
                ReadServerResponse(buffer, obj);
                obj.SallyCount = ReadByte(buffer);
                obj.EndTime = ReadUInt64(buffer);
                return obj;
            }
        }
    }
}
