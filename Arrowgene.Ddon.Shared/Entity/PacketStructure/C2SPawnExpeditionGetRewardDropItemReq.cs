using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SPawnExpeditionGetRewardDropItemReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_PAWN_EXPEDITION_PAWN_EXPEDITION_GET_REWARD_DROP_ITEM_REQ;

        public C2SPawnExpeditionGetRewardDropItemReq()
        {
        }

        public uint ItemIndex { get; set; }

        public class Serializer : PacketEntitySerializer<C2SPawnExpeditionGetRewardDropItemReq>
        {
            public override void Write(IBuffer buffer, C2SPawnExpeditionGetRewardDropItemReq obj)
            {
                WriteUInt32(buffer, obj.ItemIndex);
            }

            public override C2SPawnExpeditionGetRewardDropItemReq Read(IBuffer buffer)
            {
                C2SPawnExpeditionGetRewardDropItemReq obj = new C2SPawnExpeditionGetRewardDropItemReq();
                obj.ItemIndex = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
