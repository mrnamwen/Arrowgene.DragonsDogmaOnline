using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SMailMailSendReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_MAIL_MAIL_SEND_REQ;

        public C2SMailMailSendReq()
        {
            Title = string.Empty;
            Body = string.Empty;
        }

        public uint RecipientCharacterId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public class Serializer : PacketEntitySerializer<C2SMailMailSendReq>
        {
            public override void Write(IBuffer buffer, C2SMailMailSendReq obj)
            {
                WriteUInt32(buffer, obj.RecipientCharacterId);
                WriteMtString(buffer, obj.Title);
                WriteMtString(buffer, obj.Body);
            }

            public override C2SMailMailSendReq Read(IBuffer buffer)
            {
                C2SMailMailSendReq obj = new C2SMailMailSendReq();
                obj.RecipientCharacterId = ReadUInt32(buffer);
                obj.Title = ReadMtString(buffer);
                obj.Body = ReadMtString(buffer);
                return obj;
            }
        }
    }
}
