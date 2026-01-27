using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuickPartyRegisterRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_QUICK_PARTY_REGISTER_RES;

        public S2CQuickPartyRegisterRes()
        {
        }

        /// <summary>
        /// Registration ID for tracking the queue entry
        /// </summary>
        public uint RegistrationId { get; set; }

        public class Serializer : PacketEntitySerializer<S2CQuickPartyRegisterRes>
        {
            public override void Write(IBuffer buffer, S2CQuickPartyRegisterRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt32(buffer, obj.RegistrationId);
            }

            public override S2CQuickPartyRegisterRes Read(IBuffer buffer)
            {
                S2CQuickPartyRegisterRes obj = new S2CQuickPartyRegisterRes();
                ReadServerResponse(buffer, obj);
                obj.RegistrationId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
