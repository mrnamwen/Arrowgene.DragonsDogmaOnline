using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnExpeditionGetSallyRewardHandler : GameRequestPacketHandler<C2SPawnExpeditionGetSallyRewardReq, S2CPawnExpeditionGetSallyRewardRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnExpeditionGetSallyRewardHandler));

        public PawnExpeditionGetSallyRewardHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CPawnExpeditionGetSallyRewardRes Handle(GameClient client, C2SPawnExpeditionGetSallyRewardReq request)
        {
            var rewards = Server.PawnExpeditionManager.GetExpeditionRewards(
                client.Character.CharacterId,
                request.PawnId,
                out bool discoveredHotSpot,
                out CDataAreaSpotSet discoveredSpotInfo);

            if (discoveredHotSpot && discoveredSpotInfo != null)
            {
                Logger.Info($"Character {client.Character.CharacterId} discovered hot spot in {discoveredSpotInfo.AreaId}");
            }

            return new S2CPawnExpeditionGetSallyRewardRes()
            {
                RewardList = rewards
            };
        }
    }
}
