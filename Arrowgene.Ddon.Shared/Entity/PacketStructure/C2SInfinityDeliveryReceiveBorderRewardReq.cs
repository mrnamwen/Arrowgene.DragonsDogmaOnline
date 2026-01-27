using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SInfinityDeliveryReceiveBorderRewardReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_INFINITY_DELIVERY_RECEIVE_BORDER_REWARD_REQ;

        public uint BorderId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SInfinityDeliveryReceiveBorderRewardReq>
        {
            public override void Write(IBuffer buffer, C2SInfinityDeliveryReceiveBorderRewardReq obj)
            {
                WriteUInt32(buffer, obj.BorderId);
            }

            public override C2SInfinityDeliveryReceiveBorderRewardReq Read(IBuffer buffer)
            {
                C2SInfinityDeliveryReceiveBorderRewardReq obj = new C2SInfinityDeliveryReceiveBorderRewardReq();
                obj.BorderId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
