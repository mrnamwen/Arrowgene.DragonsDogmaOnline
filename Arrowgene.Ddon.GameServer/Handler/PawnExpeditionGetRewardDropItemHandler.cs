using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnExpeditionGetRewardDropItemHandler : GameRequestPacketHandler<C2SPawnExpeditionGetRewardDropItemReq, S2CPawnExpeditionGetRewardDropItemRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnExpeditionGetRewardDropItemHandler));

        public PawnExpeditionGetRewardDropItemHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CPawnExpeditionGetRewardDropItemRes Handle(GameClient client, C2SPawnExpeditionGetRewardDropItemReq request)
        {
            var reward = Server.PawnExpeditionManager.ClaimReward(client.Character.CharacterId, (int)request.ItemIndex);
            if (reward == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL);
            }

            // Add item to player's inventory
            var updateResults = Server.ItemManager.AddItem(Server, client.Character, false, reward.ItemId, reward.Num);

            return new S2CPawnExpeditionGetRewardDropItemRes()
            {
                UpdateItemList = updateResults
            };
        }
    }
}
