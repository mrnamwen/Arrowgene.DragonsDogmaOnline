using Arrowgene.Ddon.Database.Model;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class EquipUpdateEquipPresetHandler : GameRequestPacketHandler<C2SEquipUpdateEquipPresetReq, S2CEquipUpdateEquipPresetRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(EquipUpdateEquipPresetHandler));

        public EquipUpdateEquipPresetHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CEquipUpdateEquipPresetRes Handle(GameClient client, C2SEquipUpdateEquipPresetReq request)
        {
            var character = client.Character;
            var equipment = character.Equipment;

            // Get current performance equipment
            var perfItems = equipment.GetItems(EquipType.Performance);
            // Get current visual equipment
            var visualItems = equipment.GetItems(EquipType.Visual);

            // Check if preset already exists
            var existingPreset = Server.Database.SelectEquipPreset(character.CharacterId, character.Job, request.PresetNo);

            var preset = new EquipPreset()
            {
                CharacterId = character.CharacterId,
                Job = character.Job,
                PresetNo = request.PresetNo,
                PresetName = existingPreset?.PresetName ?? $"Preset {request.PresetNo}",
                // Performance slots (1-indexed in enum, 0-indexed in list)
                PPrimaryWeapon = perfItems.Count > 0 ? perfItems[0]?.UId : null,
                PSecondaryWeapon = perfItems.Count > 1 ? perfItems[1]?.UId : null,
                PHead = perfItems.Count > 2 ? perfItems[2]?.UId : null,
                PBody = perfItems.Count > 3 ? perfItems[3]?.UId : null,
                PClothing = perfItems.Count > 4 ? perfItems[4]?.UId : null,
                PArm = perfItems.Count > 5 ? perfItems[5]?.UId : null,
                PLeg = perfItems.Count > 6 ? perfItems[6]?.UId : null,
                PLegWear = perfItems.Count > 7 ? perfItems[7]?.UId : null,
                POverWear = perfItems.Count > 8 ? perfItems[8]?.UId : null,
                PJewelry1 = perfItems.Count > 9 ? perfItems[9]?.UId : null,
                PJewelry2 = perfItems.Count > 10 ? perfItems[10]?.UId : null,
                PJewelry3 = perfItems.Count > 11 ? perfItems[11]?.UId : null,
                PJewelry4 = perfItems.Count > 12 ? perfItems[12]?.UId : null,
                PJewelry5 = perfItems.Count > 13 ? perfItems[13]?.UId : null,
                PLantern = perfItems.Count > 14 ? perfItems[14]?.UId : null,
                // Visual slots
                VPrimaryWeapon = visualItems.Count > 0 ? visualItems[0]?.UId : null,
                VSecondaryWeapon = visualItems.Count > 1 ? visualItems[1]?.UId : null,
                VHead = visualItems.Count > 2 ? visualItems[2]?.UId : null,
                VBody = visualItems.Count > 3 ? visualItems[3]?.UId : null,
                VClothing = visualItems.Count > 4 ? visualItems[4]?.UId : null,
                VArm = visualItems.Count > 5 ? visualItems[5]?.UId : null,
                VLeg = visualItems.Count > 6 ? visualItems[6]?.UId : null,
                VLegWear = visualItems.Count > 7 ? visualItems[7]?.UId : null,
                VOverWear = visualItems.Count > 8 ? visualItems[8]?.UId : null
            };

            if (existingPreset != null)
            {
                Server.Database.UpdateEquipPreset(preset);
            }
            else
            {
                Server.Database.InsertEquipPreset(preset);
            }

            Logger.Info(client, $"Equipment preset #{request.PresetNo} saved for job {character.Job}");

            return new S2CEquipUpdateEquipPresetRes();
        }
    }
}
