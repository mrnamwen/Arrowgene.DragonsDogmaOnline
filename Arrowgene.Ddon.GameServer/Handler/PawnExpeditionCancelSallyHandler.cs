using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnExpeditionCancelSallyHandler : GameRequestPacketHandler<C2SPawnExpeditionCancelSallyReq, S2CPawnExpeditionCancelSallyRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnExpeditionCancelSallyHandler));

        public PawnExpeditionCancelSallyHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CPawnExpeditionCancelSallyRes Handle(GameClient client, C2SPawnExpeditionCancelSallyReq request)
        {
            if (!Server.PawnExpeditionManager.CancelExpedition(client.Character.CharacterId, request.PawnId, out int newSallyCount))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            return new S2CPawnExpeditionCancelSallyRes()
            {
                SallyCount = (byte)newSallyCount
            };
        }
    }
}
