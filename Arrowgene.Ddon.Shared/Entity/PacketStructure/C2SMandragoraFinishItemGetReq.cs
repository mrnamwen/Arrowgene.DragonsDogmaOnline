using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SMandragoraFinishItemGetReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_MANDRAGORA_FINISH_ITEM_GET_REQ;

        /// <summary>
        /// The mandragora ID to collect the completed craft from.
        /// </summary>
        public byte MandragoraId { get; set; }

        public C2SMandragoraFinishItemGetReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SMandragoraFinishItemGetReq>
        {
            public override void Write(IBuffer buffer, C2SMandragoraFinishItemGetReq obj)
            {
                WriteByte(buffer, obj.MandragoraId);
            }

            public override C2SMandragoraFinishItemGetReq Read(IBuffer buffer)
            {
                C2SMandragoraFinishItemGetReq obj = new C2SMandragoraFinishItemGetReq();
                obj.MandragoraId = ReadByte(buffer);
                return obj;
            }
        }
    }
}
