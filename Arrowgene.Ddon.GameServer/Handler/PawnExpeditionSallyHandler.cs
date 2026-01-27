using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;
using System;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnExpeditionSallyHandler : GameRequestPacketHandler<C2SPawnExpeditionSallyReq, S2CPawnExpeditionSallyRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnExpeditionSallyHandler));

        public PawnExpeditionSallyHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CPawnExpeditionSallyRes Handle(GameClient client, C2SPawnExpeditionSallyReq request)
        {
            if (!Server.PawnExpeditionManager.StartExpedition(
                client.Character.CharacterId,
                request.PawnId,
                request.AreaId,
                request.SpotId,
                isGolden: false,
                out DateTime endTime))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            return new S2CPawnExpeditionSallyRes()
            {
                SallyCount = (byte)Server.PawnExpeditionManager.GetSallyCount(client.Character.CharacterId),
                EndTime = (ulong)((DateTimeOffset)endTime).ToUnixTimeSeconds()
            };
        }
    }
}
