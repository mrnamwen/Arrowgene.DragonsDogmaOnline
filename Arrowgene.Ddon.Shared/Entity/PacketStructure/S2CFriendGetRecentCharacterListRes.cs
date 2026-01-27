using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CFriendGetRecentCharacterListRes : ServerResponse
    {
        public S2CFriendGetRecentCharacterListRes()
        {
            RecentCharacterList = new List<CDataCommunityCharacterBaseInfo>();
        }

        public override PacketId Id => PacketId.S2C_FRIEND_GET_RECENT_CHARACTER_LIST_RES;

        /// <summary>
        /// List of recently encountered characters (players met in parties, etc.)
        /// </summary>
        public List<CDataCommunityCharacterBaseInfo> RecentCharacterList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CFriendGetRecentCharacterListRes>
        {
            public override void Write(IBuffer buffer, S2CFriendGetRecentCharacterListRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList(buffer, obj.RecentCharacterList);
            }

            public override S2CFriendGetRecentCharacterListRes Read(IBuffer buffer)
            {
                S2CFriendGetRecentCharacterListRes obj = new S2CFriendGetRecentCharacterListRes();
                ReadServerResponse(buffer, obj);
                obj.RecentCharacterList = ReadEntityList<CDataCommunityCharacterBaseInfo>(buffer);
                return obj;
            }
        }
    }
}
