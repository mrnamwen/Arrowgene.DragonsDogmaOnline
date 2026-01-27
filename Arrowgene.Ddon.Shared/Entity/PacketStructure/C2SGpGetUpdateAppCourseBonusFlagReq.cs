using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SGpGetUpdateAppCourseBonusFlagReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_GP_GET_UPDATE_APP_COURSE_BONUS_FLAG_REQ;

        public class Serializer : PacketEntitySerializer<C2SGpGetUpdateAppCourseBonusFlagReq>
        {
            public override void Write(IBuffer buffer, C2SGpGetUpdateAppCourseBonusFlagReq obj)
            {
            }

            public override C2SGpGetUpdateAppCourseBonusFlagReq Read(IBuffer buffer)
            {
                return new C2SGpGetUpdateAppCourseBonusFlagReq();
            }
        }
    }
}
