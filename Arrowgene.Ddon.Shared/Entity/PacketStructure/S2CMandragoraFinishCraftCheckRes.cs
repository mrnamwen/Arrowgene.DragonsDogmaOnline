using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CMandragoraFinishCraftCheckRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_MANDRAGORA_FINISH_CRAFT_CHECK_RES;

        /// <summary>
        /// Whether the craft is complete and ready to collect.
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// Mandragora cultivation/craft information.
        /// </summary>
        public CDataMyMandragoraBeginCraftResUnk0 CraftInfo { get; set; } = new();

        public S2CMandragoraFinishCraftCheckRes()
        {
        }

        public class Serializer : PacketEntitySerializer<S2CMandragoraFinishCraftCheckRes>
        {
            public override void Write(IBuffer buffer, S2CMandragoraFinishCraftCheckRes obj)
            {
                WriteServerResponse(buffer, obj);

                WriteBool(buffer, obj.IsComplete);
                WriteEntity<CDataMyMandragoraBeginCraftResUnk0>(buffer, obj.CraftInfo);
            }

            public override S2CMandragoraFinishCraftCheckRes Read(IBuffer buffer)
            {
                S2CMandragoraFinishCraftCheckRes obj = new S2CMandragoraFinishCraftCheckRes();

                ReadServerResponse(buffer, obj);

                obj.IsComplete = ReadBool(buffer);
                obj.CraftInfo = ReadEntity<CDataMyMandragoraBeginCraftResUnk0>(buffer);

                return obj;
            }
        }
    }
}
