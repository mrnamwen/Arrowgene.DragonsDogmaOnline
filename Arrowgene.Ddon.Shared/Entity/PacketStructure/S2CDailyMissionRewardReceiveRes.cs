using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CDailyMissionRewardReceiveRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_DAILY_MISSION_REWARD_RECEIVE_RES;

        public S2CDailyMissionRewardReceiveRes()
        {
            MissionList = new List<CDataDailyMissionInfo>();
        }

        /// <summary>
        /// Updated mission list after claiming the reward
        /// </summary>
        public List<CDataDailyMissionInfo> MissionList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CDailyMissionRewardReceiveRes>
        {
            public override void Write(IBuffer buffer, S2CDailyMissionRewardReceiveRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList<CDataDailyMissionInfo>(buffer, obj.MissionList);
            }

            public override S2CDailyMissionRewardReceiveRes Read(IBuffer buffer)
            {
                S2CDailyMissionRewardReceiveRes obj = new S2CDailyMissionRewardReceiveRes();
                ReadServerResponse(buffer, obj);
                obj.MissionList = ReadEntityList<CDataDailyMissionInfo>(buffer);
                return obj;
            }
        }
    }
}
