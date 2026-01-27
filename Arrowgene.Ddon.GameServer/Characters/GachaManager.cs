using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Characters
{
    /// <summary>
    /// Manages gacha (treasure lot) operations including draws, probability calculations,
    /// and reward distribution.
    /// </summary>
    public class GachaManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(GachaManager));
        private readonly DdonGameServer _Server;
        private readonly Random _Random;

        public GachaManager(DdonGameServer server)
        {
            _Server = server;
            _Random = new Random();
        }

        /// <summary>
        /// Gets the gacha asset from the server's asset repository.
        /// </summary>
        public GachaAsset GachaAsset => _Server.AssetRepository.GachaAsset;

        /// <summary>
        /// Checks if a gacha exists and is currently available.
        /// </summary>
        /// <param name="gachaId">The gacha ID to check.</param>
        /// <returns>True if the gacha exists and is within its availability window.</returns>
        public bool IsGachaAvailable(uint gachaId)
        {
            if (!GachaAsset.GachaInfoList.ContainsKey(gachaId))
            {
                return false;
            }

            var gacha = GachaAsset.GachaInfoList[gachaId];
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return now >= gacha.Begin && now <= gacha.End;
        }

        /// <summary>
        /// Gets all currently available gachas.
        /// </summary>
        /// <returns>A list of available gacha info.</returns>
        public List<CDataGachaInfo> GetAvailableGachas()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return GachaAsset.GachaInfoList.Values
                .Where(g => now >= g.Begin && now <= g.End)
                .ToList();
        }

        /// <summary>
        /// Performs a gacha draw and returns the won items.
        /// </summary>
        /// <param name="gachaId">The gacha ID to draw from.</param>
        /// <param name="settlementId">The settlement/payment method ID (1=GG, 2=Silver Tickets).</param>
        /// <param name="numDraws">Number of draws to perform.</param>
        /// <returns>List of drawn items.</returns>
        public List<CDataGachaItemInfo> DrawGacha(uint gachaId, uint settlementId, int numDraws = 1)
        {
            var result = new List<CDataGachaItemInfo>();

            if (!GachaAsset.GachaInfoList.ContainsKey(gachaId))
            {
                Logger.Error($"Attempted to draw from non-existent gacha ID: {gachaId}");
                return result;
            }

            var gacha = GachaAsset.GachaInfoList[gachaId];

            for (int i = 0; i < numDraws; i++)
            {
                foreach (var drawGroup in gacha.DrawGroups)
                {
                    if (drawGroup.GachaDrawList.Count > 0)
                    {
                        var drawList = drawGroup.GachaDrawList[0];
                        var drawnItem = PerformWeightedDraw(drawList.GachaItemInfo);
                        if (drawnItem != null)
                        {
                            result.Add(drawnItem);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Performs a weighted random draw from a list of items based on their probability.
        /// </summary>
        /// <param name="items">List of items with probabilities.</param>
        /// <returns>The drawn item, or null if the list is empty.</returns>
        private CDataGachaItemInfo PerformWeightedDraw(List<CDataGachaItemInfo> items)
        {
            if (items == null || items.Count == 0)
            {
                return null;
            }

            // Calculate total weight
            double totalWeight = items.Sum(i => i.Probability);

            // If all probabilities are 0, use equal weighting
            if (totalWeight <= 0)
            {
                return items[_Random.Next(items.Count)];
            }

            // Generate random value
            double randomValue = _Random.NextDouble() * totalWeight;

            // Find the selected item
            double cumulativeWeight = 0;
            foreach (var item in items)
            {
                cumulativeWeight += item.Probability;
                if (randomValue <= cumulativeWeight)
                {
                    return item;
                }
            }

            // Fallback to last item (shouldn't happen normally)
            return items[items.Count - 1];
        }

        /// <summary>
        /// Gets the wallet type for a settlement ID.
        /// </summary>
        /// <param name="settlementId">The settlement ID (1=GG, 2=Silver Tickets).</param>
        /// <returns>The corresponding wallet type, or null if invalid.</returns>
        public WalletType? GetWalletTypeForSettlement(uint settlementId)
        {
            return settlementId switch
            {
                1 => WalletType.GoldenGemstones,
                2 => WalletType.SilverTickets,
                _ => null
            };
        }

        /// <summary>
        /// Gets the price for a specific gacha and settlement method.
        /// </summary>
        /// <param name="gachaId">The gacha ID.</param>
        /// <param name="settlementId">The settlement ID.</param>
        /// <returns>The price, or 0 if not found.</returns>
        public uint GetGachaPrice(uint gachaId, uint settlementId)
        {
            if (!GachaAsset.GachaInfoList.ContainsKey(gachaId))
            {
                return 0;
            }

            var gacha = GachaAsset.GachaInfoList[gachaId];
            foreach (var drawGroup in gacha.DrawGroups)
            {
                var settlement = drawGroup.GachaSettlementList.FirstOrDefault(s => s.Id == settlementId);
                if (settlement != null)
                {
                    return settlement.Price;
                }
            }

            return 0;
        }

        /// <summary>
        /// Validates if a character can afford a gacha draw.
        /// </summary>
        /// <param name="character">The character making the purchase.</param>
        /// <param name="gachaId">The gacha ID.</param>
        /// <param name="settlementId">The settlement/payment method.</param>
        /// <returns>True if the character can afford the draw.</returns>
        public bool CanAffordGacha(Character character, uint gachaId, uint settlementId)
        {
            var walletType = GetWalletTypeForSettlement(settlementId);
            if (walletType == null)
            {
                return false;
            }

            var price = GetGachaPrice(gachaId, settlementId);
            if (price == 0)
            {
                return false;
            }

            return _Server.WalletManager.GetWalletAmount(character, walletType.Value) >= price;
        }
    }
}
