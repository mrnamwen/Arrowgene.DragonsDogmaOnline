using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SMandragoraFinishCraftCheckReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_MANDRAGORA_FINISH_CRAFT_CHECK_REQ;

        /// <summary>
        /// The mandragora ID to check for craft completion.
        /// </summary>
        public byte MandragoraId { get; set; }

        public C2SMandragoraFinishCraftCheckReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SMandragoraFinishCraftCheckReq>
        {
            public override void Write(IBuffer buffer, C2SMandragoraFinishCraftCheckReq obj)
            {
                WriteByte(buffer, obj.MandragoraId);
            }

            public override C2SMandragoraFinishCraftCheckReq Read(IBuffer buffer)
            {
                C2SMandragoraFinishCraftCheckReq obj = new C2SMandragoraFinishCraftCheckReq();
                obj.MandragoraId = ReadByte(buffer);
                return obj;
            }
        }
    }
}
