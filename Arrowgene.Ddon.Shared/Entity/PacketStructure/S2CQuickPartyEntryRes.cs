using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuickPartyEntryRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_QUICK_PARTY_ENTRY_RES;

        public S2CQuickPartyEntryRes()
        {
        }

        public class Serializer : PacketEntitySerializer<S2CQuickPartyEntryRes>
        {
            public override void Write(IBuffer buffer, S2CQuickPartyEntryRes obj)
            {
                WriteServerResponse(buffer, obj);
            }

            public override S2CQuickPartyEntryRes Read(IBuffer buffer)
            {
                S2CQuickPartyEntryRes obj = new S2CQuickPartyEntryRes();
                ReadServerResponse(buffer, obj);
                return obj;
            }
        }
    }
}
