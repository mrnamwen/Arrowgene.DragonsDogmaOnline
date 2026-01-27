using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;
using System;
using System.Text.Json;

namespace Arrowgene.Ddon.Shared.AssetReader
{
    public class UltimateSynthesisAssetReader : IAssetDeserializer<UltimateSynthesisAsset>
    {
        private static readonly ILogger Logger = LogProvider.Logger(typeof(UltimateSynthesisAssetReader));

        public UltimateSynthesisAsset ReadPath(string path)
        {
            Logger.Info($"Reading {path}");

            UltimateSynthesisAsset asset = new();

            string json = Util.ReadAllText(path);
            JsonDocument document = JsonDocument.Parse(json);

            asset.ListTitle = document.RootElement.GetProperty("title").GetString() ?? string.Empty;

            if (document.RootElement.TryGetProperty("categories", out JsonElement jCategories))
            {
                foreach (var jCategory in jCategories.EnumerateArray())
                {
                    UltimateSynthesisCategory category = new();

                    if (jCategory.TryGetProperty("key", out JsonElement jKey))
                    {
                        category.Key = jKey.GetUInt16();
                    }

                    if (jCategory.TryGetProperty("index", out JsonElement jIndex))
                    {
                        category.Index = jIndex.GetByte();
                    }

                    if (jCategory.TryGetProperty("shop_listings", out JsonElement jShopListings))
                    {
                        foreach (var jListing in jShopListings.EnumerateArray())
                        {
                            category.ShopListings.Add(jListing.GetByte());
                        }
                    }

                    if (jCategory.TryGetProperty("premium_payments", out JsonElement jPremiumPayments))
                    {
                        foreach (var jPremiumCurrency in jPremiumPayments.EnumerateArray())
                        {
                            if (Enum.TryParse(jPremiumCurrency.GetString(), true, out WalletType walletType))
                            {
                                category.PremiumCurrencies.Add(walletType);
                            }
                        }
                    }

                    if (jCategory.TryGetProperty("payment_options", out JsonElement jPaymentOptions))
                    {
                        foreach (var jPaymentOption in jPaymentOptions.EnumerateArray())
                        {
                            if (!Enum.TryParse(jPaymentOption.GetProperty("type").GetString(), true, out WalletType walletType))
                            {
                                continue;
                            }

                            var paymentOption = new UltimateSynthesisPaymentOption
                            {
                                WalletType = walletType,
                                Label = jPaymentOption.GetProperty("name").GetString() ?? string.Empty,
                                Cost = jPaymentOption.GetProperty("amount").GetUInt32(),
                                IsFirstAttemptOnly = jPaymentOption.TryGetProperty("is_first_attempt_only", out JsonElement jFirstAttempt) && jFirstAttempt.GetBoolean()
                            };

                            category.PaymentOptions.Add(paymentOption);
                        }
                    }

                    if (jCategory.TryGetProperty("recipes", out JsonElement jRecipes))
                    {
                        foreach (var jRecipe in jRecipes.EnumerateArray())
                        {
                            UltimateSynthesisRecipe recipe = new();

                            if (jRecipe.TryGetProperty("source_item_id", out JsonElement jSourceItemId))
                            {
                                recipe.SourceItemId = jSourceItemId.GetUInt32();
                            }

                            if (jRecipe.TryGetProperty("result_item_id", out JsonElement jResultItemId))
                            {
                                recipe.ResultItemId = jResultItemId.GetUInt32();
                            }

                            if (jRecipe.TryGetProperty("material_item_ids", out JsonElement jMaterialItemIds))
                            {
                                foreach (var jMaterialId in jMaterialItemIds.EnumerateArray())
                                {
                                    recipe.MaterialItemIds.Add(jMaterialId.GetUInt32());
                                }
                            }

                            if (jRecipe.TryGetProperty("gold_cost", out JsonElement jGoldCost))
                            {
                                recipe.GoldCost = jGoldCost.GetUInt32();
                            }

                            category.Recipes.Add(recipe);
                        }
                    }

                    if (jCategory.TryGetProperty("stats", out JsonElement jStats))
                    {
                        foreach (var jStatList in jStats.EnumerateArray())
                        {
                            var stat = new UltimateSynthesisStatLottery()
                            {
                                MinGreatSuccessIndex = jStatList.GetProperty("min_great_success_index").GetUInt32()
                            };

                            if (jStatList.TryGetProperty("name", out JsonElement jStatName))
                            {
                                stat.Name = jStatName.GetString() ?? string.Empty;
                            }

                            foreach (var jId in jStatList.GetProperty("ids").EnumerateArray())
                            {
                                stat.Rolls.Add(jId.GetUInt16());
                            }
                            category.StatLottery.Add(stat);
                        }
                    }

                    asset.Categories.Add(category);
                }
            }

            // Parse Special Bonus section
            if (document.RootElement.TryGetProperty("special_bonus", out JsonElement jSpecialBonus))
            {
                asset.SpecialBonus.Enabled = jSpecialBonus.TryGetProperty("enabled", out JsonElement jEnabled) && jEnabled.GetBoolean();

                if (jSpecialBonus.TryGetProperty("stats", out JsonElement jSpecialStats))
                {
                    foreach (var jStatList in jSpecialStats.EnumerateArray())
                    {
                        var stat = new UltimateSynthesisStatLottery()
                        {
                            MinGreatSuccessIndex = jStatList.GetProperty("min_great_success_index").GetUInt32()
                        };

                        if (jStatList.TryGetProperty("name", out JsonElement jStatName))
                        {
                            stat.Name = jStatName.GetString() ?? string.Empty;
                        }

                        foreach (var jId in jStatList.GetProperty("ids").EnumerateArray())
                        {
                            stat.Rolls.Add(jId.GetUInt16());
                        }
                        asset.SpecialBonus.StatLottery.Add(stat);
                    }
                }
            }

            return asset;
        }
    }
}
