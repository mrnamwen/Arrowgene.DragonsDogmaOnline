using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SGpGpShopDisplayGetLineupReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_GP_GP_SHOP_DISPLAY_GET_LINEUP_REQ;

        public uint CategoryId { get; set; }

        public C2SGpGpShopDisplayGetLineupReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SGpGpShopDisplayGetLineupReq>
        {
            public override void Write(IBuffer buffer, C2SGpGpShopDisplayGetLineupReq obj)
            {
                WriteUInt32(buffer, obj.CategoryId);
            }

            public override C2SGpGpShopDisplayGetLineupReq Read(IBuffer buffer)
            {
                C2SGpGpShopDisplayGetLineupReq obj = new C2SGpGpShopDisplayGetLineupReq();
                obj.CategoryId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
