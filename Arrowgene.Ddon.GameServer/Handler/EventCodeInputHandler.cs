using Arrowgene.Ddon.Database;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class EventCodeInputHandler : GameRequestPacketHandler<C2SEventCodeInputReq, S2CEventCodeInputRes>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(EventCodeInputHandler));

        public EventCodeInputHandler(DdonGameServer server) : base(server)
        {
        }

        public override S2CEventCodeInputRes Handle(GameClient client, C2SEventCodeInputReq request)
        {
            S2CEventCodeInputRes res = new S2CEventCodeInputRes();

            // Normalize the code to uppercase for lookup
            string code = request.Code?.ToUpperInvariant() ?? string.Empty;

            Logger.Info($"Character {client.Character.CharacterId} attempting to redeem event code: {code}");

            // Look up the code in our asset
            if (!Server.AssetRepository.EventCodeAsset.EventCodes.TryGetValue(code, out EventCodeEntry eventCode))
            {
                Logger.Info($"Event code not found: {code}");
                throw new ResponseErrorException(ErrorCode.ERROR_CODE_EVENT_CODE_INPUT_LOCK);
            }

            // TODO: Check if character has already redeemed this code
            // This would require a database table to track redeemed codes per character
            // For now, codes can be redeemed multiple times

            // Set the response name (shown to the player)
            res.Name = eventCode.Name;

            // Create the mail message
            SystemMailMessage mail = new SystemMailMessage()
            {
                CharacterId = client.Character.CharacterId,
                MessageState = MailState.Unopened,
                SenderName = "Event Code",
                Title = eventCode.Name,
                Body = eventCode.MailBody ?? "Thank you for redeeming this event code!"
            };

            // Add item attachments
            foreach (var itemReward in eventCode.Rewards.Items)
            {
                if (itemReward.ItemId == 0 || itemReward.Amount == 0)
                {
                    continue;
                }

                mail.Attachments.Add(new SystemMailAttachment()
                {
                    AttachmentType = SystemMailAttachmentType.Item,
                    Param1 = itemReward.ItemId,
                    Param2 = itemReward.Amount,
                    IsReceived = false
                });

                Logger.Debug($"Adding item attachment: ItemId={itemReward.ItemId}, Amount={itemReward.Amount}");
            }

            // Add wallet/currency attachments
            foreach (var walletReward in eventCode.Rewards.WalletRewards)
            {
                if (walletReward.Amount == 0)
                {
                    continue;
                }

                mail.Attachments.Add(new SystemMailAttachment()
                {
                    AttachmentType = SystemMailAttachmentType.GP,
                    Param1 = (uint)walletReward.WalletType,
                    Param2 = walletReward.Amount,
                    IsReceived = false
                });

                Logger.Debug($"Adding wallet attachment: WalletType={walletReward.WalletType}, Amount={walletReward.Amount}");
            }

            // Send the mail if there are any attachments or if we want to send a notification anyway
            if (mail.Attachments.Count > 0 || !string.IsNullOrEmpty(mail.Body))
            {
                SystemMailService.DeliverSystemMailMessage(Server.Database, mail);
                Logger.Info($"Event code '{code}' redeemed by character {client.Character.CharacterId}. Mail sent with {mail.Attachments.Count} attachments.");

                // Notify the client about the new mail
                client.Send(new S2CMailSystemMailSendNtc());
            }
            else
            {
                Logger.Info($"Event code '{code}' redeemed by character {client.Character.CharacterId}. No rewards to send.");
            }

            return res;
        }
    }
}
