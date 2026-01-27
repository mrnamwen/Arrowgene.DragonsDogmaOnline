using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CPawnExpeditionChangeGoldenSallyRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_PAWN_EXPEDITION_PAWN_EXPEDITION_CHANGE_GOLDEN_SALLY_RES;

        public S2CPawnExpeditionChangeGoldenSallyRes()
        {
        }

        public class Serializer : PacketEntitySerializer<S2CPawnExpeditionChangeGoldenSallyRes>
        {
            public override void Write(IBuffer buffer, S2CPawnExpeditionChangeGoldenSallyRes obj)
            {
                WriteServerResponse(buffer, obj);
            }

            public override S2CPawnExpeditionChangeGoldenSallyRes Read(IBuffer buffer)
            {
                S2CPawnExpeditionChangeGoldenSallyRes obj = new S2CPawnExpeditionChangeGoldenSallyRes();
                ReadServerResponse(buffer, obj);
                return obj;
            }
        }
    }
}
