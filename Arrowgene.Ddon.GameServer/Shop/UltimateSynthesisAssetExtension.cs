using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.Structure;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Shop
{
    public static class UltimateSynthesisAssetExtension
    {
        /// <summary>
        /// Index offset to distinguish UltimateSynthesis from LimitBreak indices.
        /// LimitBreak uses indices 1-100, UltimateSynthesis uses 101+.
        /// </summary>
        public const ushort IndexOffset = 100;

        public static List<CDataEquipEnhanceLotteryOption> ToLotteryExampleList(this UltimateSynthesisAsset asset)
        {
            var results = new List<CDataEquipEnhanceLotteryOption>();

            ushort listingIndex = IndexOffset + 1; // Start at 101
            foreach (var category in asset.Categories)
            {
                foreach (var paymentOption in category.PaymentOptions)
                {
                    var option = new CDataEquipEnhanceLotteryOption()
                    {
                        RowTitle = $"{asset.ListTitle} {paymentOption.Label}",
                        Index = listingIndex,
                        Category = (byte)listingIndex,
                        // AttemptModifier = 1 means "first time only" - only available for initial attempt
                        AttemptModifier = paymentOption.IsFirstAttemptOnly ? (ushort)1 : (ushort)0,
                        WalletPointCost = new List<CDataWalletPoint>()
                        {
                            new CDataWalletPoint()
                            {
                                Type = paymentOption.WalletType,
                                Value = paymentOption.Cost
                            }
                        },
                        ShopTypeListings = category.ShopListings.Select(x => new CDataCommonU8(x)).ToList(),
                    };

                    // Add stat lottery as lottery candidates
                    if (category.StatLottery.Count > 0)
                    {
                        var lotteryCandidates = new CDataS2CEquipEnhancedGetPacksResUnk0Unk10()
                        {
                            EffectParamList = new List<CDataS2CEquipEnhancedGetPacksResUnk0Unk10Unk1>()
                        };

                        foreach (var stat in category.StatLottery)
                        {
                            lotteryCandidates.EffectParamList.Add(new CDataS2CEquipEnhancedGetPacksResUnk0Unk10Unk1()
                            {
                                BuffId = stat.Rolls.Last(),
                                Unk1 = 1,
                                Unk2 = 2
                            });
                        }
                        option.MainSuccessExample.Add(lotteryCandidates);
                    }
                    // Fallback to recipe info if available
                    else if (category.Recipes.Count > 0)
                    {
                        var lotteryCandidates = new CDataS2CEquipEnhancedGetPacksResUnk0Unk10()
                        {
                            EffectParamList = new List<CDataS2CEquipEnhancedGetPacksResUnk0Unk10Unk1>()
                        };

                        foreach (var recipe in category.Recipes)
                        {
                            lotteryCandidates.EffectParamList.Add(new CDataS2CEquipEnhancedGetPacksResUnk0Unk10Unk1()
                            {
                                BuffId = (ushort)recipe.ResultItemId,
                                Unk1 = 1,
                                Unk2 = 2
                            });
                        }
                        option.MainSuccessExample.Add(lotteryCandidates);
                    }

                    // Add Special Bonus stats if enabled
                    if (asset.SpecialBonus.Enabled && asset.SpecialBonus.StatLottery.Count > 0)
                    {
                        var specialBonusCandidates = new CDataS2CEquipEnhancedGetPacksResUnk0Unk10()
                        {
                            Unk0 = 1, // Mark as special bonus section
                            EffectParamList = new List<CDataS2CEquipEnhancedGetPacksResUnk0Unk10Unk1>()
                        };

                        foreach (var stat in asset.SpecialBonus.StatLottery)
                        {
                            specialBonusCandidates.EffectParamList.Add(new CDataS2CEquipEnhancedGetPacksResUnk0Unk10Unk1()
                            {
                                BuffId = stat.Rolls.Last(),
                                Unk1 = 1,
                                Unk2 = 2
                            });
                        }
                        option.MainSuccessExample.Add(specialBonusCandidates);
                    }

                    listingIndex++;
                    results.Add(option);
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the category for a given index.
        /// </summary>
        public static UltimateSynthesisCategory GetCategoryForIndex(this UltimateSynthesisAsset asset, ushort index)
        {
            // Adjust for offset
            ushort adjustedIndex = (ushort)(index - IndexOffset);
            ushort listingIndex = 1;
            foreach (var category in asset.Categories)
            {
                foreach (var paymentOption in category.PaymentOptions)
                {
                    if (listingIndex == adjustedIndex)
                    {
                        return category;
                    }
                    listingIndex++;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the specific payment option for a given index.
        /// </summary>
        public static UltimateSynthesisPaymentOption GetPaymentOptionForIndex(this UltimateSynthesisAsset asset, ushort index)
        {
            // Adjust for offset
            ushort adjustedIndex = (ushort)(index - IndexOffset);
            ushort listingIndex = 1;
            foreach (var category in asset.Categories)
            {
                foreach (var paymentOption in category.PaymentOptions)
                {
                    if (listingIndex == adjustedIndex)
                    {
                        return paymentOption;
                    }
                    listingIndex++;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if the given index is for Ultimate Synthesis (101+) rather than Limit Break (1-100).
        /// </summary>
        public static bool IsUltimateSynthesisIndex(ushort index)
        {
            return index > IndexOffset;
        }

        /// <summary>
        /// Gets a recipe by its source item ID.
        /// </summary>
        public static UltimateSynthesisRecipe GetRecipeForSourceItem(this UltimateSynthesisAsset asset, uint sourceItemId)
        {
            foreach (var category in asset.Categories)
            {
                var recipe = category.Recipes.FirstOrDefault(r => r.SourceItemId == sourceItemId);
                if (recipe != null)
                {
                    return recipe;
                }
            }
            return null;
        }
    }
}
