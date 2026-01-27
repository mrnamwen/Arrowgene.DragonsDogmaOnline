using System.Collections.Generic;
using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    /// <summary>
    /// Response for GP Shop purchase.
    /// Based on PS4 IDA analysis, adapted for PC client format.
    /// </summary>
    public class S2CGpGpShopDisplayBuyRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_GP_GP_SHOP_DISPLAY_BUY_RES;

        public uint LineupId { get; set; }
        public List<CDataCommonU32> CommonIdList { get; set; }
        public uint Balance { get; set; }
        public string LineupName { get; set; }
        public bool IsReceived { get; set; }

        public S2CGpGpShopDisplayBuyRes()
        {
            CommonIdList = new List<CDataCommonU32>();
            LineupName = string.Empty;
        }

        public class Serializer : PacketEntitySerializer<S2CGpGpShopDisplayBuyRes>
        {
            public override void Write(IBuffer buffer, S2CGpGpShopDisplayBuyRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt32(buffer, obj.LineupId);
                WriteEntityList<CDataCommonU32>(buffer, obj.CommonIdList);
                WriteUInt32(buffer, obj.Balance);
                WriteMtString(buffer, obj.LineupName);
                WriteBool(buffer, obj.IsReceived);
            }

            public override S2CGpGpShopDisplayBuyRes Read(IBuffer buffer)
            {
                S2CGpGpShopDisplayBuyRes obj = new S2CGpGpShopDisplayBuyRes();
                ReadServerResponse(buffer, obj);
                obj.LineupId = ReadUInt32(buffer);
                obj.CommonIdList = ReadEntityList<CDataCommonU32>(buffer);
                obj.Balance = ReadUInt32(buffer);
                obj.LineupName = ReadMtString(buffer);
                obj.IsReceived = ReadBool(buffer);
                return obj;
            }
        }
    }
}
