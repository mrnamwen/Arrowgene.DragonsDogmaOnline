#nullable enable
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class GpGpShopDisplayBuyHandler : GameRequestPacketHandler<C2SGpGpShopDisplayBuyReq, S2CGpGpShopDisplayBuyRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(GpGpShopDisplayBuyHandler));

        public GpGpShopDisplayBuyHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CGpGpShopDisplayBuyRes Handle(GameClient client, C2SGpGpShopDisplayBuyReq request)
        {
            var item = Server.GpShopManager.GetItem(request.LineupId);
            if (item == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_GP_INTERNAL_ERROR, "Item not found.");
            }

            // GP Display Shop always buys 1 lineup item at a time
            // Note: request.Quantity actually contains the GP price from the client, not the quantity
            uint quantity = 1;

            if (!Server.GpShopManager.ProcessPurchase(client, request.LineupId, quantity, out var itemNtc))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_GP_INTERNAL_ERROR, "Purchase failed.");
            }

            client.Send(itemNtc);

            // Get remaining GP balance after purchase
            uint balance = Server.WalletManager.GetWalletAmount(client.Character, WalletType.GoldenGemstones);

            var res = new S2CGpGpShopDisplayBuyRes
            {
                LineupId = request.LineupId,
                Balance = balance,
                LineupName = item.Name,
                IsReceived = true
            };

            return res;
        }
    }
}
