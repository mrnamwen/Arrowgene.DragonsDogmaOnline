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
            // TODO: Fix daily missions causing client disconnect.
            // The CDataDailyMissionInfo serialization order doesn't match the original game.
            // Based on packet dump analysis (Dump_119), the correct structure appears to be:
            // 1. MissionId, Category, Type, TargetCount, CurrentCount (5 x uint32)
            // 2. Unknown fields (4 x uint32)
            // 3. RewardList (comes BEFORE SortId/strings, not after)
            // 4. SortId and more unknown fields (4 x uint32)
            // 5. Title, IconUrl, ImageUrl (MtString)
            // 6. IsComplete, IsReceived flags
            // 7. StartTime, EndTime (uint32, not uint64)
            // 8. Additional unknown fields
            // See GameFullDump.cs Dump_119 for reference packet data.
            // res.MissionList = Server.RewardMissionManager.GetMissionInfoList(client);
            return res;
        }
    }
}
