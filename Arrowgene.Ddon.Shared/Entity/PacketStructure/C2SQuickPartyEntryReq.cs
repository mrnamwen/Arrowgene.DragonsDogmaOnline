using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SQuickPartyEntryReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_QUICK_PARTY_ENTRY_REQ;

        public C2SQuickPartyEntryReq()
        {
        }

        /// <summary>
        /// Whether player is confirming entry (true) or canceling (false)
        /// </summary>
        public bool IsEntry { get; set; }

        public class Serializer : PacketEntitySerializer<C2SQuickPartyEntryReq>
        {
            public override void Write(IBuffer buffer, C2SQuickPartyEntryReq obj)
            {
                WriteBool(buffer, obj.IsEntry);
            }

            public override C2SQuickPartyEntryReq Read(IBuffer buffer)
            {
                C2SQuickPartyEntryReq obj = new C2SQuickPartyEntryReq();
                obj.IsEntry = ReadBool(buffer);
                return obj;
            }
        }
    }
}
