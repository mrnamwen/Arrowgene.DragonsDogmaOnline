using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SInfinityDeliveryGetNumBorderInfoReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_INFINITY_DELIVERY_GET_NUM_BORDER_INFO_REQ;

        public class Serializer : PacketEntitySerializer<C2SInfinityDeliveryGetNumBorderInfoReq>
        {
            public override void Write(IBuffer buffer, C2SInfinityDeliveryGetNumBorderInfoReq obj)
            {
            }

            public override C2SInfinityDeliveryGetNumBorderInfoReq Read(IBuffer buffer)
            {
                return new C2SInfinityDeliveryGetNumBorderInfoReq();
            }
        }
    }
}
