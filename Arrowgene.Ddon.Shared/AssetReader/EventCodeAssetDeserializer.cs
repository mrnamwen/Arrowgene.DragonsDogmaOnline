using System;
using System.Text.Json;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class EventCodeAssetDeserializer : IAssetDeserializer<EventCodeAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(EventCodeAssetDeserializer));

        public EventCodeAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            EventCodeAsset asset = new EventCodeAsset();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            if (!document.RootElement.TryGetProperty("event_codes", out JsonElement eventCodesArray))
            {
                Logger.Info("No 'event_codes' array found in EventCodes.json");
                return asset;
            }

            foreach (var jEventCode in eventCodesArray.EnumerateArray())
            {
                EventCodeEntry entry = new EventCodeEntry();

                entry.Code = jEventCode.GetProperty("code").GetString().ToUpperInvariant();
                entry.Name = jEventCode.GetProperty("name").GetString();

                if (jEventCode.TryGetProperty("mail_body", out JsonElement mailBody))
                {
                    entry.MailBody = mailBody.GetString();
                }
                else
                {
                    entry.MailBody = "Thank you for redeeming this event code!";
                }

                if (jEventCode.TryGetProperty("comment", out JsonElement comment))
                {
                    entry.Comment = comment.GetString();
                }

                if (jEventCode.TryGetProperty("rewards", out JsonElement rewards))
                {
                    // Parse item rewards
                    if (rewards.TryGetProperty("items", out JsonElement items))
                    {
                        foreach (var jItem in items.EnumerateArray())
                        {
                            EventCodeItemReward itemReward = new EventCodeItemReward();
                            itemReward.ItemId = jItem.GetProperty("item_id").GetUInt32();
                            itemReward.Amount = jItem.GetProperty("amount").GetUInt32();

                            if (jItem.TryGetProperty("comment", out JsonElement itemComment))
                            {
                                itemReward.Comment = itemComment.GetString();
                            }

                            entry.Rewards.Items.Add(itemReward);
                        }
                    }

                    // Parse wallet rewards
                    if (rewards.TryGetProperty("wallet", out JsonElement wallet))
                    {
                        foreach (var jWallet in wallet.EnumerateArray())
                        {
                            EventCodeWalletReward walletReward = new EventCodeWalletReward();

                            string walletTypeStr = jWallet.GetProperty("wallet_type").GetString();
                            if (Enum.TryParse(walletTypeStr, true, out WalletType walletType))
                            {
                                walletReward.WalletType = walletType;
                            }
                            else
                            {
                                Logger.Error($"Invalid wallet type '{walletTypeStr}' in event code {entry.Code}");
                                continue;
                            }

                            walletReward.Amount = jWallet.GetProperty("amount").GetUInt32();

                            entry.Rewards.WalletRewards.Add(walletReward);
                        }
                    }
                }

                if (asset.EventCodes.ContainsKey(entry.Code))
                {
                    Logger.Info($"Duplicate event code '{entry.Code}' found, skipping.");
                    continue;
                }

                asset.EventCodes[entry.Code] = entry;
                Logger.Debug($"Loaded event code: {entry.Code} - {entry.Name}");
            }

            Logger.Info($"Loaded {asset.EventCodes.Count} event codes");

            return asset;
        }
    }
}
