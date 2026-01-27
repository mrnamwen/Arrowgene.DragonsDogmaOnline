using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SInfinityDeliveryGetCurrentEventReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_INFINITY_DELIVERY_GET_CURRENT_EVENT_REQ;

        public class Serializer : PacketEntitySerializer<C2SInfinityDeliveryGetCurrentEventReq>
        {
            public override void Write(IBuffer buffer, C2SInfinityDeliveryGetCurrentEventReq obj)
            {
            }

            public override C2SInfinityDeliveryGetCurrentEventReq Read(IBuffer buffer)
            {
                return new C2SInfinityDeliveryGetCurrentEventReq();
            }
        }
    }
}
