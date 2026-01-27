using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SQuickPartyRegisterQuestReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_QUICK_PARTY_REGISTER_QUEST_REQ;

        public C2SQuickPartyRegisterQuestReq()
        {
        }

        /// <summary>
        /// Quest schedule ID to register for
        /// </summary>
        public uint QuestScheduleId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SQuickPartyRegisterQuestReq>
        {
            public override void Write(IBuffer buffer, C2SQuickPartyRegisterQuestReq obj)
            {
                WriteUInt32(buffer, obj.QuestScheduleId);
            }

            public override C2SQuickPartyRegisterQuestReq Read(IBuffer buffer)
            {
                C2SQuickPartyRegisterQuestReq obj = new C2SQuickPartyRegisterQuestReq();
                obj.QuestScheduleId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
