using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    /// <summary>
    /// Notification sent when quick party registration is canceled.
    /// </summary>
    public class S2CQuickPartyCancelNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUICK_PARTY_CANCEL_NTC;

        public S2CQuickPartyCancelNtc()
        {
        }

        /// <summary>
        /// Reason for cancellation (0 = user canceled, 1 = timeout, 2 = error)
        /// </summary>
        public uint CancelReason { get; set; }

        public class Serializer : PacketEntitySerializer<S2CQuickPartyCancelNtc>
        {
            public override void Write(IBuffer buffer, S2CQuickPartyCancelNtc obj)
            {
                WriteUInt32(buffer, obj.CancelReason);
            }

            public override S2CQuickPartyCancelNtc Read(IBuffer buffer)
            {
                S2CQuickPartyCancelNtc obj = new S2CQuickPartyCancelNtc();
                obj.CancelReason = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
