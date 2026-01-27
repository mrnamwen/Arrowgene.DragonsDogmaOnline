using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CInfinityDeliveryGetNumBorderInfoRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_INFINITY_DELIVERY_GET_NUM_BORDER_INFO_RES;

        public S2CInfinityDeliveryGetNumBorderInfoRes()
        {
            BorderList = new List<CDataInfinityDeliveryBorder>();
        }

        public List<CDataInfinityDeliveryBorder> BorderList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CInfinityDeliveryGetNumBorderInfoRes>
        {
            public override void Write(IBuffer buffer, S2CInfinityDeliveryGetNumBorderInfoRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList<CDataInfinityDeliveryBorder>(buffer, obj.BorderList);
            }

            public override S2CInfinityDeliveryGetNumBorderInfoRes Read(IBuffer buffer)
            {
                S2CInfinityDeliveryGetNumBorderInfoRes obj = new S2CInfinityDeliveryGetNumBorderInfoRes();
                ReadServerResponse(buffer, obj);
                obj.BorderList = ReadEntityList<CDataInfinityDeliveryBorder>(buffer);
                return obj;
            }
        }
    }
}
