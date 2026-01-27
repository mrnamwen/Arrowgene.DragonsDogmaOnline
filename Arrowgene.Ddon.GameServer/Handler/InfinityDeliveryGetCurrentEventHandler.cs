using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class InfinityDeliveryGetCurrentEventHandler : GameRequestPacketHandler<C2SInfinityDeliveryGetCurrentEventReq, S2CInfinityDeliveryGetCurrentEventRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(InfinityDeliveryGetCurrentEventHandler));

        public InfinityDeliveryGetCurrentEventHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CInfinityDeliveryGetCurrentEventRes Handle(GameClient client, C2SInfinityDeliveryGetCurrentEventReq request)
        {
            var res = new S2CInfinityDeliveryGetCurrentEventRes();

            res.IsActive = Server.InfinityDeliveryManager.IsEventEnabled();
            res.EventId = Server.InfinityDeliveryManager.GetEventId();

            if (res.IsActive)
            {
                var (startTime, endTime) = Server.InfinityDeliveryManager.GetEventTimes();
                res.StartTime = startTime;
                res.EndTime = endTime;
            }

            return res;
        }
    }
}
