using System.Collections.Generic;
using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CMandragoraChangeGoldenCraftRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_MANDRAGORA_CHANGE_GOLDEN_CRAFT_RES;

        /// <summary>
        /// The new species index of the mandragora after instant cultivation.
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

        /// <summary>
        /// Wallet point updates (GG deduction).
        /// </summary>
        public List<CDataWalletPoint> WalletPoints { get; set; } = new();

        public S2CMandragoraChangeGoldenCraftRes()
        {
        }

        public class Serializer : PacketEntitySerializer<S2CMandragoraChangeGoldenCraftRes>
        {
            public override void Write(IBuffer buffer, S2CMandragoraChangeGoldenCraftRes obj)
            {
                WriteServerResponse(buffer, obj);

                WriteUInt32(buffer, obj.NewSpeciesIndex);
                WriteBool(buffer, obj.IsNewSpeciesDiscovery);
                WriteBool(buffer, obj.IsFirstDiscovery);
                WriteEntityList<CDataItemUpdateResult>(buffer, obj.ItemUpdateResults);
                WriteEntityList<CDataWalletPoint>(buffer, obj.WalletPoints);
            }

            public override S2CMandragoraChangeGoldenCraftRes Read(IBuffer buffer)
            {
                S2CMandragoraChangeGoldenCraftRes obj = new S2CMandragoraChangeGoldenCraftRes();

                ReadServerResponse(buffer, obj);

                obj.NewSpeciesIndex = ReadUInt32(buffer);
                obj.IsNewSpeciesDiscovery = ReadBool(buffer);
                obj.IsFirstDiscovery = ReadBool(buffer);
                obj.ItemUpdateResults = ReadEntityList<CDataItemUpdateResult>(buffer);
                obj.WalletPoints = ReadEntityList<CDataWalletPoint>(buffer);

                return obj;
            }
        }
    }
}
