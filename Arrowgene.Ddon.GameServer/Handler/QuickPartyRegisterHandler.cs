using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class QuickPartyRegisterHandler : GameRequestPacketHandler<C2SQuickPartyRegisterReq, S2CQuickPartyRegisterRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(QuickPartyRegisterHandler));

        public QuickPartyRegisterHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CQuickPartyRegisterRes Handle(GameClient client, C2SQuickPartyRegisterReq request)
        {
            // Check if player is already in a party
            if (client.Party != null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_PARTY_ALREADY_JOINED);
            }

            // Check if already registered
            if (Server.QuickPartyManager.IsCharacterInQueue(client.Character.CharacterId))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            var registration = Server.QuickPartyManager.RegisterForContent(client.Character.CharacterId, request.ContentId);
            if (registration == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            Logger.Info($"Character {client.Character.CharacterId} registered for quick party content {request.ContentId}");

            return new S2CQuickPartyRegisterRes
            {
                RegistrationId = registration.RegistrationId
            };
        }
    }
}
