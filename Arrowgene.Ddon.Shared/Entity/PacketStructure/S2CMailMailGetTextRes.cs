using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CMailMailGetTextRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_MAIL_MAIL_GET_TEXT_RES;

        public S2CMailMailGetTextRes()
        {
            MailTextInfo = new CDataMailTextInfo();
        }

        public ulong MailId { get; set; }
        public CDataMailTextInfo MailTextInfo { get; set; }

        public class Serializer : PacketEntitySerializer<S2CMailMailGetTextRes>
        {
            public override void Write(IBuffer buffer, S2CMailMailGetTextRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt64(buffer, obj.MailId);
                WriteEntity(buffer, obj.MailTextInfo);
            }

            public override S2CMailMailGetTextRes Read(IBuffer buffer)
            {
                S2CMailMailGetTextRes obj = new S2CMailMailGetTextRes();
                ReadServerResponse(buffer, obj);
                obj.MailId = ReadUInt64(buffer);
                obj.MailTextInfo = ReadEntity<CDataMailTextInfo>(buffer);
                return obj;
            }
        }
    }
}
