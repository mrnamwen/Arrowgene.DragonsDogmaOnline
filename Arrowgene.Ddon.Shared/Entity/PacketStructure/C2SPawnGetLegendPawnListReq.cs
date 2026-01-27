using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Network;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class C2SPawnGetLegendPawnListReq : IPacketStructure
    {
        public PacketId Id => PacketId.C2S_PAWN_GET_LEGEND_PAWN_LIST_REQ;

        public class Serializer : PacketEntitySerializer<C2SPawnGetLegendPawnListReq>
        {
            public override void Write(IBuffer buffer, C2SPawnGetLegendPawnListReq obj)
            {
            }

            public override C2SPawnGetLegendPawnListReq Read(IBuffer buffer)
            {
                return new C2SPawnGetLegendPawnListReq();
            }
        }
    }
}
