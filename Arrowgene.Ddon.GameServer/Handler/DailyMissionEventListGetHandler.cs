using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class DailyMissionEventListGetHandler : GameRequestPacketHandler<C2SDailyMissionEventListGetReq, S2CDailyMissionEventListGetRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(DailyMissionEventListGetHandler));

        public DailyMissionEventListGetHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CDailyMissionEventListGetRes Handle(GameClient client, C2SDailyMissionEventListGetReq request)
        {
            // Return empty event mission list for now
            return new S2CDailyMissionEventListGetRes();
        }
    }
}
