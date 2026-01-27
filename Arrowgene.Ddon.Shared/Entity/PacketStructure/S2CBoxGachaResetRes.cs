using System.Collections.Generic;
using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CBoxGachaResetRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_BOX_GACHA_BOX_GACHA_RESET_RES;

        public uint BoxGachaId { get; set; }
        public List<CDataBoxGachaItemInfo> BoxGachaItemList { get; set; }

        public S2CBoxGachaResetRes()
        {
            BoxGachaItemList = new List<CDataBoxGachaItemInfo>();
        }

        public class Serializer : PacketEntitySerializer<S2CBoxGachaResetRes>
        {
            public override void Write(IBuffer buffer, S2CBoxGachaResetRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteUInt32(buffer, obj.BoxGachaId);
                WriteEntityList<CDataBoxGachaItemInfo>(buffer, obj.BoxGachaItemList);
            }

            public override S2CBoxGachaResetRes Read(IBuffer buffer)
            {
                S2CBoxGachaResetRes obj = new S2CBoxGachaResetRes();
                ReadServerResponse(buffer, obj);
                obj.BoxGachaId = ReadUInt32(buffer);
                obj.BoxGachaItemList = ReadEntityList<CDataBoxGachaItemInfo>(buffer);
                return obj;
            }
        }
    }
}
