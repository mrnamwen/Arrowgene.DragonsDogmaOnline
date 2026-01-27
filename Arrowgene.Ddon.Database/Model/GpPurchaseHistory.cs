#nullable enable
using System;

namespace Arrowgene.Ddon.Database.Model;

public class GpPurchaseHistory
{
    public ulong Id { get; set; }
    public uint CharacterId { get; set; }
    public uint ShopType { get; set; }
    public uint LineupId { get; set; }
    public uint ItemId { get; set; }
    public uint Quantity { get; set; }
    public uint GpCost { get; set; }
    public DateTime PurchaseTime { get; set; }
}
