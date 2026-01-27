using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CPawnExpeditionGetRewardDropItemListRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_PAWN_EXPEDITION_PAWN_EXPEDITION_GET_REWARD_DROP_ITEM_LIST_RES;

        public S2CPawnExpeditionGetRewardDropItemListRes()
        {
            RewardList = new List<CDataPawnExpeditionReward>();
        }

        public List<CDataPawnExpeditionReward> RewardList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CPawnExpeditionGetRewardDropItemListRes>
        {
            public override void Write(IBuffer buffer, S2CPawnExpeditionGetRewardDropItemListRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList(buffer, obj.RewardList);
            }

            public override S2CPawnExpeditionGetRewardDropItemListRes Read(IBuffer buffer)
            {
                S2CPawnExpeditionGetRewardDropItemListRes obj = new S2CPawnExpeditionGetRewardDropItemListRes();
                ReadServerResponse(buffer, obj);
                obj.RewardList = ReadEntityList<CDataPawnExpeditionReward>(buffer);
                return obj;
            }
        }
    }
}
