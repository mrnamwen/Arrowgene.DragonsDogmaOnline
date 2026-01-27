using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class MailMailGetTextHandler : GameRequestPacketHandler<C2SMailMailGetTextReq, S2CMailMailGetTextRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(MailMailGetTextHandler));

        public MailMailGetTextHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CMailMailGetTextRes Handle(GameClient client, C2SMailMailGetTextReq request)
        {
            var message = Server.Database.SelectPersonalMailMessage(request.MailId);

            // Mark the message as opened
            Server.Database.UpdatePersonalMailMessageState(request.MailId, MailState.Opened);

            var result = new S2CMailMailGetTextRes()
            {
                MailId = request.MailId,
                MailTextInfo = new CDataMailTextInfo()
                {
                    Text = message.Body
                }
            };

            return result;
        }
    }
}
