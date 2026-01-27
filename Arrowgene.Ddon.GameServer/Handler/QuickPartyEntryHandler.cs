using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class QuickPartyEntryHandler : GameRequestPacketHandler<C2SQuickPartyEntryReq, S2CQuickPartyEntryRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(QuickPartyEntryHandler));

        public QuickPartyEntryHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CQuickPartyEntryRes Handle(GameClient client, C2SQuickPartyEntryReq request)
        {
            // Check if player is in a match
            var match = Server.QuickPartyManager.GetMatch(client.Character.CharacterId);
            if (match == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            if (!Server.QuickPartyManager.ConfirmEntry(client.Character.CharacterId, request.IsEntry))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            if (request.IsEntry)
            {
                Logger.Info($"Character {client.Character.CharacterId} confirmed quick party entry");
            }
            else
            {
                Logger.Info($"Character {client.Character.CharacterId} declined quick party entry");
            }

            return new S2CQuickPartyEntryRes();
        }
    }
}
