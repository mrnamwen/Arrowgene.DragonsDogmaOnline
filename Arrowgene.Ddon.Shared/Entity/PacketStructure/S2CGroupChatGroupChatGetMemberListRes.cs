using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CGroupChatGroupChatGetMemberListRes : ServerResponse
    {
        public S2CGroupChatGroupChatGetMemberListRes()
        {
            MemberList = new List<CDataCommunityCharacterBaseInfo>();
        }

        public override PacketId Id => PacketId.S2C_GROUP_CHAT_GROUP_CHAT_GET_MEMBER_LIST_RES;

        /// <summary>
        /// The group chat ID this response is for
        /// </summary>
        public ulong GroupId { get; set; }

        /// <summary>
        /// List of members in the group chat
        /// </summary>
        public List<CDataCommunityCharacterBaseInfo> MemberList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CGroupChatGroupChatGetMemberListRes>
        {
            public override void Write(IBuffer buffer, S2CGroupChatGroupChatGetMemberListRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt64(buffer, obj.GroupId);
                WriteEntityList(buffer, obj.MemberList);
            }

            public override S2CGroupChatGroupChatGetMemberListRes Read(IBuffer buffer)
            {
                S2CGroupChatGroupChatGetMemberListRes obj = new S2CGroupChatGroupChatGetMemberListRes();
                ReadServerResponse(buffer, obj);
                obj.GroupId = ReadUInt64(buffer);
                obj.MemberList = ReadEntityList<CDataCommunityCharacterBaseInfo>(buffer);
                return obj;
            }
        }
    }
}
