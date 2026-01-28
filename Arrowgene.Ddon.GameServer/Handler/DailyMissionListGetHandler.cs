using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class DailyMissionListGetHandler : GameRequestPacketHandler<C2SDailyMissionListGetReq, S2CDailyMissionListGetRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(DailyMissionListGetHandler));

        public DailyMissionListGetHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CDailyMissionListGetRes Handle(GameClient client, C2SDailyMissionListGetReq request)
        {
            var res = new S2CDailyMissionListGetRes();
            //res.MissionList = Server.RewardMissionManager.GetMissionInfoList(client);
            return res;
        }
    }
}
