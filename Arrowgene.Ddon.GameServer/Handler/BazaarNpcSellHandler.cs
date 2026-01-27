using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    /// <summary>
    /// Handler for selling items to the NPC Bazaar.
    /// Provides an instant-sell mechanism for solo players.
    /// </summary>
    public class BazaarNpcSellHandler : GameRequestPacketHandler<C2SBazaarNpcSellReq, S2CBazaarNpcSellRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(BazaarNpcSellHandler));

        public BazaarNpcSellHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CBazaarNpcSellRes Handle(GameClient client, C2SBazaarNpcSellReq request)
        {
            // Check if NPC Bazaar is enabled
            if (!Server.GameSettings.GameServerSettings.EnableNpcBazaar)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_BAZAAR_INTERNAL_ERROR, "NPC Bazaar is disabled.");
            }

            // Process the sale through NpcBazaarManager
            var updateNtc = Server.BazaarManager.NpcBazaar.ProcessNpcSale(
                client,
                request.StorageType,
                request.ItemUID,
                request.Num
            );

            // Send the item/wallet update notification
            client.Send(updateNtc);

            // Calculate the gold received for the response
            uint goldReceived = 0;
            foreach (var walletUpdate in updateNtc.UpdateWalletList)
            {
                if (walletUpdate.Type == Shared.Model.WalletType.Gold && walletUpdate.AddPoint > 0)
                {
                    goldReceived += (uint)walletUpdate.AddPoint;
                }
            }

            return new S2CBazaarNpcSellRes()
            {
                GoldReceived = goldReceived
            };
        }
    }
}
