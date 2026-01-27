using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CQuickPartyRegisterQuestRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_QUICK_PARTY_REGISTER_QUEST_RES;

        public S2CQuickPartyRegisterQuestRes()
        {
        }

        /// <summary>
        /// Registration ID for tracking the queue entry
        /// </summary>
        public uint RegistrationId { get; set; }

        public class Serializer : PacketEntitySerializer<S2CQuickPartyRegisterQuestRes>
        {
            public override void Write(IBuffer buffer, S2CQuickPartyRegisterQuestRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt32(buffer, obj.RegistrationId);
            }

            public override S2CQuickPartyRegisterQuestRes Read(IBuffer buffer)
            {
                S2CQuickPartyRegisterQuestRes obj = new S2CQuickPartyRegisterQuestRes();
                ReadServerResponse(buffer, obj);
                obj.RegistrationId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
