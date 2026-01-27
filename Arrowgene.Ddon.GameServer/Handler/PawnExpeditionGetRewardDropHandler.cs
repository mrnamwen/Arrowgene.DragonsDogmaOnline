using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnExpeditionGetRewardDropHandler : GameRequestPacketHandler<C2SPawnExpeditionGetRewardDropReq, S2CPawnExpeditionGetRewardDropRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnExpeditionGetRewardDropHandler));

        public PawnExpeditionGetRewardDropHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CPawnExpeditionGetRewardDropRes Handle(GameClient client, C2SPawnExpeditionGetRewardDropReq request)
        {
            return new S2CPawnExpeditionGetRewardDropRes()
            {
                HasReward = Server.PawnExpeditionManager.HasUnclaimedRewards(client.Character.CharacterId)
            };
        }
    }
}
