using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnExpeditionGetMySallyInfoHandler : GameRequestPacketHandler<C2SPawnExpeditionGetMySallyInfoReq, S2CPawnExpeditionGetMySallyInfoRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnExpeditionGetMySallyInfoHandler));

        public PawnExpeditionGetMySallyInfoHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CPawnExpeditionGetMySallyInfoRes Handle(GameClient client, C2SPawnExpeditionGetMySallyInfoReq request)
        {
            return new S2CPawnExpeditionGetMySallyInfoRes()
            {
                PawnExpeditionList = Server.PawnExpeditionManager.GetMyExpeditions(client.Character.CharacterId)
            };
        }
    }
}
