using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SEquipUpdateEquipPresetReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_EQUIP_UPDATE_EQUIP_PRESET_REQ;

        public C2SEquipUpdateEquipPresetReq()
        {
        }

        public byte PresetNo { get; set; }

        public class Serializer : PacketEntitySerializer<C2SEquipUpdateEquipPresetReq>
        {
            public override void Write(IBuffer buffer, C2SEquipUpdateEquipPresetReq obj)
            {
                WriteByte(buffer, obj.PresetNo);
            }

            public override C2SEquipUpdateEquipPresetReq Read(IBuffer buffer)
            {
                C2SEquipUpdateEquipPresetReq obj = new C2SEquipUpdateEquipPresetReq();
                obj.PresetNo = ReadByte(buffer);
                return obj;
            }
        }
    }
}
