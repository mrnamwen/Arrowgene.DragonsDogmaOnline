using System.Collections.Generic;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Network;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class MandragoraChangeGoldenCraftHandler : GameRequestPacketHandler<C2SMandragoraChangeGoldenCraftReq, S2CMandragoraChangeGoldenCraftRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(MandragoraChangeGoldenCraftHandler));

        public MandragoraChangeGoldenCraftHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CMandragoraChangeGoldenCraftRes Handle(GameClient client, C2SMandragoraChangeGoldenCraftReq request)
        {
            uint characterId = client.Character.CharacterId;
            uint mandragoraId = request.MandragoraId;

            // Get the mandragora to verify it exists
            var mandragora = Server.Database.SelectMandragora(characterId, mandragoraId);
            if (mandragora == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL, "Invalid mandragora ID");
            }

            // Check for active craft
            var craft = Server.MandragoraManager.GetActiveCraft(characterId, mandragoraId);
            if (craft == null)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_CRAFT_PRODUCT_NOT_COMPLETE, "No active craft to skip");
            }

            // Check if already complete (no need to spend GG)
            if (Server.MandragoraManager.IsCraftComplete(characterId, mandragoraId))
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_FAIL, "Craft is already complete");
            }

            // Get GG cost from settings
            uint ggCost = Server.GameSettings.GameServerSettings.MandragoraCultivationSkipGGCost;

            // Check if player has enough GG
            uint currentGG = Server.WalletManager.GetWalletAmount(client.Character, WalletType.GoldenGemstones);
            if (currentGG < ggCost)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_GP_LACK_GP);
            }

            // Deduct GG from wallet
            bool walletUpdate = Server.WalletManager.RemoveFromWalletNtc(client, client.Character, WalletType.GoldenGemstones, ggCost);
            if (!walletUpdate)
            {
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_GP_LACK_GP);
            }

            // Force complete the craft with species evolution
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

            // Get updated wallet amounts
            var walletPoints = new List<CDataWalletPoint>
            {
                new CDataWalletPoint
                {
                    Type = WalletType.GoldenGemstones,
                    Value = Server.WalletManager.GetWalletAmount(client.Character, WalletType.GoldenGemstones)
                }
            };

            var res = new S2CMandragoraChangeGoldenCraftRes()
            {
                NewSpeciesIndex = evolutionResult.NewSpeciesIndex,
                IsNewSpeciesDiscovery = evolutionResult.IsNewSpeciesDiscovery,
                IsFirstDiscovery = evolutionResult.IsFirstDiscovery,
                ItemUpdateResults = itemUpdateResults,
                WalletPoints = walletPoints
            };

            Logger.Info($"Character {characterId} used {ggCost} GG to instantly complete craft from mandragora {mandragoraId}, received item {evolutionResult.ResultItemId}, species {evolutionResult.OldSpeciesIndex} -> {evolutionResult.NewSpeciesIndex}");

            return res;
        }
    }
}
