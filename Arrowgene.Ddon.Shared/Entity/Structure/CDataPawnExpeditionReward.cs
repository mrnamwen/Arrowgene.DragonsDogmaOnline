using Arrowgene.Buffers;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataPawnExpeditionReward
    {
        public uint ItemId { get; set; }
        public uint Num { get; set; }

        public class Serializer : EntitySerializer<CDataPawnExpeditionReward>
        {
            public override void Write(IBuffer buffer, CDataPawnExpeditionReward obj)
            {
                WriteUInt32(buffer, obj.ItemId);
                WriteUInt32(buffer, obj.Num);
            }

            public override CDataPawnExpeditionReward Read(IBuffer buffer)
            {
                CDataPawnExpeditionReward obj = new CDataPawnExpeditionReward();
                obj.ItemId = ReadUInt32(buffer);
                obj.Num = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
