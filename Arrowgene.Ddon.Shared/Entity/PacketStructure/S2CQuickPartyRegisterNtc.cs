using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    /// <summary>
    /// Notification sent when a match is found for a quick party registration.
    /// </summary>
    public class S2CQuickPartyRegisterNtc : IPacketStructure
    {
        public PacketId Id => PacketId.S2C_QUICK_PARTY_REGISTER_NTC;

        public S2CQuickPartyRegisterNtc()
        {
        }

        /// <summary>
        /// Content ID that was matched
        /// </summary>
        public uint ContentId { get; set; }

        /// <summary>
        /// Number of players in the matched group
        /// </summary>
        public uint MemberCount { get; set; }

        public class Serializer : PacketEntitySerializer<S2CQuickPartyRegisterNtc>
        {
            public override void Write(IBuffer buffer, S2CQuickPartyRegisterNtc obj)
            {
                WriteUInt32(buffer, obj.ContentId);
                WriteUInt32(buffer, obj.MemberCount);
            }

            public override S2CQuickPartyRegisterNtc Read(IBuffer buffer)
            {
                S2CQuickPartyRegisterNtc obj = new S2CQuickPartyRegisterNtc();
                obj.ContentId = ReadUInt32(buffer);
                obj.MemberCount = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
