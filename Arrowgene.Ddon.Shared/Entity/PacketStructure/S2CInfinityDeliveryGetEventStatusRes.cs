using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CInfinityDeliveryGetEventStatusRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_INFINITY_DELIVERY_GET_EVENT_STATUS_RES;

        public S2CInfinityDeliveryGetEventStatusRes()
        {
            Status = new CDataInfinityDeliveryStatus();
        }

        public CDataInfinityDeliveryStatus Status { get; set; }

        public class Serializer : PacketEntitySerializer<S2CInfinityDeliveryGetEventStatusRes>
        {
            public override void Write(IBuffer buffer, S2CInfinityDeliveryGetEventStatusRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntity<CDataInfinityDeliveryStatus>(buffer, obj.Status);
            }

            public override S2CInfinityDeliveryGetEventStatusRes Read(IBuffer buffer)
            {
                S2CInfinityDeliveryGetEventStatusRes obj = new S2CInfinityDeliveryGetEventStatusRes();
                ReadServerResponse(buffer, obj);
                obj.Status = ReadEntity<CDataInfinityDeliveryStatus>(buffer);
                return obj;
            }
        }
    }
}
