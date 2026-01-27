using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SInfinityDeliveryGetEventStatusReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_INFINITY_DELIVERY_GET_EVENT_STATUS_REQ;

        public class Serializer : PacketEntitySerializer<C2SInfinityDeliveryGetEventStatusReq>
        {
            public override void Write(IBuffer buffer, C2SInfinityDeliveryGetEventStatusReq obj)
            {
            }

            public override C2SInfinityDeliveryGetEventStatusReq Read(IBuffer buffer)
            {
                return new C2SInfinityDeliveryGetEventStatusReq();
            }
        }
    }
}
