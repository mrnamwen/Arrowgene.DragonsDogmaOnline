using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CPawnExpeditionGetRewardDropRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_PAWN_EXPEDITION_PAWN_EXPEDITION_GET_REWARD_DROP_RES;

        public S2CPawnExpeditionGetRewardDropRes()
        {
        }

        public bool HasReward { get; set; }

        public class Serializer : PacketEntitySerializer<S2CPawnExpeditionGetRewardDropRes>
        {
            public override void Write(IBuffer buffer, S2CPawnExpeditionGetRewardDropRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteBool(buffer, obj.HasReward);
            }

            public override S2CPawnExpeditionGetRewardDropRes Read(IBuffer buffer)
            {
                S2CPawnExpeditionGetRewardDropRes obj = new S2CPawnExpeditionGetRewardDropRes();
                ReadServerResponse(buffer, obj);
                obj.HasReward = ReadBool(buffer);
                return obj;
            }
        }
    }
}
