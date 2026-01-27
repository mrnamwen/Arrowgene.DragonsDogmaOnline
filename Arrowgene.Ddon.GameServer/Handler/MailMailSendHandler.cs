using System;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class MailMailSendHandler : GameRequestPacketHandler<C2SMailMailSendReq, S2CMailMailSendRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(MailMailSendHandler));

        public MailMailSendHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CMailMailSendRes Handle(GameClient client, C2SMailMailSendReq request)
        {
            // Create the personal mail message
            var message = new PersonalMailMessage()
            {
                RecipientCharacterId = request.RecipientCharacterId,
                SenderCharacterId = client.Character.CharacterId,
                SenderName = client.Character.FirstName,
                MessageState = MailState.Unopened,
                Title = request.Title,
                Body = request.Body,
                SendDate = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            // Insert into database
            long messageId = Server.Database.InsertPersonalMailMessage(message);
            message.MessageId = (ulong)messageId;

            // Get sender's clan name if they have one
            var (clanId, _) = Server.ClanManager.ClanMembership(client.Character.CharacterId);
            if (clanId != 0)
            {
                var clan = Server.ClanManager.GetClan(clanId);
                if (clan != null)
                {
                    message.SenderClanName = clan.ClanUserParam.Name;
                }
            }

            // Try to notify the recipient if they're online
            GameClient recipientClient = Server.ClientLookup.GetClientByCharacterId(request.RecipientCharacterId);
            if (recipientClient != null)
            {
                var ntc = new S2CMailMailSendNtc()
                {
                    MailInfo = message.ToCDataMailInfo()
                };
                recipientClient.Send(ntc);
            }

            return new S2CMailMailSendRes();
        }
    }
}
