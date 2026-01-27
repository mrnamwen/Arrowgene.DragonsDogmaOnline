using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CMailMailSendRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_MAIL_MAIL_SEND_RES;

        public S2CMailMailSendRes()
        {
        }

        public class Serializer : PacketEntitySerializer<S2CMailMailSendRes>
        {
            public override void Write(IBuffer buffer, S2CMailMailSendRes obj)
            {
                WriteServerResponse(buffer, obj);
            }

            public override S2CMailMailSendRes Read(IBuffer buffer)
            {
                S2CMailMailSendRes obj = new S2CMailMailSendRes();
                ReadServerResponse(buffer, obj);
                return obj;
            }
        }
    }
}
