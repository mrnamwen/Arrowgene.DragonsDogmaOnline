using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    /// <summary>
    /// Notification sent when the quick party is ready to start.
    /// </summary>
    public class S2CQuickPartyReadyNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUICK_PARTY_READY_NTC;

        public S2CQuickPartyReadyNtc()
        {
        }

        /// <summary>
        /// Party ID that was created
        /// </summary>
        public uint PartyId { get; set; }

        public class Serializer : PacketEntitySerializer<S2CQuickPartyReadyNtc>
        {
            public override void Write(IBuffer buffer, S2CQuickPartyReadyNtc obj)
            {
                WriteUInt32(buffer, obj.PartyId);
            }

            public override S2CQuickPartyReadyNtc Read(IBuffer buffer)
            {
                S2CQuickPartyReadyNtc obj = new S2CQuickPartyReadyNtc();
                obj.PartyId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
