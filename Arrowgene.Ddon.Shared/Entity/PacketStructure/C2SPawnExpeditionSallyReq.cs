using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SPawnExpeditionSallyReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_PAWN_EXPEDITION_PAWN_EXPEDITION_SALLY_REQ;

        public C2SPawnExpeditionSallyReq()
        {
        }

        public uint PawnId { get; set; }
        public uint AreaId { get; set; }
        public uint SpotId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SPawnExpeditionSallyReq>
        {
            public override void Write(IBuffer buffer, C2SPawnExpeditionSallyReq obj)
            {
                WriteUInt32(buffer, obj.PawnId);
                WriteUInt32(buffer, obj.AreaId);
                WriteUInt32(buffer, obj.SpotId);
            }

            public override C2SPawnExpeditionSallyReq Read(IBuffer buffer)
            {
                C2SPawnExpeditionSallyReq obj = new C2SPawnExpeditionSallyReq();
                obj.PawnId = ReadUInt32(buffer);
                obj.AreaId = ReadUInt32(buffer);
                obj.SpotId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
