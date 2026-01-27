using System.Collections.Generic;
using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CMandragoraFinishItemGetRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_MANDRAGORA_FINISH_ITEM_GET_RES;

        /// <summary>
        /// The new species index of the mandragora after cultivation.
        /// </summary>
        public uint NewSpeciesIndex { get; set; }

        /// <summary>
        /// Whether a new species was discovered.
        /// </summary>
        public bool IsNewSpeciesDiscovery { get; set; }

        /// <summary>
        /// Whether this character is the first to discover this species server-wide.
        /// </summary>
        public bool IsFirstDiscovery { get; set; }

        /// <summary>
        /// Items received from completing the craft.
        /// </summary>
        public List<CDataItemUpdateResult> ItemUpdateResults { get; set; } = new();

        public S2CMandragoraFinishItemGetRes()
        {
        }

        public class Serializer : PacketEntitySerializer<S2CMandragoraFinishItemGetRes>
        {
            public override void Write(IBuffer buffer, S2CMandragoraFinishItemGetRes obj)
            {
                WriteServerResponse(buffer, obj);

                WriteUInt32(buffer, obj.NewSpeciesIndex);
                WriteBool(buffer, obj.IsNewSpeciesDiscovery);
                WriteBool(buffer, obj.IsFirstDiscovery);
                WriteEntityList<CDataItemUpdateResult>(buffer, obj.ItemUpdateResults);
            }

            public override S2CMandragoraFinishItemGetRes Read(IBuffer buffer)
            {
                S2CMandragoraFinishItemGetRes obj = new S2CMandragoraFinishItemGetRes();

                ReadServerResponse(buffer, obj);

                obj.NewSpeciesIndex = ReadUInt32(buffer);
                obj.IsNewSpeciesDiscovery = ReadBool(buffer);
                obj.IsFirstDiscovery = ReadBool(buffer);
                obj.ItemUpdateResults = ReadEntityList<CDataItemUpdateResult>(buffer);

                return obj;
            }
        }
    }
}
