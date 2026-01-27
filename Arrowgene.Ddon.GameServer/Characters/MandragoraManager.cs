using Arrowgene.Ddon.Database.Model;
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
    public class MandragoraManager
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(MandragoraManager));

        private readonly DdonGameServer _Server;

        // Rarity level names (static data)
        private readonly List<CDataMyMandragoraRarityLevel> _RarityLevels;

        // Species category names (static data)
        private readonly List<CDataMyMandragoraSpeciesCategory> _SpeciesCategories;

        public MandragoraManager(DdonGameServer server)
        {
            _Server = server;

            // Initialize static rarity levels
            _RarityLevels = new List<CDataMyMandragoraRarityLevel>
            {
                new CDataMyMandragoraRarityLevel { RarityId = MandragoraRarity.LimitedRare, Rarity = "Limited Rare" },
                new CDataMyMandragoraRarityLevel { RarityId = MandragoraRarity.Common, Rarity = "Common" },
                new CDataMyMandragoraRarityLevel { RarityId = MandragoraRarity.Uncommon, Rarity = "Uncommon" },
                new CDataMyMandragoraRarityLevel { RarityId = MandragoraRarity.Rare, Rarity = "Rare" },
                new CDataMyMandragoraRarityLevel { RarityId = MandragoraRarity.MysticRare, Rarity = "Mystic Rare" }
            };

            // Initialize static species categories
            _SpeciesCategories = new List<CDataMyMandragoraSpeciesCategory>
            {
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Normal, CategoryName = "Normal Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Chilli, CategoryName = "Chilli Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Albino, CategoryName = "Albino Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Charcoal, CategoryName = "Charcoal Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Veggie, CategoryName = "Veggie Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Armored, CategoryName = "Armored Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Clothed, CategoryName = "Clothed Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Flowering, CategoryName = "Flowering Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Barbarian, CategoryName = "Barbarian Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Scroll, CategoryName = "Scroll Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Helmet, CategoryName = "Helmet Species" },
                new CDataMyMandragoraSpeciesCategory { SpeciesCategory = MandragoraSpeciesCategory.Special, CategoryName = "Special Species" }
            };
        }

        #region Mandragora Ownership

        /// <summary>
        /// Gets all mandragoras owned by a character.
        /// </summary>
        public List<CDataMyMandragora> GetCharacterMandragoras(uint characterId)
        {
            var mandragoras = _Server.Database.SelectMandragoras(characterId);
            return mandragoras.Select(m => m.ToCDataMyMandragora()).ToList();
        }

        /// <summary>
        /// Gets furniture items for all owned mandragoras.
        /// </summary>
        public List<CDataMyMandragoraFurnitureItem> GetCharacterMandragoraFurniture(uint characterId)
        {
            var mandragoras = _Server.Database.SelectMandragoras(characterId);
            return mandragoras.Select(m => m.ToCDataMyMandragoraFurnitureItem()).ToList();
        }

        /// <summary>
        /// Creates a new mandragora for a character.
        /// </summary>
        public bool CreateMandragora(uint characterId, uint mandragoraId, string name, uint speciesIndex, MandragoraSpeciesCategory category, uint furnitureItemId)
        {
            var mandragora = new Mandragora
            {
                CharacterId = characterId,
                MandragoraId = mandragoraId,
                Name = name,
                SpeciesIndex = speciesIndex,
                SpeciesCategory = category,
                FurnitureItemId = furnitureItemId,
                GrowthLevel = 1
            };

            bool success = _Server.Database.InsertMandragora(mandragora);
            if (success)
            {
                Logger.Info($"Created mandragora {mandragoraId} '{name}' for character {characterId}");

                // Also register the species discovery
                DiscoverSpecies(characterId, speciesIndex, category);
            }
            return success;
        }

        /// <summary>
        /// Updates an existing mandragora.
        /// </summary>
        public bool UpdateMandragora(uint characterId, uint mandragoraId, string name = null, uint? speciesIndex = null, MandragoraSpeciesCategory? category = null)
        {
            var mandragora = _Server.Database.SelectMandragora(characterId, mandragoraId);
            if (mandragora == null)
            {
                return false;
            }

            if (name != null) mandragora.Name = name;
            if (speciesIndex.HasValue)
            {
                mandragora.SpeciesIndex = speciesIndex.Value;
                // Register discovery of new species
                if (category.HasValue)
                {
                    mandragora.SpeciesCategory = category.Value;
                    DiscoverSpecies(characterId, speciesIndex.Value, category.Value);
                }
            }

            return _Server.Database.UpdateMandragora(mandragora);
        }

        /// <summary>
        /// Deletes a mandragora.
        /// </summary>
        public bool DeleteMandragora(uint characterId, uint mandragoraId)
        {
            return _Server.Database.DeleteMandragora(characterId, mandragoraId);
        }

        /// <summary>
        /// Gets free mandragora IDs (slots that can be used for new mandragoras).
        /// Based on furniture items the character owns.
        /// </summary>
        public List<CDataCommonU8> GetFreeMandragoraIds(uint characterId)
        {
            var result = new List<CDataCommonU8>();
            var asset = _Server.AssetRepository.MandragoraAsset;
            var ownedMandragoras = _Server.Database.SelectMandragoras(characterId);
            var ownedIds = ownedMandragoras.Select(m => m.MandragoraId).ToHashSet();

            // Return IDs for mandragora furniture items (1, 2, 3 by default)
            foreach (var furniture in asset.FurnitureItems)
            {
                if (!ownedIds.Contains(furniture.MandragoraId))
                {
                    result.Add(new CDataCommonU8((byte)furniture.MandragoraId));
                }
            }

            return result;
        }

        #endregion

        #region Species Discovery

        /// <summary>
        /// Discovers a species for a character.
        /// </summary>
        public bool DiscoverSpecies(uint characterId, uint speciesIndex, MandragoraSpeciesCategory category)
        {
            var existingDiscoveries = _Server.Database.SelectMandragoraSpeciesDiscoveriesByCategory(characterId, category);
            if (existingDiscoveries.Any(d => d.SpeciesIndex == speciesIndex))
            {
                // Already discovered
                return false;
            }

            // Get rarity from asset
            var asset = _Server.AssetRepository.MandragoraAsset;
            var speciesDefinition = asset.Species.FirstOrDefault(s => s.Index == speciesIndex);
            var rarity = speciesDefinition?.Rarity ?? MandragoraRarity.Common;

            var discovery = new MandragoraSpeciesDiscovery
            {
                CharacterId = characterId,
                SpeciesIndex = speciesIndex,
                SpeciesCategory = category,
                Rarity = rarity,
                IsNew = true,
                DiscoveredDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            bool success = _Server.Database.InsertMandragoraSpeciesDiscovery(discovery);
            if (success)
            {
                // Also try to register as first discovery server-wide
                var character = _Server.ClientLookup.GetClientByCharacterId(characterId)?.Character;
                if (character != null)
                {
                    _Server.Database.InsertOrIgnoreMandragoraFirstDiscovery(
                        speciesIndex,
                        characterId,
                        character.FirstName + " " + character.LastName,
                        discovery.DiscoveredDate
                    );
                }
                Logger.Info($"Character {characterId} discovered species {speciesIndex} (category {category})");
            }

            return success;
        }

        /// <summary>
        /// Gets species list for a specific category with discovery status.
        /// </summary>
        public List<CDataMyMandragoraSpecies> GetSpeciesList(uint characterId, MandragoraSpeciesCategory category)
        {
            var asset = _Server.AssetRepository.MandragoraAsset;
            var discoveries = _Server.Database.SelectMandragoraSpeciesDiscoveriesByCategory(characterId, category);
            var discoveryMap = discoveries.ToDictionary(d => d.SpeciesIndex);

            var result = new List<CDataMyMandragoraSpecies>();

            // Get all species definitions for this category from asset
            foreach (var speciesDefinition in asset.Species.Where(s => s.Category == category))
            {
                if (discoveryMap.TryGetValue(speciesDefinition.Index, out var discovery))
                {
                    // Character has discovered this species
                    string firstDiscoverer = _Server.Database.SelectMandragoraFirstDiscoverer(speciesDefinition.Index)
                        ?? speciesDefinition.Index.ToString();
                    result.Add(discovery.ToCDataMyMandragoraSpecies(firstDiscoverer));
                }
                else
                {
                    // Species not yet discovered by this character
                    result.Add(new CDataMyMandragoraSpecies
                    {
                        Index = speciesDefinition.Index,
                        Unk1 = 0,
                        Rarity = speciesDefinition.Rarity,
                        Unk3 = 0,
                        Visible = true,
                        Unk5 = false,
                        FirstDiscovery = speciesDefinition.Index.ToString(),
                        DiscoveredDate = 0,
                        New = false
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the species categories with discovered counts.
        /// </summary>
        public List<CDataMyMandragoraSpeciesCategory> GetSpeciesCategories(uint characterId)
        {
            var discoveries = _Server.Database.SelectMandragoraSpeciesDiscoveries(characterId);
            var countByCategory = discoveries
                .GroupBy(d => d.SpeciesCategory)
                .ToDictionary(g => g.Key, g => g.Count());

            var result = new List<CDataMyMandragoraSpeciesCategory>();
            foreach (var category in _SpeciesCategories)
            {
                countByCategory.TryGetValue(category.SpeciesCategory, out int count);
                result.Add(new CDataMyMandragoraSpeciesCategory
                {
                    SpeciesCategory = category.SpeciesCategory,
                    CategoryName = category.CategoryName,
                    DiscoveredSpeciesNumMaybe = (uint)count
                });
            }

            return result;
        }

        /// <summary>
        /// Marks a species discovery as no longer new.
        /// </summary>
        public bool MarkSpeciesAsViewed(uint characterId, uint speciesIndex)
        {
            var discoveries = _Server.Database.SelectMandragoraSpeciesDiscoveries(characterId);
            var discovery = discoveries.FirstOrDefault(d => d.SpeciesIndex == speciesIndex);
            if (discovery == null || !discovery.IsNew)
            {
                return false;
            }

            discovery.IsNew = false;
            return _Server.Database.UpdateMandragoraSpeciesDiscovery(discovery);
        }

        #endregion

        #region Craft System

        // In-memory tracking of active crafts (Character ID -> Mandragora ID -> ActiveCraft)
        private readonly Dictionary<uint, Dictionary<uint, MandragoraCraft>> _ActiveCrafts = new();

        public class MandragoraCraft
        {
            public uint CharacterId { get; set; }
            public uint MandragoraId { get; set; }
            public uint RecipeId { get; set; }
            public uint ResultItemId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            /// <summary>
            /// List of fertilizer item IDs used in this craft (influences species evolution).
            /// </summary>
            public List<uint> FertilizerItemIds { get; set; } = new();
        }

        /// <summary>
        /// Starts a craft with a mandragora.
        /// </summary>
        public MandragoraCraft StartCraft(uint characterId, uint mandragoraId, uint recipeId, List<uint> fertilizerItemIds = null)
        {
            var asset = _Server.AssetRepository.MandragoraAsset;
            var recipe = asset.CraftRecipes.FirstOrDefault(r => r.RecipeId == recipeId);
            if (recipe == null)
            {
                Logger.Error($"Recipe {recipeId} not found");
                return null;
            }

            // Check if mandragora exists and belongs to character
            var mandragora = _Server.Database.SelectMandragora(characterId, mandragoraId);
            if (mandragora == null)
            {
                Logger.Error($"Mandragora {mandragoraId} not found for character {characterId}");
                return null;
            }

            // Check if mandragora is already crafting
            lock (_ActiveCrafts)
            {
                if (_ActiveCrafts.TryGetValue(characterId, out var charCrafts) &&
                    charCrafts.ContainsKey(mandragoraId))
                {
                    Logger.Error($"Mandragora {mandragoraId} is already crafting");
                    return null;
                }

                // Apply speed factor from settings (lower = faster)
                double speedFactor = _Server.GameSettings.GameServerSettings.MandragoraCraftSpeedFactor;
                int adjustedCraftTime = (int)(recipe.CraftTime * speedFactor);
                if (adjustedCraftTime < 1) adjustedCraftTime = 1; // Minimum 1 second

                var craft = new MandragoraCraft
                {
                    CharacterId = characterId,
                    MandragoraId = mandragoraId,
                    RecipeId = recipeId,
                    ResultItemId = recipe.ResultItemId,
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow.AddSeconds(adjustedCraftTime),
                    FertilizerItemIds = fertilizerItemIds ?? new List<uint>()
                };

                if (!_ActiveCrafts.ContainsKey(characterId))
                {
                    _ActiveCrafts[characterId] = new Dictionary<uint, MandragoraCraft>();
                }
                _ActiveCrafts[characterId][mandragoraId] = craft;

                Logger.Info($"Character {characterId}'s mandragora {mandragoraId} started crafting recipe {recipeId}, ends at {craft.EndTime} (speedFactor: {speedFactor})");
                return craft;
            }
        }

        /// <summary>
        /// Gets active craft for a mandragora.
        /// </summary>
        public MandragoraCraft GetActiveCraft(uint characterId, uint mandragoraId)
        {
            lock (_ActiveCrafts)
            {
                if (_ActiveCrafts.TryGetValue(characterId, out var charCrafts) &&
                    charCrafts.TryGetValue(mandragoraId, out var craft))
                {
                    return craft;
                }
                return null;
            }
        }

        /// <summary>
        /// Checks if a craft is complete.
        /// </summary>
        public bool IsCraftComplete(uint characterId, uint mandragoraId)
        {
            var craft = GetActiveCraft(characterId, mandragoraId);
            return craft != null && DateTime.UtcNow >= craft.EndTime;
        }

        /// <summary>
        /// Completes a craft and returns the result item ID.
        /// </summary>
        public uint? CompleteCraft(uint characterId, uint mandragoraId)
        {
            lock (_ActiveCrafts)
            {
                if (!_ActiveCrafts.TryGetValue(characterId, out var charCrafts) ||
                    !charCrafts.TryGetValue(mandragoraId, out var craft))
                {
                    return null;
                }

                if (DateTime.UtcNow < craft.EndTime)
                {
                    Logger.Error($"Craft not yet complete for mandragora {mandragoraId}");
                    return null;
                }

                uint resultItemId = craft.ResultItemId;
                charCrafts.Remove(mandragoraId);
                Logger.Info($"Character {characterId}'s mandragora {mandragoraId} completed craft, result: {resultItemId}");
                return resultItemId;
            }
        }

        /// <summary>
        /// Completes a craft and handles species evolution.
        /// Returns evolution result including new species info.
        /// </summary>
        public SpeciesEvolutionResult CompleteCraftWithEvolution(uint characterId, uint mandragoraId)
        {
            lock (_ActiveCrafts)
            {
                if (!_ActiveCrafts.TryGetValue(characterId, out var charCrafts) ||
                    !charCrafts.TryGetValue(mandragoraId, out var craft))
                {
                    return null;
                }

                if (DateTime.UtcNow < craft.EndTime)
                {
                    Logger.Error($"Craft not yet complete for mandragora {mandragoraId}");
                    return null;
                }

                var mandragora = _Server.Database.SelectMandragora(characterId, mandragoraId);
                if (mandragora == null)
                {
                    return null;
                }

                var result = new SpeciesEvolutionResult
                {
                    ResultItemId = craft.ResultItemId,
                    OldSpeciesIndex = mandragora.SpeciesIndex,
                    NewSpeciesIndex = mandragora.SpeciesIndex,
                    IsNewSpeciesDiscovery = false,
                    IsFirstDiscovery = false
                };

                // Check if species evolution should occur (requires fertilizer items)
                if (craft.FertilizerItemIds.Count > 0)
                {
                    var newSpecies = DetermineSpeciesEvolution(mandragora.SpeciesCategory, craft.FertilizerItemIds);
                    if (newSpecies.HasValue && newSpecies.Value != mandragora.SpeciesIndex)
                    {
                        result.NewSpeciesIndex = newSpecies.Value;

                        // Check if this is a new discovery for the character
                        var existingDiscoveries = _Server.Database.SelectMandragoraSpeciesDiscoveriesByCategory(characterId, mandragora.SpeciesCategory);
                        if (!existingDiscoveries.Any(d => d.SpeciesIndex == newSpecies.Value))
                        {
                            result.IsNewSpeciesDiscovery = true;

                            // Check if this is the first discovery server-wide
                            string firstDiscoverer = _Server.Database.SelectMandragoraFirstDiscoverer(newSpecies.Value);
                            if (string.IsNullOrEmpty(firstDiscoverer))
                            {
                                result.IsFirstDiscovery = true;
                            }
                        }

                        // Update mandragora with new species
                        UpdateMandragora(characterId, mandragoraId, speciesIndex: newSpecies.Value, category: mandragora.SpeciesCategory);
                        Logger.Info($"Mandragora {mandragoraId} evolved from species {mandragora.SpeciesIndex} to {newSpecies.Value}");
                    }
                }

                charCrafts.Remove(mandragoraId);
                Logger.Info($"Character {characterId}'s mandragora {mandragoraId} completed craft with evolution, result item: {result.ResultItemId}");
                return result;
            }
        }

        public class SpeciesEvolutionResult
        {
            public uint ResultItemId { get; set; }
            public uint OldSpeciesIndex { get; set; }
            public uint NewSpeciesIndex { get; set; }
            public bool IsNewSpeciesDiscovery { get; set; }
            public bool IsFirstDiscovery { get; set; }
        }

        /// <summary>
        /// Determines the new species based on fertilizer items used.
        /// </summary>
        private uint? DetermineSpeciesEvolution(MandragoraSpeciesCategory category, List<uint> fertilizerItemIds)
        {
            if (fertilizerItemIds.Count == 0)
            {
                return null;
            }

            var asset = _Server.AssetRepository.MandragoraAsset;
            var categorySpecies = asset.Species.Where(s => s.Category == category).ToList();

            if (categorySpecies.Count == 0)
            {
                return null;
            }

            // Simple evolution logic based on fertilizer count:
            // More fertilizers = higher chance of rarer species
            var random = new Random();
            int fertilizerBonus = Math.Min(fertilizerItemIds.Count, 20); // Max bonus at 20 items

            // Weight species by rarity, with fertilizer bonus increasing rare chances
            var weightedSpecies = categorySpecies.Select(s =>
            {
                int baseWeight = s.Rarity switch
                {
                    MandragoraRarity.Common => 100,
                    MandragoraRarity.Uncommon => 50 + fertilizerBonus * 2,
                    MandragoraRarity.Rare => 20 + fertilizerBonus * 3,
                    MandragoraRarity.MysticRare => 5 + fertilizerBonus * 2,
                    MandragoraRarity.LimitedRare => 1 + fertilizerBonus,
                    _ => 50
                };
                return (Species: s, Weight: baseWeight);
            }).ToList();

            int totalWeight = weightedSpecies.Sum(w => w.Weight);
            int roll = random.Next(totalWeight);

            int cumulative = 0;
            foreach (var (species, weight) in weightedSpecies)
            {
                cumulative += weight;
                if (roll < cumulative)
                {
                    return species.Index;
                }
            }

            // Fallback to first species
            return categorySpecies.First().Index;
        }

        /// <summary>
        /// Cancels an active craft.
        /// </summary>
        public bool CancelCraft(uint characterId, uint mandragoraId)
        {
            lock (_ActiveCrafts)
            {
                if (!_ActiveCrafts.TryGetValue(characterId, out var charCrafts) ||
                    !charCrafts.ContainsKey(mandragoraId))
                {
                    return false;
                }

                charCrafts.Remove(mandragoraId);
                Logger.Info($"Character {characterId}'s mandragora {mandragoraId} cancelled craft");
                return true;
            }
        }

        /// <summary>
        /// Gets craft categories.
        /// </summary>
        public List<CDataMyMandragoraCraftCategory> GetCraftCategories()
        {
            var asset = _Server.AssetRepository.MandragoraAsset;
            return asset.CraftCategories.Select(c => new CDataMyMandragoraCraftCategory
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
        }

        /// <summary>
        /// Gets craft recipes for a specific category.
        /// </summary>
        public List<CDataMyMandragoraCraftRecipe> GetCraftRecipes(uint categoryId)
        {
            var asset = _Server.AssetRepository.MandragoraAsset;
            var recipes = asset.CraftRecipes.Where(r => r.CategoryId == categoryId || categoryId == 0);

            return recipes.Select(r => new CDataMyMandragoraCraftRecipe
            {
                RecipeId = r.RecipeId,
                ItemId = (ItemId)r.ResultItemId,
                Time = r.CraftTime,
                Unk3 = r.Cost,
                Unk4 = new List<CDataMyMandragoraCraftRecipeUnk4>
                {
                    new CDataMyMandragoraCraftRecipeUnk4 { Unk0 = 1, Unk1 = 1 }
                },
                Unk5 = r.IsLocked,
                CraftMaterialList = r.Materials.Select(m => new CDataMDataCraftMaterial
                {
                    ItemId = (ItemId)m.ItemId,
                    Num = (ushort)m.Quantity,
                    SortNo = (byte)m.SortNo,
                    IsSp = m.IsSpecial
                }).ToList()
            }).ToList();
        }

        #endregion

        #region Static Data

        /// <summary>
        /// Gets rarity level definitions.
        /// </summary>
        public List<CDataMyMandragoraRarityLevel> GetRarityLevels()
        {
            return _RarityLevels;
        }

        /// <summary>
        /// Gets fertilizer items.
        /// </summary>
        public List<CDataMyMandragoraFertilizerItem> GetFertilizerItems()
        {
            var asset = _Server.AssetRepository.MandragoraAsset;
            return asset.FertilizerItems.Select(f => new CDataMyMandragoraFertilizerItem
            {
                ItemId = f.ItemId,
                ItemNum = 0 // TODO: Get actual count from character inventory
            }).ToList();
        }

        /// <summary>
        /// Gets cultivation material max.
        /// </summary>
        public uint GetCultivationMaterialMax()
        {
            return _Server.AssetRepository.MandragoraAsset.CultivationMaterialMax;
        }

        #endregion

        #region Default Data

        /// <summary>
        /// Creates default mandragoras for a new character.
        /// </summary>
        public void InitializeNewCharacter(uint characterId)
        {
            // Check if character already has mandragoras
            var existing = _Server.Database.SelectMandragoras(characterId);
            if (existing.Count > 0)
            {
                return;
            }

            var asset = _Server.AssetRepository.MandragoraAsset;

            // Create default mandragoras based on furniture items in asset
            // By default, give them species index 1 (first normal species)
            foreach (var furniture in asset.FurnitureItems.Take(1)) // Start with just 1 mandragora
            {
                CreateMandragora(
                    characterId,
                    furniture.MandragoraId,
                    "Mandragora", // Default name
                    1, // First species
                    MandragoraSpeciesCategory.Normal,
                    furniture.ItemId
                );
            }
        }

        #endregion
    }
}
