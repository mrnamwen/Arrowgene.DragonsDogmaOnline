using Arrowgene.Ddon.GameServer.Characters;
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared;
using Arrowgene.Ddon.Shared.Asset;
using Arrowgene.Ddon.Shared.Entity.Structure;
using Arrowgene.Ddon.Shared.Model;
using Arrowgene.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Arrowgene.Ddon.GameServer.Shop
{
    /// <summary>
    /// Manages the Special Bonus system for Ultimate Synthesis.
    /// Special Bonus is triggered when the player possesses the previous season's
    /// strongest weapon at 4-star enhancement, providing an additional stat roll.
    /// </summary>
    public class SpecialBonusManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(SpecialBonusManager));

        private readonly DdonGameServer _server;

        public SpecialBonusManager(DdonGameServer server)
        {
            _server = server;
        }

        /// <summary>
        /// Checks if the character is eligible for the Special Bonus based on the Ultimate Synthesis configuration.
        /// Eligibility requires possessing an item from the eligible item list at 4-star enhancement.
        /// </summary>
        /// <param name="character">The character to check eligibility for.</param>
        /// <param name="eligibleItemIds">List of item IDs that qualify for the special bonus.</param>
        /// <returns>True if the character is eligible for the special bonus.</returns>
        public bool IsEligibleForSpecialBonus(Character character, IEnumerable<uint> eligibleItemIds)
        {
            var specialBonus = _server.AssetRepository.UltimateSynthesisAsset.SpecialBonus;
            if (!specialBonus.Enabled)
            {
                return false;
            }

            if (eligibleItemIds == null || !eligibleItemIds.Any())
            {
                return false;
            }

            // Search all equipment storages for an eligible item with 4-star enhancement
            foreach (var itemId in eligibleItemIds)
            {
                var foundItems = character.Storage.FindItemsByIdInStorage(
                    ItemManager.EquipmentStorages,
                    (ItemId)itemId);

                foreach (var (storageType, (index, item, amount)) in foundItems)
                {
                    if (item.PlusValue >= 4)
                    {
                        Logger.Debug($"Character {character.CharacterId} is eligible for Special Bonus " +
                            $"with item {item.ItemId} at +{item.PlusValue} in {storageType}");
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the character is eligible for the Special Bonus using the global configuration.
        /// </summary>
        /// <param name="character">The character to check eligibility for.</param>
        /// <returns>True if the character is eligible for the special bonus.</returns>
        public bool IsEligibleForSpecialBonus(Character character)
        {
            var specialBonus = _server.AssetRepository.UltimateSynthesisAsset.SpecialBonus;
            if (!specialBonus.Enabled)
            {
                return false;
            }

            // If no specific eligible items are configured in the asset, return false
            // The eligibility list should be provided by the caller or configured per category
            return false;
        }

        /// <summary>
        /// Rolls the special bonus stat for an item using the configured stat lottery.
        /// </summary>
        /// <param name="forceGreatSuccess">If true, forces a great success roll (higher stats).</param>
        /// <returns>The rolled stat value, or null if special bonus is not enabled.</returns>
        public ushort? RollSpecialBonusStat(bool forceGreatSuccess)
        {
            var specialBonus = _server.AssetRepository.UltimateSynthesisAsset.SpecialBonus;
            if (!specialBonus.Enabled || specialBonus.StatLottery.Count == 0)
            {
                return null;
            }

            var statRolls = specialBonus.StatLottery.OrderBy(x => Random.Shared.Next()).First();
            if (statRolls.Rolls.Count == 0)
            {
                return null;
            }

            ushort statRoll;
            if (forceGreatSuccess)
            {
                int minIndex = (int)statRolls.MinGreatSuccessIndex;
                statRoll = statRolls.Rolls[Random.Shared.Next(minIndex, statRolls.Rolls.Count)];
            }
            else
            {
                statRoll = statRolls.Rolls.GetWeightedRandomElement(
                    _server.GameSettings.GameServerSettings.EquipmentLimitBreakBias);
            }

            return statRoll;
        }

        /// <summary>
        /// Applies the special bonus to an item by adding the stat to its AddStatusParamList.
        /// </summary>
        /// <param name="item">The item to apply the special bonus to.</param>
        /// <param name="statRoll">The stat roll value to apply.</param>
        /// <returns>The CDataAddStatusParam that was added or updated.</returns>
        public CDataAddStatusParam ApplySpecialBonus(Item item, ushort statRoll)
        {
            var param = item.AddStatusParamList.Find(x => x.EnhanceType == EquipEnhanceType.UltimateSynthesisSpecialBonus);
            if (param is null)
            {
                param = new CDataAddStatusParam()
                {
                    EnhanceId = statRoll,
                    EnhanceType = EquipEnhanceType.UltimateSynthesisSpecialBonus
                };
                item.AddStatusParamList.Add(param);
            }
            else
            {
                param.EnhanceId = statRoll;
            }

            return param;
        }

        /// <summary>
        /// Applies the special bonus and persists it to the database.
        /// </summary>
        /// <param name="characterId">The character ID who owns the item.</param>
        /// <param name="item">The item to apply the special bonus to.</param>
        /// <param name="statRoll">The stat roll value to apply.</param>
        /// <param name="connection">Optional database connection for transaction support.</param>
        /// <returns>The CDataAddStatusParam that was added or updated.</returns>
        public CDataAddStatusParam ApplyAndPersistSpecialBonus(uint characterId, Item item, ushort statRoll, DbConnection connection = null)
        {
            var param = ApplySpecialBonus(item, statRoll);
            _server.Database.UpsertEquipmentLimitBreakRecord(characterId, item.UId, param, connection);
            return param;
        }

        /// <summary>
        /// Gets the stat lottery options for the special bonus.
        /// </summary>
        /// <returns>The list of stat lottery options, or an empty list if not enabled.</returns>
        public List<UltimateSynthesisStatLottery> GetStatLotteryOptions()
        {
            var specialBonus = _server.AssetRepository.UltimateSynthesisAsset.SpecialBonus;
            if (!specialBonus.Enabled)
            {
                return new List<UltimateSynthesisStatLottery>();
            }

            return specialBonus.StatLottery;
        }

        /// <summary>
        /// Checks if the special bonus system is enabled in the configuration.
        /// </summary>
        /// <returns>True if special bonus is enabled.</returns>
        public bool IsEnabled()
        {
            return _server.AssetRepository.UltimateSynthesisAsset.SpecialBonus.Enabled;
        }
    }
}
