using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SInfinityDeliveryGetCategoryListReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_INFINITY_DELIVERY_GET_CATEGORY_LIST_REQ;

        public class Serializer : PacketEntitySerializer<C2SInfinityDeliveryGetCategoryListReq>
        {
            public override void Write(IBuffer buffer, C2SInfinityDeliveryGetCategoryListReq obj)
            {
            }

            public override C2SInfinityDeliveryGetCategoryListReq Read(IBuffer buffer)
            {
                return new C2SInfinityDeliveryGetCategoryListReq();
            }
        }
    }
}
