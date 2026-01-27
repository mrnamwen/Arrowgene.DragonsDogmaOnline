using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CInfinityDeliveryGetCategoryListRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_INFINITY_DELIVERY_GET_CATEGORY_LIST_RES;

        public S2CInfinityDeliveryGetCategoryListRes()
        {
            CategoryList = new List<CDataInfinityDeliveryCategory>();
        }

        public List<CDataInfinityDeliveryCategory> CategoryList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CInfinityDeliveryGetCategoryListRes>
        {
            public override void Write(IBuffer buffer, S2CInfinityDeliveryGetCategoryListRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList<CDataInfinityDeliveryCategory>(buffer, obj.CategoryList);
            }

            public override S2CInfinityDeliveryGetCategoryListRes Read(IBuffer buffer)
            {
                S2CInfinityDeliveryGetCategoryListRes obj = new S2CInfinityDeliveryGetCategoryListRes();
                ReadServerResponse(buffer, obj);
                obj.CategoryList = ReadEntityList<CDataInfinityDeliveryCategory>(buffer);
                return obj;
            }
        }
    }
}
