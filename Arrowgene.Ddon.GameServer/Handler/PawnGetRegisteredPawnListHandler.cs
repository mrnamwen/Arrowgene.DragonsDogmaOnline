using Arrowgene.Ddon.GameServer.Scripting.Interfaces;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class PawnGetRegisteredPawnListHandler(DdonGameServer server) : GameRequestPacketHandler<C2SPawnGetRegisteredPawnListReq, S2CPawnGetRegisteredPawnListRes>(server)
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(PawnGetRegisteredPawnListHandler));

        public override S2CPawnGetRegisteredPawnListRes Handle(GameClient client, C2SPawnGetRegisteredPawnListReq request)
        {
            HashSet<uint> clanPawns = [];
            List<CDataRegisterdPawnList> registeredPawns = [];

            Server.Database.ExecuteInTransaction(connection =>
            {
                if (client.Character.ClanId != 0)
                {
                    clanPawns = [.. Server.Database.SelectClanPawns(client.Character.ClanId, limit: 1000, connectionIn: connection)];
                }
                registeredPawns = Server.Database.SelectRegisteredPawns(client.Character, request.SearchParam, connection);
            });

            // Add official pawns to the search results if enabled
            if (Server.GameSettings.GameServerSettings.EnableOfficialPawnsInSearch)
            {
                var officialPawns = Server.OfficialPawnManager.GetOfficialPawnsForSearch(request.SearchParam);
                var maxOfficialPawns = (int)Server.GameSettings.GameServerSettings.OfficialPawnMaxResults;
                registeredPawns.AddRange(officialPawns.Take(maxOfficialPawns));
            }

            var mixin = Server.ScriptManager.MixinModule.Get<IRentalCostMixin>("rental_cost");

            foreach (var registeredPawn in registeredPawns)
            {
                // Official pawns have free rental cost
                if (Server.OfficialPawnManager.IsOfficialPawn(registeredPawn.PawnId))
                {
                    registeredPawn.RentalCost = 0;
                }
                else
                {
                    registeredPawn.RentalCost = mixin.GetRentalCost(client, registeredPawn, clanPawns.Contains(registeredPawn.PawnId));
                }
            }

            return new()
            {
                RegisterdPawnList = registeredPawns,
            };
        }
    }
}
