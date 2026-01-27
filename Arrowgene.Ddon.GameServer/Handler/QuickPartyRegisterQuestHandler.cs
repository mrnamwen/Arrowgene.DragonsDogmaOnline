using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class QuickPartyRegisterQuestHandler : GameRequestPacketHandler<C2SQuickPartyRegisterQuestReq, S2CQuickPartyRegisterQuestRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(QuickPartyRegisterQuestHandler));

        public QuickPartyRegisterQuestHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CQuickPartyRegisterQuestRes Handle(GameClient client, C2SQuickPartyRegisterQuestReq request)
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

            var registration = Server.QuickPartyManager.RegisterForQuest(client.Character.CharacterId, request.QuestScheduleId);
            if (registration == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            Logger.Info($"Character {client.Character.CharacterId} registered for quick party quest {request.QuestScheduleId}");

            return new S2CQuickPartyRegisterQuestRes
            {
                RegistrationId = registration.RegistrationId
            };
        }
    }
}
