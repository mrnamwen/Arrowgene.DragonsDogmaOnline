#nullable enable
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class GpGpShopDisplayGetLineupHandler : GameRequestPacketHandler<C2SGpGpShopDisplayGetLineupReq, S2CGpGpShopDisplayGetLineupRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(GpGpShopDisplayGetLineupHandler));

        public GpGpShopDisplayGetLineupHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CGpGpShopDisplayGetLineupRes Handle(GameClient client, C2SGpGpShopDisplayGetLineupReq request)
        {
            var res = new S2CGpGpShopDisplayGetLineupRes();

            res.LineupList = Server.GpShopManager.GetDisplayLineupForCategory(request.CategoryId);

            Logger.Info($"Player {client.Character.CharacterId} requested GP Shop lineup for category {request.CategoryId}, returned {res.LineupList.Count} items.");

            return res;
        }
    }
}
