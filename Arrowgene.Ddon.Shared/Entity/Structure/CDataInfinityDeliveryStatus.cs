using Arrowgene.Buffers;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.Structure;

public class CDataInfinityDeliveryStatus
{
    public CDataInfinityDeliveryStatus()
    {
        ReceivedBorderIds = new List<CDataCommonU32>();
    }

    public uint CurrentPoints { get; set; }
    public List<CDataCommonU32> ReceivedBorderIds { get; set; }

    public class Serializer : EntitySerializer<CDataInfinityDeliveryStatus>
    {
        public override void Write(IBuffer buffer, CDataInfinityDeliveryStatus obj)
        {
            WriteUInt32(buffer, obj.CurrentPoints);
            WriteEntityList<CDataCommonU32>(buffer, obj.ReceivedBorderIds);
        }

        public override CDataInfinityDeliveryStatus Read(IBuffer buffer)
        {
            CDataInfinityDeliveryStatus obj = new CDataInfinityDeliveryStatus();
            obj.CurrentPoints = ReadUInt32(buffer);
            obj.ReceivedBorderIds = ReadEntityList<CDataCommonU32>(buffer);
            return obj;
        }
    }
}
