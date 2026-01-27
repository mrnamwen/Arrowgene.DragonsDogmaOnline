#nullable enable
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class GpGpShopCanBuyHandler : GameRequestPacketHandler<C2SGpGpShopCanBuyReq, S2CGpGpShopCanBuyRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(GpGpShopCanBuyHandler));

        public GpGpShopCanBuyHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CGpGpShopCanBuyRes Handle(GameClient client, C2SGpGpShopCanBuyReq request)
        {
            var item = Server.GpShopManager.GetItem(request.LineupId);
            bool canBuy = Server.GpShopManager.CanBuyItem(client.Character, request.LineupId, 1);

            var res = new S2CGpGpShopCanBuyRes
            {
                CanBuy = canBuy,
                LineupId = request.LineupId,
                GP = item?.Price ?? 0,
                DiscountType = 0,
                DiscountGP = 0,
                ExpireDateTime = 0,
                Unk0 = 0,
                Unk1 = true,
                Unk2 = true
            };

            Logger.Info($"Player {client.Character.CharacterId} checked if can buy LineupId {request.LineupId}: {canBuy}");

            return res;
        }
    }
}
