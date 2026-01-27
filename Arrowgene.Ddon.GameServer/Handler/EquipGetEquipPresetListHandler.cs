using System.Linq;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class EquipGetEquipPresetListHandler : GameRequestPacketHandler<C2SEquipGetEquipPresetListReq, S2CEquipGetEquipPresetListRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(EquipGetEquipPresetListHandler));

        public EquipGetEquipPresetListHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CEquipGetEquipPresetListRes Handle(GameClient client, C2SEquipGetEquipPresetListReq request)
        {
            var presets = Server.Database.SelectEquipPresets(client.Character.CharacterId, client.Character.Job);

            return new S2CEquipGetEquipPresetListRes()
            {
                EquipPresetList = presets.Select(p => p.ToCDataEquipPreset()).ToList()
            };
        }
    }
}
