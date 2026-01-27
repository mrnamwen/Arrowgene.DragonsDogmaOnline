using Arrowgene.Buffers;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Network;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Entity.PacketStructure
{
    public class S2CPawnExpeditionGetRewardDropItemRes : ServerResponse
    {
        public override PacketId Id => PacketId.S2C_PAWN_EXPEDITION_PAWN_EXPEDITION_GET_REWARD_DROP_ITEM_RES;

        public S2CPawnExpeditionGetRewardDropItemRes()
        {
            UpdateItemList = new List<CDataItemUpdateResult>();
        }

        public List<CDataItemUpdateResult> UpdateItemList { get; set; }

        public class Serializer : PacketEntitySerializer<S2CPawnExpeditionGetRewardDropItemRes>
        {
            public override void Write(IBuffer buffer, S2CPawnExpeditionGetRewardDropItemRes obj)
            {
                WriteServerResponse(buffer, obj);
                WriteEntityList(buffer, obj.UpdateItemList);
            }

            public override S2CPawnExpeditionGetRewardDropItemRes Read(IBuffer buffer)
            {
                S2CPawnExpeditionGetRewardDropItemRes obj = new S2CPawnExpeditionGetRewardDropItemRes();
                ReadServerResponse(buffer, obj);
                obj.UpdateItemList = ReadEntityList<CDataItemUpdateResult>(buffer);
                return obj;
            }
        }
    }
}
