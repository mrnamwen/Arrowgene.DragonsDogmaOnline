using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnExpeditionChangeGoldenSallyHandler : GameRequestPacketHandler<C2SPawnExpeditionChangeGoldenSallyReq, S2CPawnExpeditionChangeGoldenSallyRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnExpeditionChangeGoldenSallyHandler));

        public PawnExpeditionChangeGoldenSallyHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CPawnExpeditionChangeGoldenSallyRes Handle(GameClient client, C2SPawnExpeditionChangeGoldenSallyReq request)
        {
            // Deduct golden gems for golden expedition upgrade
            bool walletResult = Server.WalletManager.RemoveFromWalletNtc(client, client.Character, WalletType.GoldenGemstones, PawnExpeditionManager.GOLDEN_SALLY_PRICE);
            if (!walletResult)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            if (!Server.PawnExpeditionManager.ChangeToGoldenExpedition(client.Character.CharacterId, request.PawnId))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            return new S2CPawnExpeditionChangeGoldenSallyRes();
        }
    }
}
