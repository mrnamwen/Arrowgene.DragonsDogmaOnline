using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;
using Arrowgene.Ddon.Shared.Entity.Structure;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SInfinityDeliveryDeliverItemReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_INFINITY_DELIVERY_DELIVER_CLIENT_REQ;

        public C2SInfinityDeliveryDeliverItemReq()
        {
            DeliverItems = new List<CDataItemUIDList>();
        }

        public uint CategoryId { get; set; }
        public List<CDataItemUIDList> DeliverItems { get; set; }

        public class Serializer : PacketEntitySerializer<C2SInfinityDeliveryDeliverItemReq>
        {
            public override void Write(IBuffer buffer, C2SInfinityDeliveryDeliverItemReq obj)
            {
                WriteUInt32(buffer, obj.CategoryId);
                WriteEntityList<CDataItemUIDList>(buffer, obj.DeliverItems);
            }

            public override C2SInfinityDeliveryDeliverItemReq Read(IBuffer buffer)
            {
                C2SInfinityDeliveryDeliverItemReq obj = new C2SInfinityDeliveryDeliverItemReq();
                obj.CategoryId = ReadUInt32(buffer);
                obj.DeliverItems = ReadEntityList<CDataItemUIDList>(buffer);
                return obj;
            }
        }
    }
}
