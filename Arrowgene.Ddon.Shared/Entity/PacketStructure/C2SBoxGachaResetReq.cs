using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SBoxGachaResetReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_BOX_GACHA_BOX_GACHA_RESET_REQ;

        public uint BoxGachaId { get; set; }

        public C2SBoxGachaResetReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SBoxGachaResetReq>
        {
            public override void Write(IBuffer buffer, C2SBoxGachaResetReq obj)
            {
                WriteUInt32(buffer, obj.BoxGachaId);
            }

            public override C2SBoxGachaResetReq Read(IBuffer buffer)
            {
                C2SBoxGachaResetReq obj = new C2SBoxGachaResetReq();
                obj.BoxGachaId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
