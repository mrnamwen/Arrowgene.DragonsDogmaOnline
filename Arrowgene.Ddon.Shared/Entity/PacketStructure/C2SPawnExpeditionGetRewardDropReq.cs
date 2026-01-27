using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SPawnExpeditionGetRewardDropReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_PAWN_EXPEDITION_PAWN_EXPEDITION_GET_REWARD_DROP_REQ;

        public C2SPawnExpeditionGetRewardDropReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SPawnExpeditionGetRewardDropReq>
        {
            public override void Write(IBuffer buffer, C2SPawnExpeditionGetRewardDropReq obj)
            {
            }

            public override C2SPawnExpeditionGetRewardDropReq Read(IBuffer buffer)
            {
                return new C2SPawnExpeditionGetRewardDropReq();
            }
        }
    }
}
