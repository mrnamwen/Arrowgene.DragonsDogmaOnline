using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnGetLegendPawnListHandler : GameRequestPacketHandler<C2SPawnGetLegendPawnListReq, S2CPawnGetLegendPawnListRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnGetLegendPawnListHandler));

        public PawnGetLegendPawnListHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CPawnGetLegendPawnListRes Handle(GameClient client, C2SPawnGetLegendPawnListReq request)
        {
            // Legend Pawns for Riftstone search use CDataRegisterdPawnList (same as Official Pawns).
            // Note: CDataRegisteredLegendPawnInfo is for crafting legend pawns (CraftMasterLegendPawns).
            // Currently returns empty list - Legend Pawns would need database support similar to Official Pawns.
            return new S2CPawnGetLegendPawnListRes();
        }
    }
}
