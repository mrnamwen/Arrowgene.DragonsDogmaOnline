#nullable enable
using System;
using System.Collections.Generic;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class BoxGachaResetHandler : GameRequestPacketHandler<C2SBoxGachaResetReq, S2CBoxGachaResetRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(BoxGachaResetHandler));

        public BoxGachaResetHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CBoxGachaResetRes Handle(GameClient client, C2SBoxGachaResetReq request)
        {
            var res = new S2CBoxGachaResetRes
            {
                BoxGachaId = request.BoxGachaId
            };

            // TODO: Reset the box gacha state for this character in the database
            // For now, return the default box contents (fresh box)

            // Return the reset box with full item stock
            res.BoxGachaItemList.Add(new CDataBoxGachaItemInfo
            {
                ItemId = 13800,
                ItemNum = 5,
                ItemStock = 13,
                Rank = 2,
                Effect = 0,
                Probability = 0,
                DrawNum = 0
            });

            Logger.Info($"Player {client.Character.CharacterId} reset Box Gacha {request.BoxGachaId}.");

            return res;
        }
    }
}
