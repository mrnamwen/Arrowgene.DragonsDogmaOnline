using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnExpeditionChargeSallyCountHandler : GameRequestPacketHandler<C2SPawnExpeditionChargeSallyCountReq, S2CPawnExpeditionChargeSallyCountRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnExpeditionChargeSallyCountHandler));

        public PawnExpeditionChargeSallyCountHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CPawnExpeditionChargeSallyCountRes Handle(GameClient client, C2SPawnExpeditionChargeSallyCountReq request)
        {
            // Deduct golden gems for recharging sally count
            bool walletResult = Server.WalletManager.RemoveFromWalletNtc(client, client.Character, WalletType.GoldenGemstones, PawnExpeditionManager.CHARGE_SALLY_PRICE);
            if (!walletResult)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            if (!Server.PawnExpeditionManager.ChargeSallyCount(client.Character.CharacterId, out int newSallyCount))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            return new S2CPawnExpeditionChargeSallyCountRes()
            {
                SallyCount = (byte)newSallyCount
            };
        }
    }
}
