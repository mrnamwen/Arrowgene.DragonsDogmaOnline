using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Server.Network;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class InfinityDeliveryDeliverItemHandler : GameRequestPacketQueueHandler<C2SInfinityDeliveryDeliverItemReq, S2CInfinityDeliveryDeliverItemRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(InfinityDeliveryDeliverItemHandler));

        public InfinityDeliveryDeliverItemHandler(DdonGameServer server) : base(server)
        {
        }

        public override PacketQueue Handle(GameClient client, C2SInfinityDeliveryDeliverItemReq request)
        {
            PacketQueue packets = new();

            uint pointsEarned = Server.InfinityDeliveryManager.CalculatePointsEarned(client, request.CategoryId, request.DeliverItems);

            var deliverQueue = Server.InfinityDeliveryManager.DeliverItems(client, request.CategoryId, request.DeliverItems);
            packets.AddRange(deliverQueue);

            var res = new S2CInfinityDeliveryDeliverItemRes();
            res.PointsEarned = pointsEarned;
            res.Status = Server.InfinityDeliveryManager.GetPlayerStatus(client);
            packets.Enqueue(client, res);

            return packets;
        }
    }
}
