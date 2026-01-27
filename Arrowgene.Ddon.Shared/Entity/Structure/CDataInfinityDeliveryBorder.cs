using Arrowgene.Buffers;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure;

public class CDataInfinityDeliveryBorder
{
    public CDataInfinityDeliveryBorder()
    {
        Rewards = new List<CDataRewardItem>();
    }

    public uint BorderId { get; set; }
    public uint RequiredPoints { get; set; }
    public List<CDataRewardItem> Rewards { get; set; }

    public class Serializer : EntitySerializer<CDataInfinityDeliveryBorder>
    {
        public override void Write(IBuffer buffer, CDataInfinityDeliveryBorder obj)
        {
            WriteUInt32(buffer, obj.BorderId);
            WriteUInt32(buffer, obj.RequiredPoints);
            WriteEntityList<CDataRewardItem>(buffer, obj.Rewards);
        }

        public override CDataInfinityDeliveryBorder Read(IBuffer buffer)
        {
            CDataInfinityDeliveryBorder obj = new CDataInfinityDeliveryBorder();
            obj.BorderId = ReadUInt32(buffer);
            obj.RequiredPoints = ReadUInt32(buffer);
            obj.Rewards = ReadEntityList<CDataRewardItem>(buffer);
            return obj;
        }
    }
}
