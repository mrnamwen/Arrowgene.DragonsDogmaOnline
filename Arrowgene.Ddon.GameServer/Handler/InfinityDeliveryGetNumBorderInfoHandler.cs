using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class InfinityDeliveryGetNumBorderInfoHandler : GameRequestPacketHandler<C2SInfinityDeliveryGetNumBorderInfoReq, S2CInfinityDeliveryGetNumBorderInfoRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(InfinityDeliveryGetNumBorderInfoHandler));

        public InfinityDeliveryGetNumBorderInfoHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CInfinityDeliveryGetNumBorderInfoRes Handle(GameClient client, C2SInfinityDeliveryGetNumBorderInfoReq request)
        {
            var res = new S2CInfinityDeliveryGetNumBorderInfoRes();
            res.BorderList = Server.InfinityDeliveryManager.GetBorders();
            return res;
        }
    }
}
