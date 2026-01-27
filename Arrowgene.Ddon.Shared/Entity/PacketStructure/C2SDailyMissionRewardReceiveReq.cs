using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SDailyMissionRewardReceiveReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_DAILY_MISSION_REWARD_RECEIVE_REQ;

        public C2SDailyMissionRewardReceiveReq()
        {
        }

        public uint MissionId { get; set; }

        public class Serializer : PacketEntitySerializer<C2SDailyMissionRewardReceiveReq>
        {
            public override void Write(IBuffer buffer, C2SDailyMissionRewardReceiveReq obj)
            {
                WriteUInt32(buffer, obj.MissionId);
            }

            public override C2SDailyMissionRewardReceiveReq Read(IBuffer buffer)
            {
                C2SDailyMissionRewardReceiveReq obj = new C2SDailyMissionRewardReceiveReq();
                obj.MissionId = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
