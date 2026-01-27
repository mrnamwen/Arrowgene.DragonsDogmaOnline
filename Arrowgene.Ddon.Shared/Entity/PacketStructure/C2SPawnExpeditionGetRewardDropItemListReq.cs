using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SPawnExpeditionGetRewardDropItemListReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_PAWN_EXPEDITION_PAWN_EXPEDITION_GET_REWARD_DROP_ITEM_LIST_REQ;

        public C2SPawnExpeditionGetRewardDropItemListReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SPawnExpeditionGetRewardDropItemListReq>
        {
            public override void Write(IBuffer buffer, C2SPawnExpeditionGetRewardDropItemListReq obj)
            {
            }

            public override C2SPawnExpeditionGetRewardDropItemListReq Read(IBuffer buffer)
            {
                return new C2SPawnExpeditionGetRewardDropItemListReq();
            }
        }
    }
}
