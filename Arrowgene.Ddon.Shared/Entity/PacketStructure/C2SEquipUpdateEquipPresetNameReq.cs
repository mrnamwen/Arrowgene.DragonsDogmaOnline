using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SEquipUpdateEquipPresetNameReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_EQUIP_UPDATE_EQUIP_PRESET_NAME_REQ;

        public C2SEquipUpdateEquipPresetNameReq()
        {
            PresetName = string.Empty;
        }

        public byte PresetNo { get; set; }
        public string PresetName { get; set; }

        public class Serializer : PacketEntitySerializer<C2SEquipUpdateEquipPresetNameReq>
        {
            public override void Write(IBuffer buffer, C2SEquipUpdateEquipPresetNameReq obj)
            {
                WriteByte(buffer, obj.PresetNo);
                WriteMtString(buffer, obj.PresetName);
            }

            public override C2SEquipUpdateEquipPresetNameReq Read(IBuffer buffer)
            {
                C2SEquipUpdateEquipPresetNameReq obj = new C2SEquipUpdateEquipPresetNameReq();
                obj.PresetNo = ReadByte(buffer);
                obj.PresetName = ReadMtString(buffer);
                return obj;
            }
        }
    }
}
