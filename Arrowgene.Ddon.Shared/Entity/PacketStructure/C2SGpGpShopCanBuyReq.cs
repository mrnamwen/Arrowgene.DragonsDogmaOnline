using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SGpGpShopCanBuyReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_GP_GP_SHOP_CAN_BUY_REQ;

        public uint LineupId { get; set; }

        public C2SGpGpShopCanBuyReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SGpGpShopCanBuyReq>
        {
            public override void Write(IBuffer buffer, C2SGpGpShopCanBuyReq obj)
            {
                WriteUInt32(buffer, obj.LineupId);
            }

            public override C2SGpGpShopCanBuyReq Read(IBuffer buffer)
            {
                C2SGpGpShopCanBuyReq obj = new C2SGpGpShopCanBuyReq();
                obj.LineupId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
