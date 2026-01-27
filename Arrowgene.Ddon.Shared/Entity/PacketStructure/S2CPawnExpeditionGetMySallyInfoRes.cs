using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CPawnExpeditionGetMySallyInfoRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_PAWN_EXPEDITION_PAWN_EXPEDITION_GET_MY_SALLY_INFO_RES;

        public S2CPawnExpeditionGetMySallyInfoRes()
        {
            PawnExpeditionList = new List<CDataPawnExpeditionMySallyInfo>();
        }

        public List<CDataPawnExpeditionMySallyInfo> PawnExpeditionList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CPawnExpeditionGetMySallyInfoRes>
        {
            public override void Write(IBuffer buffer, S2CPawnExpeditionGetMySallyInfoRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList(buffer, obj.PawnExpeditionList);
            }

            public override S2CPawnExpeditionGetMySallyInfoRes Read(IBuffer buffer)
            {
                S2CPawnExpeditionGetMySallyInfoRes obj = new S2CPawnExpeditionGetMySallyInfoRes();
                ReadServerResponse(buffer, obj);
                obj.PawnExpeditionList = ReadEntityList<CDataPawnExpeditionMySallyInfo>(buffer);
                return obj;
            }
        }
    }
}
