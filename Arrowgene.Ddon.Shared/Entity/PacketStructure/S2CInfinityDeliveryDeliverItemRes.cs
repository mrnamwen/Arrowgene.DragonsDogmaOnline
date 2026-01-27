using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CInfinityDeliveryDeliverItemRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_INFINITY_DELIVERY_DELIVER_CLIENT_RES;

        public S2CInfinityDeliveryDeliverItemRes()
        {
            Status = new CDataInfinityDeliveryStatus();
        }

        public uint PointsEarned { get; set; }
        public CDataInfinityDeliveryStatus Status { get; set; }

        public class Serializer : PacketEntitySerializer<S2CInfinityDeliveryDeliverItemRes>
        {
            public override void Write(IBuffer buffer, S2CInfinityDeliveryDeliverItemRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt32(buffer, obj.PointsEarned);
                WriteEntity<CDataInfinityDeliveryStatus>(buffer, obj.Status);
            }

            public override S2CInfinityDeliveryDeliverItemRes Read(IBuffer buffer)
            {
                S2CInfinityDeliveryDeliverItemRes obj = new S2CInfinityDeliveryDeliverItemRes();
                ReadServerResponse(buffer, obj);
                obj.PointsEarned = ReadUInt32(buffer);
                obj.Status = ReadEntity<CDataInfinityDeliveryStatus>(buffer);
                return obj;
            }
        }
    }
}
