using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Server.Network;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class DailyMissionRewardReceiveHandler : GameRequestPacketQueueHandler<C2SDailyMissionRewardReceiveReq, S2CDailyMissionRewardReceiveRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(DailyMissionRewardReceiveHandler));

        public DailyMissionRewardReceiveHandler(DdonGameServer server) : base(server)
        {
        }

        public override PacketQueue Handle(GameClient client, C2SDailyMissionRewardReceiveReq request)
        {
            PacketQueue packets = new();

            // Claim the reward (this enqueues item notifications)
            var rewardQueue = Server.RewardMissionManager.ClaimMissionReward(client, request.MissionId);
            packets.AddRange(rewardQueue);

            // Return updated mission list
            var res = new S2CDailyMissionRewardReceiveRes();
            res.MissionList = Server.RewardMissionManager.GetMissionInfoList(client);
            packets.Enqueue(client, res);

            return packets;
        }
    }
}
