using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SPawnExpeditionChargeSallyCountReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_PAWN_EXPEDITION_PAWN_EXPEDITION_CHARGE_SALLY_COUNT_REQ;

        public C2SPawnExpeditionChargeSallyCountReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SPawnExpeditionChargeSallyCountReq>
        {
            public override void Write(IBuffer buffer, C2SPawnExpeditionChargeSallyCountReq obj)
            {
            }

            public override C2SPawnExpeditionChargeSallyCountReq Read(IBuffer buffer)
            {
                return new C2SPawnExpeditionChargeSallyCountReq();
            }
        }
    }
}
