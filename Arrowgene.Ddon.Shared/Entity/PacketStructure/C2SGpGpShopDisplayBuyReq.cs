using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    /// <summary>
    /// Request to buy from GP Display Shop.
    /// Note: The GP field contains the price sent by the client, not a quantity.
    /// </summary>
    public class C2SGpGpShopDisplayBuyReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_GP_GP_SHOP_DISPLAY_BUY_REQ;

        public uint LineupId { get; set; }
        public uint GP { get; set; }

        public C2SGpGpShopDisplayBuyReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SGpGpShopDisplayBuyReq>
        {
            public override void Write(IBuffer buffer, C2SGpGpShopDisplayBuyReq obj)
            {
                WriteUInt32(buffer, obj.LineupId);
                WriteUInt32(buffer, obj.GP);
            }

            public override C2SGpGpShopDisplayBuyReq Read(IBuffer buffer)
            {
                C2SGpGpShopDisplayBuyReq obj = new C2SGpGpShopDisplayBuyReq();
                obj.LineupId = ReadUInt32(buffer);
                obj.GP = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
