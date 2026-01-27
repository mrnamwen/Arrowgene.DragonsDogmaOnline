using System.Collections.Generic;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class EventCodeReward
    {
        public EventCodeReward()
        {
            Items = new List<EventCodeItemReward>();
            WalletRewards = new List<EventCodeWalletReward>();
        }

        /// <summary>
        /// Item rewards to be sent via mail.
        /// </summary>
        public List<EventCodeItemReward> Items { get; set; }

        /// <summary>
        /// Wallet/currency rewards to be sent via mail.
        /// </summary>
        public List<EventCodeWalletReward> WalletRewards { get; set; }
    }

    public class EventCodeItemReward
    {
        public uint ItemId { get; set; }
        public uint Amount { get; set; }
        public string Comment { get; set; }
    }

    public class EventCodeWalletReward
    {
        public WalletType WalletType { get; set; }
        public uint Amount { get; set; }
    }

    public class EventCodeEntry
    {
        public EventCodeEntry()
        {
            Rewards = new EventCodeReward();
        }

        /// <summary>
        /// The event code string (e.g., "UPCT-YUAW-3K16-8255").
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The display name shown to the player when the code is redeemed.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The body text for the mail message.
        /// </summary>
        public string MailBody { get; set; }

        /// <summary>
        /// Optional comment for documentation purposes.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The rewards granted by this code.
        /// </summary>
        public EventCodeReward Rewards { get; set; }
    }

    public class EventCodeAsset
    {
        public EventCodeAsset()
        {
            EventCodes = new Dictionary<string, EventCodeEntry>();
        }

        /// <summary>
        /// Dictionary mapping code strings to their entries.
        /// </summary>
        public Dictionary<string, EventCodeEntry> EventCodes { get; set; }
    }
}
