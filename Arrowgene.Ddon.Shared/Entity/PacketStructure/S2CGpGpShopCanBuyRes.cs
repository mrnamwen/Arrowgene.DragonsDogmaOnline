using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    /// <summary>
    /// Response for GP Shop can buy check.
    /// Based on PC client packet capture analysis.
    /// </summary>
    public class S2CGpGpShopCanBuyRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_GP_GP_SHOP_CAN_BUY_RES;

        public bool CanBuy { get; set; }
        public uint LineupId { get; set; }
        public uint GP { get; set; }
        public uint DiscountType { get; set; }
        public uint DiscountGP { get; set; }
        public ulong ExpireDateTime { get; set; }
        public uint Unk0 { get; set; }
        public bool Unk1 { get; set; }
        public bool Unk2 { get; set; }

        public S2CGpGpShopCanBuyRes()
        {
        }

        public class Serializer : PacketEntitySerializer<S2CGpGpShopCanBuyRes>
        {
            public override void Write(IBuffer buffer, S2CGpGpShopCanBuyRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteBool(buffer, obj.CanBuy);
                WriteUInt32(buffer, obj.LineupId);
                WriteUInt32(buffer, obj.GP);
                WriteUInt32(buffer, obj.DiscountType);
                WriteUInt32(buffer, obj.DiscountGP);
                WriteUInt64(buffer, obj.ExpireDateTime);
                WriteUInt32(buffer, obj.Unk0);
                WriteBool(buffer, obj.Unk1);
                WriteBool(buffer, obj.Unk2);
            }

            public override S2CGpGpShopCanBuyRes Read(IBuffer buffer)
            {
                S2CGpGpShopCanBuyRes obj = new S2CGpGpShopCanBuyRes();
                ReadServerResponse(buffer, obj);
                obj.CanBuy = ReadBool(buffer);
                obj.LineupId = ReadUInt32(buffer);
                obj.GP = ReadUInt32(buffer);
                obj.DiscountType = ReadUInt32(buffer);
                obj.DiscountGP = ReadUInt32(buffer);
                obj.ExpireDateTime = ReadUInt64(buffer);
                obj.Unk0 = ReadUInt32(buffer);
                obj.Unk1 = ReadBool(buffer);
                obj.Unk2 = ReadBool(buffer);
                return obj;
            }
        }
    }
}
