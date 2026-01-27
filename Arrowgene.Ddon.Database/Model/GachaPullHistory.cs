#nullable enable
using System;
using System.Collections.Generic;

namespace Arrowgene.Ddon.Database.Model;

public class GachaPullHistory
{
    public ulong Id { get; set; }
    public uint CharacterId { get; set; }
    public uint GachaId { get; set; }
    public uint PullType { get; set; }
    public uint CurrencyType { get; set; }
    public uint CurrencyCost { get; set; }
    public uint PullCount { get; set; }
    public DateTime PullTime { get; set; }
    public List<GachaPullResult> Results { get; set; } = new();
}

public class GachaPullResult
{
    public ulong Id { get; set; }
    public ulong PullId { get; set; }
    public uint ResultIndex { get; set; }
    public uint ItemId { get; set; }
    public uint Quantity { get; set; }
    public uint Rarity { get; set; }
    public bool IsBonus { get; set; }
}
