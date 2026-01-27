using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CDailyMissionListGetRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_DAILY_MISSION_LIST_GET_RES;

        public S2CDailyMissionListGetRes()
        {
            MissionList = new List<CDataDailyMissionInfo>();
        }

        public List<CDataDailyMissionInfo> MissionList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CDailyMissionListGetRes>
        {
            public override void Write(IBuffer buffer, S2CDailyMissionListGetRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList<CDataDailyMissionInfo>(buffer, obj.MissionList);
            }

            public override S2CDailyMissionListGetRes Read(IBuffer buffer)
            {
                S2CDailyMissionListGetRes obj = new S2CDailyMissionListGetRes();
                ReadServerResponse(buffer, obj);
                obj.MissionList = ReadEntityList<CDataDailyMissionInfo>(buffer);
                return obj;
            }
        }
    }
}
