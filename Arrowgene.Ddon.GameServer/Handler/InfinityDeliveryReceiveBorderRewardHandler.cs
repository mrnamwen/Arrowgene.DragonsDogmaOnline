using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Server.Network;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class InfinityDeliveryReceiveBorderRewardHandler : GameRequestPacketQueueHandler<C2SInfinityDeliveryReceiveBorderRewardReq, S2CInfinityDeliveryReceiveBorderRewardRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(InfinityDeliveryReceiveBorderRewardHandler));

        public InfinityDeliveryReceiveBorderRewardHandler(DdonGameServer server) : base(server)
        {
        }

        public override PacketQueue Handle(GameClient client, C2SInfinityDeliveryReceiveBorderRewardReq request)
        {
            PacketQueue packets = new();

            var rewardQueue = Server.InfinityDeliveryManager.ClaimBorderReward(client, request.BorderId);
            packets.AddRange(rewardQueue);

            var res = new S2CInfinityDeliveryReceiveBorderRewardRes();
            res.Status = Server.InfinityDeliveryManager.GetPlayerStatus(client);
            packets.Enqueue(client, res);

            return packets;
        }
    }
}
