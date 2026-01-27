using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class InfinityDeliveryGetEventStatusHandler : GameRequestPacketHandler<C2SInfinityDeliveryGetEventStatusReq, S2CInfinityDeliveryGetEventStatusRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(InfinityDeliveryGetEventStatusHandler));

        public InfinityDeliveryGetEventStatusHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CInfinityDeliveryGetEventStatusRes Handle(GameClient client, C2SInfinityDeliveryGetEventStatusReq request)
        {
            Server.InfinityDeliveryManager.CheckWeeklyReset(client);

            var res = new S2CInfinityDeliveryGetEventStatusRes();
            res.Status = Server.InfinityDeliveryManager.GetPlayerStatus(client);
            return res;
        }
    }
}
