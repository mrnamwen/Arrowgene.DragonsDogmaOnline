using System.Collections.Generic;
using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CPawnGetLegendPawnListRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_PAWN_GET_LEGEND_PAWN_LIST_RES;

        public S2CPawnGetLegendPawnListRes()
        {
            LegendPawnList = new List<CDataRegisterdPawnList>();
        }

        /// <summary>
        /// Legend Pawns for Riftstone search use the same structure as Official/Registered pawns.
        /// Note: CDataRegisteredLegendPawnInfo is for crafting legend pawns, not Riftstone search.
        /// </summary>
        public List<CDataRegisterdPawnList> LegendPawnList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CPawnGetLegendPawnListRes>
        {
            public override void Write(IBuffer buffer, S2CPawnGetLegendPawnListRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList(buffer, obj.LegendPawnList);
            }

            public override S2CPawnGetLegendPawnListRes Read(IBuffer buffer)
            {
                S2CPawnGetLegendPawnListRes obj = new S2CPawnGetLegendPawnListRes();
                ReadServerResponse(buffer, obj);
                obj.LegendPawnList = ReadEntityList<CDataRegisterdPawnList>(buffer);
                return obj;
            }
        }
    }
}
