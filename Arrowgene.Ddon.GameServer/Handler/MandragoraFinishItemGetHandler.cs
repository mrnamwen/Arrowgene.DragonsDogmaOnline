using System.Collections.Generic;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Network;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class MandragoraFinishItemGetHandler : GameRequestPacketHandler<C2SMandragoraFinishItemGetReq, S2CMandragoraFinishItemGetRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(MandragoraFinishItemGetHandler));

        public MandragoraFinishItemGetHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CMandragoraFinishItemGetRes Handle(GameClient client, C2SMandragoraFinishItemGetReq request)
        {
            uint characterId = client.Character.CharacterId;
            uint mandragoraId = request.MandragoraId;

            // Get the mandragora to verify it exists
            var mandragora = Server.Database.SelectMandragora(characterId, mandragoraId);
            if (mandragora == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL, "Invalid mandragora ID");
            }

            // Check if craft is complete
            if (!Server.MandragoraManager.IsCraftComplete(characterId, mandragoraId))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_CRAFT_PRODUCT_NOT_COMPLETE);
            }

            // Complete the craft with species evolution
            var evolutionResult = Server.MandragoraManager.CompleteCraftWithEvolution(characterId, mandragoraId);
            if (evolutionResult == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_CRAFT_INTERNAL);
            }

            // Add the crafted item to inventory
            var itemUpdateResults = new List<CDataItemUpdateResult>();

            Server.Database.ExecuteInTransaction(connection =>
            {
                // Add crafted item to player's item bag (true = send to item bag, not storage)
                List<CDataItemUpdateResult> updateResults = Server.ItemManager.AddItem(
                    Server,
                    client.Character,
                    true, // Send to item bag
                    evolutionResult.ResultItemId,
                    1, // Quantity
                    connectionIn: connection
                );
                itemUpdateResults.AddRange(updateResults);
            });

            var res = new S2CMandragoraFinishItemGetRes()
            {
                NewSpeciesIndex = evolutionResult.NewSpeciesIndex,
                IsNewSpeciesDiscovery = evolutionResult.IsNewSpeciesDiscovery,
                IsFirstDiscovery = evolutionResult.IsFirstDiscovery,
                ItemUpdateResults = itemUpdateResults
            };

            Logger.Info($"Character {characterId} collected craft from mandragora {mandragoraId}, received item {evolutionResult.ResultItemId}, species {evolutionResult.OldSpeciesIndex} -> {evolutionResult.NewSpeciesIndex}");

            return res;
        }
    }
}
