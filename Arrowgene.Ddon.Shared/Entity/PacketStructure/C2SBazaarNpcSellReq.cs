using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    /// <summary>
    /// Request to sell an item to the NPC Bazaar.
    /// Uses the previously undefined BAZAAR_36_10_1 packet slot.
    /// </summary>
    public class C2SBazaarNpcSellReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_BAZAAR_36_10_1_REQ;

        public C2SBazaarNpcSellReq()
        {
            ItemUID = string.Empty;
        }

        public StorageType StorageType { get; set; }
        public string ItemUID { get; set; }
        public ushort Num { get; set; }

        public class Serializer : PacketEntitySerializer<C2SBazaarNpcSellReq>
        {
            public override void Write(IBuffer buffer, C2SBazaarNpcSellReq obj)
            {
                WriteByte(buffer, (byte)obj.StorageType);
                WriteMtString(buffer, obj.ItemUID);
                WriteUInt16(buffer, obj.Num);
            }

            public override C2SBazaarNpcSellReq Read(IBuffer buffer)
            {
                C2SBazaarNpcSellReq obj = new C2SBazaarNpcSellReq();
                obj.StorageType = (StorageType)ReadByte(buffer);
                obj.ItemUID = ReadMtString(buffer);
                obj.Num = ReadUInt16(buffer);
                return obj;
            }
        }
    }
}
