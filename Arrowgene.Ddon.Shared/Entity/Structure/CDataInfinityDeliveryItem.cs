using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Shared.Entity.Structure;

public class CDataInfinityDeliveryItem
{
    public uint ItemId { get; set; }
    public uint PointValue { get; set; }

    public class Serializer : EntitySerializer<CDataInfinityDeliveryItem>
    {
        public override void Write(IBuffer buffer, CDataInfinityDeliveryItem obj)
        {
            WriteUInt32(buffer, obj.ItemId);
            WriteUInt32(buffer, obj.PointValue);
        }

        public override CDataInfinityDeliveryItem Read(IBuffer buffer)
        {
            CDataInfinityDeliveryItem obj = new CDataInfinityDeliveryItem();
            obj.ItemId = ReadUInt32(buffer);
            obj.PointValue = ReadUInt32(buffer);
            return obj;
        }
    }
}
