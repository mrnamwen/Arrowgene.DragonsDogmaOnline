using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    /// <summary>
    /// Response after selling an item to the NPC Bazaar.
    /// Uses the previously undefined BAZAAR_36_10_2 packet slot.
    /// </summary>
    public class S2CBazaarNpcSellRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_BAZAAR_36_10_2_RES;

        public uint GoldReceived { get; set; }

        public class Serializer : PacketEntitySerializer<S2CBazaarNpcSellRes>
        {
            public override void Write(IBuffer buffer, S2CBazaarNpcSellRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt32(buffer, obj.GoldReceived);
            }

            public override S2CBazaarNpcSellRes Read(IBuffer buffer)
            {
                S2CBazaarNpcSellRes obj = new S2CBazaarNpcSellRes();
                ReadServerResponse(buffer, obj);
                obj.GoldReceived = ReadUInt32(buffer);
                return obj;
            }
        }
    }
}
