using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SFriendGetRecentCharacterListReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_FRIEND_GET_RECENT_CHARACTER_LIST_REQ;

        public class Serializer : PacketEntitySerializer<C2SFriendGetRecentCharacterListReq>
        {
            public override void Write(IBuffer buffer, C2SFriendGetRecentCharacterListReq obj)
            {
            }

            public override C2SFriendGetRecentCharacterListReq Read(IBuffer buffer)
            {
                return new C2SFriendGetRecentCharacterListReq();
            }
        }
    }
}
