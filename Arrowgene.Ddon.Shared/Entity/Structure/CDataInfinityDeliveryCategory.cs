using Arrowgene.Buffers;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure;

public class CDataInfinityDeliveryCategory
{
    public CDataInfinityDeliveryCategory()
    {
        Name = string.Empty;
        Items = new List<CDataInfinityDeliveryItem>();
    }

    public uint CategoryId { get; set; }
    public string Name { get; set; }
    public List<CDataInfinityDeliveryItem> Items { get; set; }

    public class Serializer : EntitySerializer<CDataInfinityDeliveryCategory>
    {
        public override void Write(IBuffer buffer, CDataInfinityDeliveryCategory obj)
        {
            WriteUInt32(buffer, obj.CategoryId);
            WriteMtString(buffer, obj.Name);
            WriteEntityList<CDataInfinityDeliveryItem>(buffer, obj.Items);
        }

        public override CDataInfinityDeliveryCategory Read(IBuffer buffer)
        {
            CDataInfinityDeliveryCategory obj = new CDataInfinityDeliveryCategory();
            obj.CategoryId = ReadUInt32(buffer);
            obj.Name = ReadMtString(buffer);
            obj.Items = ReadEntityList<CDataInfinityDeliveryItem>(buffer);
            return obj;
        }
    }
}
