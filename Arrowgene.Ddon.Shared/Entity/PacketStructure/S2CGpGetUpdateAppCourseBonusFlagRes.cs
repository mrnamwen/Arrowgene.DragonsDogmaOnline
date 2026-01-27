using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CGpGetUpdateAppCourseBonusFlagRes : ServerResponse
    {
        public S2CGpGetUpdateAppCourseBonusFlagRes()
        {
            // Default to no update needed
            UpdateFlag = false;
        }

        public override PacketId Id => PacketId.S2C_GP_GET_UPDATE_APP_COURSE_BONUS_FLAG_RES;

        /// <summary>
        /// Flag indicating whether the client needs to update app course bonus info.
        /// In retail, this would indicate if GP course bonuses have changed since last check.
        /// </summary>
        public bool UpdateFlag { get; set; }

        public class Serializer : PacketEntitySerializer<S2CGpGetUpdateAppCourseBonusFlagRes>
        {
            public override void Write(IBuffer buffer, S2CGpGetUpdateAppCourseBonusFlagRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteBool(buffer, obj.UpdateFlag);
            }

            public override S2CGpGetUpdateAppCourseBonusFlagRes Read(IBuffer buffer)
            {
                S2CGpGetUpdateAppCourseBonusFlagRes obj = new S2CGpGetUpdateAppCourseBonusFlagRes();
                ReadServerResponse(buffer, obj);
                obj.UpdateFlag = ReadBool(buffer);
                return obj;
            }
        }
    }
}
