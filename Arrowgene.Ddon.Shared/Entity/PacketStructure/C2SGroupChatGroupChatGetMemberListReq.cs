using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SGroupChatGroupChatGetMemberListReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_GROUP_CHAT_GROUP_CHAT_GET_MEMBER_LIST_REQ;

        /// <summary>
        /// The group chat ID to get members for
        /// </summary>
        public ulong GroupId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SGroupChatGroupChatGetMemberListReq>
        {
            public override void Write(IBuffer buffer, C2SGroupChatGroupChatGetMemberListReq obj)
            {
                WriteUInt64(buffer, obj.GroupId);
            }

            public override C2SGroupChatGroupChatGetMemberListReq Read(IBuffer buffer)
            {
                C2SGroupChatGroupChatGetMemberListReq obj = new C2SGroupChatGroupChatGetMemberListReq();
                obj.GroupId = ReadUInt64(buffer);
                return obj;
            }
        }
    }
}
