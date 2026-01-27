using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Logging;
using System.Data;
using System;
using System.IO;
using System.Text.Json;
using Arrowgene.Ddon.Shared.Model;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class GachaAssetDeserializer : IAssetDeserializer<GachaAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(GachaAssetDeserializer));

        public GachaAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            GachaAsset asset = new GachaAsset();

            string json = File.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            foreach (var jLootBox in document.RootElement.GetProperty("boxes").EnumerateArray())
            {
                var gachaInfo = new CDataGachaInfo()
                {
                    Id = jLootBox.GetProperty("gacha_id").GetUInt32(),
                    Begin = jLootBox.GetProperty("start_time").GetInt64(),
                    End = jLootBox.GetProperty("end_time").GetInt64(),
                    Name = jLootBox.GetProperty("name").GetString(),
                    Description = jLootBox.GetProperty("description").GetString(),
                    Detail = jLootBox.GetProperty("detail").GetString(),
                    WeightDispType = jLootBox.GetProperty("weight_display_type").GetByte(),
                    WeightDispTitle = jLootBox.GetProperty("weight_display_title").GetString(),
                    WeightDispText = jLootBox.GetProperty("weight_display_text").GetString(),
                    ListAddr = jLootBox.GetProperty("list_address").GetString(),
                    ImageAddr = jLootBox.GetProperty("image_address").GetString(),
                };

                // Build the base item pool from draw_list (shared across all draw options)
                var baseItemPool = new System.Collections.Generic.List<CDataGachaItemInfo>();

                foreach (var jItem in jLootBox.GetProperty("draw_list").EnumerateArray())
                {
                    baseItemPool.Add(new CDataGachaItemInfo()
                    {
                        ItemId = jItem.GetProperty("item_id").GetUInt32(),
                        ItemNum = jItem.GetProperty("amount").GetUInt32(),
                        Rank = jItem.GetProperty("rank").GetUInt32(),
                        Effect = jItem.GetProperty("effect").GetUInt32(),
                        Probability = jItem.GetProperty("chance").GetDouble()
                    });
                }

                // Parse draw_options - each option creates a separate DrawGroup
                foreach (var jDrawOption in jLootBox.GetProperty("draw_options").EnumerateArray())
                {
                    uint numDraws = jDrawOption.GetProperty("num_draws").GetUInt32();

                    var drawGroupInfo = new CDataGachaDrawGroupInfo();

                    // Create a DrawInfo with the correct number of draws for this option
                    var drawInfo = new CDataGachaDrawInfo()
                    {
                        Num = numDraws,
                        IsBonus = false
                    };
                    drawInfo.GachaItemInfo.AddRange(baseItemPool);
                    drawGroupInfo.GachaDrawList.Add(drawInfo);

                    // Parse payment options for this draw count
                    foreach (var jPayment in jDrawOption.GetProperty("payments").EnumerateArray())
                    {
                        WalletType walletType;
                        if (!Enum.TryParse(jPayment.GetProperty("wallet_type").GetString(), true, out walletType))
                        {
                            Logger.Error($"Failed to parse WalletType for {path}. Skipping.");
                            continue;
                        }

                        if (walletType != WalletType.GoldenGemstones && walletType != WalletType.SilverTickets)
                        {
                            Logger.Error($"The currency '{walletType}' is not a valid loot box currency. Skipping.");
                            continue;
                        }

                        var drawGroupId = jPayment.GetProperty("group_id").GetUInt32();
                        var settlementInfo = new CDataGachaSettlementInfo()
                        {
                            Id = (walletType == WalletType.GoldenGemstones) ? 1u : 2u,
                            DrawGroupId = drawGroupId,
                            Price = jPayment.GetProperty("price").GetUInt32(),
                            BasePrice = jPayment.GetProperty("base_price").GetUInt32(),
                            PurchaseNum = jPayment.GetProperty("purchase_num").GetUInt32(),
                            PurchaseMaxNum = jPayment.GetProperty("purchase_max_num").GetUInt32(),
                            SpecialPriceNum = jPayment.GetProperty("special_price_num").GetUInt32(),
                            SpecialPriceMaxNum = jPayment.GetProperty("special_price_max_num").GetUInt32(),
                            Unk1 = jPayment.GetProperty("unk1").GetUInt32(),
                        };

                        // Store num_draws mapping for server-side use
                        gachaInfo.DrawGroupNumDraws[drawGroupId] = numDraws;

                        drawGroupInfo.GachaSettlementList.Add(settlementInfo);
                    }

                    gachaInfo.DrawGroups.Add(drawGroupInfo);
                }

                asset.GachaInfoList[gachaInfo.Id] = gachaInfo;
            }

            return asset;
        }
    }
}
