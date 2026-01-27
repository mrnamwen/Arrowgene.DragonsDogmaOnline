using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using System;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Asset
{
    public class InfinityDeliveryAsset
    {
        public InfinityDeliveryAsset()
        {
            Categories = new List<InfinityDeliveryCategory>();
            Borders = new List<InfinityDeliveryBorderReward>();
        }

        public bool Enabled { get; set; }
        public uint EventId { get; set; }
        public DayOfWeek ResetDay { get; set; }
        public int ResetHour { get; set; }
        public List<InfinityDeliveryCategory> Categories { get; set; }
        public List<InfinityDeliveryBorderReward> Borders { get; set; }
    }

    public class InfinityDeliveryCategory
    {
        public InfinityDeliveryCategory()
        {
            Name = string.Empty;
            Items = new List<InfinityDeliveryItemEntry>();
        }

        public uint CategoryId { get; set; }
        public string Name { get; set; }
        public List<InfinityDeliveryItemEntry> Items { get; set; }
    }

    public class InfinityDeliveryItemEntry
    {
        public uint ItemId { get; set; }
        public uint PointValue { get; set; }
    }

    public class InfinityDeliveryBorderReward
    {
        public InfinityDeliveryBorderReward()
        {
            Rewards = new List<InfinityDeliveryReward>();
        }

        public uint BorderId { get; set; }
        public uint RequiredPoints { get; set; }
        public List<InfinityDeliveryReward> Rewards { get; set; }
    }

    public class InfinityDeliveryReward
    {
        public uint ItemId { get; set; }
        public uint Num { get; set; }
        public WalletType WalletType { get; set; }
        public uint WalletAmount { get; set; }
    }
}
