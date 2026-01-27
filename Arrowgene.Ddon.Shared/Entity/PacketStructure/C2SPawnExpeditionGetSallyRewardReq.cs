using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SPawnExpeditionGetSallyRewardReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_PAWN_EXPEDITION_PAWN_EXPEDITION_GET_SALLY_REWARD_REQ;

        public C2SPawnExpeditionGetSallyRewardReq()
        {
        }

        public uint PawnId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SPawnExpeditionGetSallyRewardReq>
        {
            public override void Write(IBuffer buffer, C2SPawnExpeditionGetSallyRewardReq obj)
            {
                WriteUInt32(buffer, obj.PawnId);
            }

            public override C2SPawnExpeditionGetSallyRewardReq Read(IBuffer buffer)
            {
                C2SPawnExpeditionGetSallyRewardReq obj = new C2SPawnExpeditionGetSallyRewardReq();
                obj.PawnId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
