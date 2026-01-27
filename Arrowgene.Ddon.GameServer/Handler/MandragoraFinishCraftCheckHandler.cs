using System;
using System.Collections.Generic;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Ddon.Shared.Network;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class MandragoraFinishCraftCheckHandler : GameRequestPacketHandler<C2SMandragoraFinishCraftCheckReq, S2CMandragoraFinishCraftCheckRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(MandragoraFinishCraftCheckHandler));

        public MandragoraFinishCraftCheckHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CMandragoraFinishCraftCheckRes Handle(GameClient client, C2SMandragoraFinishCraftCheckReq request)
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
            bool isComplete = craft != null && Server.MandragoraManager.IsCraftComplete(characterId, mandragoraId);

            var res = new S2CMandragoraFinishCraftCheckRes()
            {
                IsComplete = isComplete,
                CraftInfo = new CDataMyMandragoraBeginCraftResUnk0
                {
                    SpeciesIndex = mandragora.SpeciesIndex,
                    Unk1 = (byte)mandragora.SpeciesCategory,
                    MandragoraId = mandragora.MandragoraId,
                    Unk3 = mandragora.Name,
                    Unk4 = craft?.RecipeId ?? 0,
                    Unk5 = craft != null ? ((DateTimeOffset)craft.EndTime).ToUnixTimeSeconds() : 0,
                    Unk6 = craft != null ? (uint)(craft.EndTime - craft.StartTime).TotalSeconds : 0,
                    Unk7 = new CDataMyMandragoraBeginCraftResUnk0Unk7
                    {
                        Unk0 = craft?.ResultItemId ?? 0,
                        Unk1 = 1,
                        Unk2 = new List<CDataMyMandragoraBeginCraftResUnk0Unk7Unk2>(),
                        Unk3 = 0
                    }
                }
            };

            Logger.Info($"Character {characterId} checked craft status for mandragora {mandragoraId}: isComplete={isComplete}");

            return res;
        }
    }
}
