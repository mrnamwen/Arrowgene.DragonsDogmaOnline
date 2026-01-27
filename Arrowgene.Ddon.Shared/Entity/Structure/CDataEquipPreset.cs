using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Shared.Entity.Structure
{
    public class CDataEquipPreset
    {
        public CDataEquipPreset()
        {
            PresetName = string.Empty;
        }

        public byte PresetNo { get; set; }
        public string PresetName { get; set; }
        public JobId Job { get; set; }

        public class Serializer : EntitySerializer<CDataEquipPreset>
        {
            public override void Write(IBuffer buffer, CDataEquipPreset obj)
            {
                WriteByte(buffer, obj.PresetNo);
                WriteMtString(buffer, obj.PresetName);
                WriteByte(buffer, (byte)obj.Job);
            }

            public override CDataEquipPreset Read(IBuffer buffer)
            {
                CDataEquipPreset obj = new CDataEquipPreset();
                obj.PresetNo = ReadByte(buffer);
                obj.PresetName = ReadMtString(buffer);
                obj.Job = (JobId)ReadByte(buffer);
                return obj;
            }
        }
    }
}
