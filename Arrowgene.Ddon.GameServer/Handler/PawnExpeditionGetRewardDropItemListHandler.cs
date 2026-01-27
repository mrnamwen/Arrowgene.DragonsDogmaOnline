using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnExpeditionGetRewardDropItemListHandler : GameRequestPacketHandler<C2SPawnExpeditionGetRewardDropItemListReq, S2CPawnExpeditionGetRewardDropItemListRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnExpeditionGetRewardDropItemListHandler));

        public PawnExpeditionGetRewardDropItemListHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CPawnExpeditionGetRewardDropItemListRes Handle(GameClient client, C2SPawnExpeditionGetRewardDropItemListReq request)
        {
            return new S2CPawnExpeditionGetRewardDropItemListRes()
            {
                RewardList = Server.PawnExpeditionManager.GetUnclaimedRewards(client.Character.CharacterId)
            };
        }
    }
}
