using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SPawnExpeditionGetMySallyInfoReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_PAWN_EXPEDITION_PAWN_EXPEDITION_GET_MY_SALLY_INFO_REQ;

        public C2SPawnExpeditionGetMySallyInfoReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SPawnExpeditionGetMySallyInfoReq>
        {
            public override void Write(IBuffer buffer, C2SPawnExpeditionGetMySallyInfoReq obj)
            {
            }

            public override C2SPawnExpeditionGetMySallyInfoReq Read(IBuffer buffer)
            {
                return new C2SPawnExpeditionGetMySallyInfoReq();
            }
        }
    }
}
