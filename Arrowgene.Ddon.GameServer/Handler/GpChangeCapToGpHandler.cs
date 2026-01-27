#nullable enable
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class GpChangeCapToGpHandler : GameRequestPacketHandler<C2SGpChangeCapToGpReq, S2CGpChangeCapToGpRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(GpChangeCapToGpHandler));

        public GpChangeCapToGpHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CGpChangeCapToGpRes Handle(GameClient client, C2SGpChangeCapToGpReq request)
        {
            S2CGpChangeCapToGpRes res = new S2CGpChangeCapToGpRes();

            res.ChangeListID = request.ChangeListID;

            // Map ChangeListID to GP amount (matches GpGetCapToGpChangeListHandler)
            uint changeListGPValue = request.ChangeListID switch
            {
                1 => 1,   // 1 Golden Gemstone
                2 => 10,  // 10 Golden Gemstones
                _ => 1    // Default fallback
            };

            Logger.Info($"Player {client.Character.CharacterId} converting CAP to GP: ChangeListID={request.ChangeListID}, GP={changeListGPValue}");

            var walletNtc = Server.WalletManager.AddToWalletNtc(client, client.Character, WalletType.GoldenGemstones, changeListGPValue);
            client.Send(walletNtc);

            res.GP = Server.WalletManager.GetWalletAmount(client.Character, WalletType.GoldenGemstones);
            // TODO: store CAP somewhere?
            res.CAP = 0;

            return res;
        }
    }
}
