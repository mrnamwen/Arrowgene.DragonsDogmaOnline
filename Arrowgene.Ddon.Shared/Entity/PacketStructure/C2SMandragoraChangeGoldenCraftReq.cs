using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SMandragoraChangeGoldenCraftReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_MANDRAGORA_CHANGE_GOLDEN_CRAFT_REQ;

        /// <summary>
        /// The mandragora ID to instantly complete cultivation.
        /// </summary>
        public byte MandragoraId { get; set; }

        public C2SMandragoraChangeGoldenCraftReq()
        {
        }

        public class Serializer : PacketEntitySerializer<C2SMandragoraChangeGoldenCraftReq>
        {
            public override void Write(IBuffer buffer, C2SMandragoraChangeGoldenCraftReq obj)
            {
                WriteByte(buffer, obj.MandragoraId);
            }

            public override C2SMandragoraChangeGoldenCraftReq Read(IBuffer buffer)
            {
                C2SMandragoraChangeGoldenCraftReq obj = new C2SMandragoraChangeGoldenCraftReq();
                obj.MandragoraId = ReadByte(buffer);
                return obj;
            }
        }
    }
}
