using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class EquipUpdateEquipPresetNameHandler : GameRequestPacketHandler<C2SEquipUpdateEquipPresetNameReq, S2CEquipUpdateEquipPresetNameRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(EquipUpdateEquipPresetNameHandler));

        public EquipUpdateEquipPresetNameHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CEquipUpdateEquipPresetNameRes Handle(GameClient client, C2SEquipUpdateEquipPresetNameReq request)
        {
            var character = client.Character;

            Server.Database.UpdateEquipPresetName(
                character.CharacterId,
                character.Job,
                request.PresetNo,
                request.PresetName
            );

            Logger.Info(client, $"Equipment preset #{request.PresetNo} renamed to '{request.PresetName}'");

            return new S2CEquipUpdateEquipPresetNameRes();
        }
    }
}
