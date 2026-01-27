using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataDailyMissionReward
    {
        public CDataDailyMissionReward()
        {
        }

        public uint ItemId { get; set; }
        public uint Num { get; set; }
        public WalletType WalletType { get; set; }
        public uint WalletAmount { get; set; }

        public class Serializer : EntitySerializer<CDataDailyMissionReward>
        {
            public override void Write(IBuffer buffer, CDataDailyMissionReward obj)
            {
                WriteUInt32(buffer, obj.ItemId);
                WriteUInt32(buffer, obj.Num);
                WriteByte(buffer, (byte)obj.WalletType);
                WriteUInt32(buffer, obj.WalletAmount);
            }

            public override CDataDailyMissionReward Read(IBuffer buffer)
            {
                CDataDailyMissionReward obj = new CDataDailyMissionReward();
                obj.ItemId = ReadUInt32(buffer);
                obj.Num = ReadUInt32(buffer);
                obj.WalletType = (WalletType)ReadByte(buffer);
                obj.WalletAmount = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
