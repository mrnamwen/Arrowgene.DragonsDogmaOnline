using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SPawnExpeditionCancelSallyReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_PAWN_EXPEDITION_PAWN_EXPEDITION_CANCEL_SALLY_REQ;

        public C2SPawnExpeditionCancelSallyReq()
        {
        }

        public uint PawnId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SPawnExpeditionCancelSallyReq>
        {
            public override void Write(IBuffer buffer, C2SPawnExpeditionCancelSallyReq obj)
            {
                WriteUInt32(buffer, obj.PawnId);
            }

            public override C2SPawnExpeditionCancelSallyReq Read(IBuffer buffer)
            {
                C2SPawnExpeditionCancelSallyReq obj = new C2SPawnExpeditionCancelSallyReq();
                obj.PawnId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
