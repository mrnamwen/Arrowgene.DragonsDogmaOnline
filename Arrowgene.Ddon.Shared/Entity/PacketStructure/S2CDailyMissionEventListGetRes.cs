using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CDailyMissionEventListGetRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_DAILY_MISSION_EVENT_LIST_GET_RES;

        public S2CDailyMissionEventListGetRes()
        {
            MissionList = new List<CDataDailyMissionInfo>();
        }

        public List<CDataDailyMissionInfo> MissionList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CDailyMissionEventListGetRes>
        {
            public override void Write(IBuffer buffer, S2CDailyMissionEventListGetRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList<CDataDailyMissionInfo>(buffer, obj.MissionList);
            }

            public override S2CDailyMissionEventListGetRes Read(IBuffer buffer)
            {
                S2CDailyMissionEventListGetRes obj = new S2CDailyMissionEventListGetRes();
                ReadServerResponse(buffer, obj);
                obj.MissionList = ReadEntityList<CDataDailyMissionInfo>(buffer);
                return obj;
            }
        }
    }
}
