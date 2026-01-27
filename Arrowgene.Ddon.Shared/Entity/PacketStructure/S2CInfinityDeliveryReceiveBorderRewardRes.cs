using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CInfinityDeliveryReceiveBorderRewardRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_INFINITY_DELIVERY_RECEIVE_BORDER_REWARD_RES;

        public S2CInfinityDeliveryReceiveBorderRewardRes()
        {
            Status = new CDataInfinityDeliveryStatus();
        }

        public CDataInfinityDeliveryStatus Status { get; set; }

        public class Serializer : PacketEntitySerializer<S2CInfinityDeliveryReceiveBorderRewardRes>
        {
            public override void Write(IBuffer buffer, S2CInfinityDeliveryReceiveBorderRewardRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntity<CDataInfinityDeliveryStatus>(buffer, obj.Status);
            }

            public override S2CInfinityDeliveryReceiveBorderRewardRes Read(IBuffer buffer)
            {
                S2CInfinityDeliveryReceiveBorderRewardRes obj = new S2CInfinityDeliveryReceiveBorderRewardRes();
                ReadServerResponse(buffer, obj);
                obj.Status = ReadEntity<CDataInfinityDeliveryStatus>(buffer);
                return obj;
            }
        }
    }
}
