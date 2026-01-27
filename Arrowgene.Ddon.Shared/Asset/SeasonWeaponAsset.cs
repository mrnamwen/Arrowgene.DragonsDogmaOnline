using System.Collections.Generic;

namespace Arrowgene.Ddon.Shared.Asset
{
    /// <summary>
    /// Asset containing season weapon definitions for the Ultimate Synthesis Special Bonus system.
    /// Maps weapon item IDs to their season numbers to determine eligibility for special bonuses.
    /// </summary>
    public class SeasonWeaponAsset
    {
        public SeasonWeaponAsset()
        {
            Weapons = new List<SeasonWeaponEntry>();
        }

        /// <summary>
        /// List of all season weapons.
        /// </summary>
        public List<SeasonWeaponEntry> Weapons { get; set; }

        /// <summary>
        /// Gets the season number for a given weapon item ID.
        /// Returns 0 if the weapon is not a season weapon.
        /// </summary>
        public uint GetSeasonForWeapon(uint itemId)
        {
            foreach (var weapon in Weapons)
            {
                if (weapon.ItemId == itemId)
                {
                    return weapon.Season;
                }
            }
            return 0;
        }

        /// <summary>
        /// Checks if the given weapon is from the previous season relative to a target season.
        /// </summary>
        public bool IsPreviousSeasonWeapon(uint itemId, uint currentSeason)
        {
            if (currentSeason <= 1)
            {
                return false;
            }

            uint weaponSeason = GetSeasonForWeapon(itemId);
            return weaponSeason == currentSeason - 1;
        }

        /// <summary>
        /// Gets all weapons for a specific season.
        /// </summary>
        public List<SeasonWeaponEntry> GetWeaponsForSeason(uint season)
        {
            var result = new List<SeasonWeaponEntry>();
            foreach (var weapon in Weapons)
            {
                if (weapon.Season == season)
                {
                    result.Add(weapon);
                }
            }
            return result;
        }
    }

    /// <summary>
    /// Represents a single season weapon entry.
    /// </summary>
    public class SeasonWeaponEntry
    {
        /// <summary>
        /// The item ID of the weapon.
        /// </summary>
        public uint ItemId { get; set; }

        /// <summary>
        /// The season number this weapon belongs to (e.g., 1, 2, 3).
        /// </summary>
        public uint Season { get; set; }

        /// <summary>
        /// Optional comment/name for documentation purposes.
        /// </summary>
        public string Comment { get; set; } = string.Empty;
    }
}
