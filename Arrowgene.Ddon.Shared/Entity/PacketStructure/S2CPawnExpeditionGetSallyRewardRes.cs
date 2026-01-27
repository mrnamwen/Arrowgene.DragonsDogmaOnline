using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CPawnExpeditionGetSallyRewardRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_PAWN_EXPEDITION_PAWN_EXPEDITION_GET_SALLY_REWARD_RES;

        public S2CPawnExpeditionGetSallyRewardRes()
        {
            RewardList = new List<CDataPawnExpeditionReward>();
        }

        public List<CDataPawnExpeditionReward> RewardList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CPawnExpeditionGetSallyRewardRes>
        {
            public override void Write(IBuffer buffer, S2CPawnExpeditionGetSallyRewardRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList(buffer, obj.RewardList);
            }

            public override S2CPawnExpeditionGetSallyRewardRes Read(IBuffer buffer)
            {
                S2CPawnExpeditionGetSallyRewardRes obj = new S2CPawnExpeditionGetSallyRewardRes();
                ReadServerResponse(buffer, obj);
                obj.RewardList = ReadEntityList<CDataPawnExpeditionReward>(buffer);
                return obj;
            }
        }
    }
}
