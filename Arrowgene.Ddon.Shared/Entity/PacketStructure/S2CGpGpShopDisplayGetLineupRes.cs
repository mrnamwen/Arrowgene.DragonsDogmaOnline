using System.Collections.Generic;
using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CGpGpShopDisplayGetLineupRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_GP_GP_SHOP_DISPLAY_GET_LINEUP_RES;

        public List<CDataGPShopDisplayLineup> LineupList { get; set; }

        public S2CGpGpShopDisplayGetLineupRes()
        {
            LineupList = new List<CDataGPShopDisplayLineup>();
        }

        public class Serializer : PacketEntitySerializer<S2CGpGpShopDisplayGetLineupRes>
        {
            public override void Write(IBuffer buffer, S2CGpGpShopDisplayGetLineupRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList<CDataGPShopDisplayLineup>(buffer, obj.LineupList);
            }

            public override S2CGpGpShopDisplayGetLineupRes Read(IBuffer buffer)
            {
                S2CGpGpShopDisplayGetLineupRes obj = new S2CGpGpShopDisplayGetLineupRes();
                ReadServerResponse(buffer, obj);
                obj.LineupList = ReadEntityList<CDataGPShopDisplayLineup>(buffer);
                return obj;
            }
        }
    }
}
