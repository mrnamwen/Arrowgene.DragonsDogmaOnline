using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class InfinityDeliveryGetCategoryListHandler : GameRequestPacketHandler<C2SInfinityDeliveryGetCategoryListReq, S2CInfinityDeliveryGetCategoryListRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(InfinityDeliveryGetCategoryListHandler));

        public InfinityDeliveryGetCategoryListHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CInfinityDeliveryGetCategoryListRes Handle(GameClient client, C2SInfinityDeliveryGetCategoryListReq request)
        {
            var res = new S2CInfinityDeliveryGetCategoryListRes();
            res.CategoryList = Server.InfinityDeliveryManager.GetCategories();
            return res;
        }
    }
}
