using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.Structure;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class GpShopAssetDeserializer : IAssetDeserializer<GpShopAsset>
    {
        public GpShopAsset ReadPath(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<GpShopAssetJson>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })?.ToAsset() ?? new GpShopAsset();
        }

        private class GpShopAssetJson
        {
            public List<GpShopItemJson> Items { get; set; }

            public GpShopAssetJson()
            {
                Items = new List<GpShopItemJson>();
            }

            public GpShopAsset ToAsset()
            {
                var asset = new GpShopAsset();

                foreach (var item in Items)
                {
                    var lineupItem = new CDataGPShopLineupItem
                    {
                        LineupId = item.LineupId,
                        ItemId = item.ItemId,
                        Name = item.Name ?? string.Empty,
                        Description = item.Description ?? string.Empty,
                        Price = item.Price,
                        BasePrice = item.BasePrice > 0 ? item.BasePrice : item.Price,
                        ItemNum = item.ItemNum > 0 ? item.ItemNum : 1,
                        PurchaseLimit = item.PurchaseLimit,
                        PurchaseCount = 0,
                        BeginTime = item.BeginTime,
                        EndTime = item.EndTime,
                        CategoryId = item.CategoryId,
                        IconId = item.IconId,
                        SortOrder = item.SortOrder,
                        // Course ticket fields (default to 0 for regular items)
                        CourseId = item.CourseId,
                        DurationSeconds = item.DurationSeconds,
                        BackIconId = item.BackIconId,
                        FrameIconId = item.FrameIconId
                    };

                    asset.LineupItems[item.LineupId] = lineupItem;

                    if (!asset.CategoryLineups.ContainsKey(item.CategoryId))
                    {
                        asset.CategoryLineups[item.CategoryId] = new List<uint>();
                    }
                    asset.CategoryLineups[item.CategoryId].Add(item.LineupId);
                }

                return asset;
            }
        }

        private class GpShopItemJson
        {
            public uint LineupId { get; set; }
            public uint ItemId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public uint Price { get; set; }
            public uint BasePrice { get; set; }
            public uint ItemNum { get; set; }
            public uint PurchaseLimit { get; set; }
            public long BeginTime { get; set; }
            public long EndTime { get; set; }
            public uint CategoryId { get; set; }
            public uint IconId { get; set; }
            public byte SortOrder { get; set; }
            // Course ticket fields (0 = regular item, >0 = course ticket)
            public uint CourseId { get; set; }
            public uint DurationSeconds { get; set; }
            public uint BackIconId { get; set; }
            public uint FrameIconId { get; set; }
        }
    }
}
