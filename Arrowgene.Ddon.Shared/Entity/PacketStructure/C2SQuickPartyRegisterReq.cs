using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SQuickPartyRegisterReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_QUICK_PARTY_REGISTER_REQ;

        public C2SQuickPartyRegisterReq()
        {
        }

        /// <summary>
        /// Content type to register for (dungeon, mission, etc.)
        /// </summary>
        public uint ContentId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SQuickPartyRegisterReq>
        {
            public override void Write(IBuffer buffer, C2SQuickPartyRegisterReq obj)
            {
                WriteUInt32(buffer, obj.ContentId);
            }

            public override C2SQuickPartyRegisterReq Read(IBuffer buffer)
            {
                C2SQuickPartyRegisterReq obj = new C2SQuickPartyRegisterReq();
                obj.ContentId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
