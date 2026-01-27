# DDON Server Implementation Roadmap

**Created:** 2025-01-25
**Purpose:** Persistent implementation plan (survives context compaction)

---

## ACTIVE WORK: Phase 1 - Complete Partial Features

### 1. Pawn Expedition System (75% → 95%) [MOSTLY COMPLETE]

**Location:** `GameServer/Characters/PawnExpeditionManager.cs`

**Current State:**
- Manager exists with full structure
- 9 handlers implemented
- Database migration exists
- Sally (dispatch) basic flow works
- **NEW:** Area-based reward tables via `PawnExpeditionRewards.json`
- **NEW:** Asset system: `PawnExpeditionRewardsAsset.cs` + `PawnExpeditionRewardsAssetDeserializer.cs`
- **NEW:** Hot spot discovery system with weekly rotation
- **NEW:** Character-specific hot spot discovery tracking

**COMPLETED:**
- [x] Complete hot spot discovery system with area-based randomization
- [x] Implement area-based reward tables (materials via JSON asset)
- [x] Add golden expedition support (2-hour duration, 5 gems cost)
- [x] Implement sally count recharge (10 gems per charge, max 5)

**REMAINING TODO:**
- [ ] Add pawn restrictions while on expedition (can't use in party/crafting)
- [ ] Complete GetRewardDropItem handlers for loot collection (add items to inventory)
- [ ] Implement daily reset at 5:00 AM JST (currently weekly rotation)
- [ ] Test 6-slot inventory requirement for rewards
- [ ] Persist discovered hot spots to database

**Handlers Status:**
- `PawnExpeditionSallyHandler.cs` - ✅ Working
- `PawnExpeditionGetMySallyInfoHandler.cs` - ✅ Working
- `PawnExpeditionGetSallyRewardHandler.cs` - ✅ Working (updated for hot spot discovery)
- `PawnExpeditionCancelSallyHandler.cs` - ✅ Working
- `PawnExpeditionChangeGoldenSallyHandler.cs` - ✅ Working
- `PawnExpeditionChargeSallyCountHandler.cs` - ✅ Working
- `PawnExpeditionGetRewardDrop*.cs` - ⚠️ Need inventory integration

**Retail Mechanics (from wiki):**
- Requires Clan Level 3 (Clan Hall unlocked)
- Pawn Expedition Facility costs 2,000 CP
- Once per day per pawn (resend costs 1 GG)
- Returns random materials + monster kill from area
- Hot spots: random discovery, persist 1 day, special items
- 3 GG guarantees hot spot success (Rusty Iron Lump)
- Reset: 5:00 AM JST daily

---

### 2. Mandragora Cultivation (30% → 70%) [PARTIALLY COMPLETE]

**Location:** `GameServer/Characters/MandragoraManager.cs`

**Current State Assessment (2025-01-25):**

**COMPLETED (70%):**
- [x] MandragoraManager with full ownership CRUD
- [x] Species discovery tracking per character
- [x] First discoverer registration (server-wide)
- [x] Database tables: `ddon_mandragora`, `ddon_mandragora_species_discovery`, `ddon_mandragora_first_discovery`
- [x] Database SQL methods (Insert/Update/Delete/Select for all tables)
- [x] Asset system: `MandragoraAsset.cs` with species, recipes, categories
- [x] JSON asset: 64 species defined (35 Normal, 20 Chilli, 9 Special)
- [x] Craft recipe definitions (4 example recipes in JSON)
- [x] **In-memory craft tracking system** - MandragoraCraft class with timer support
- [x] **Craft lifecycle methods** - StartCraft, GetActiveCraft, IsCraftComplete, CompleteCraft, CancelCraft
- [x] 5 handlers implemented:
  - `MandragoraGetMyMandragoraHandler` - ✅ Full data retrieval
  - `MandragoraGetSpeciesCategoryListHandler` - ✅ Working
  - `MandragoraGetSpeciesListHandler` - ✅ Working
  - `MandragoraGetCraftRecipeListHandler` - ✅ Working
  - `MandragoraBeginCraftHandler` - ✅ Working (validates mandragora, starts timed craft)
- [x] Rarity system (5 levels: LimitedRare, Common, Uncommon, Rare, MysticRare)
- [x] 12 species categories (Normal, Chilli, Albino, Charcoal, Veggie, Armored, Clothed, Flowering, Barbarian, Scroll, Helmet, Special)
- [x] Furniture items (3 mandragora slots)
- [x] Character initialization with default mandragora

**REMAINING TODO (30%):**
- [ ] **18-hour cultivation timer** - No timer system, mandragoras don't grow
- [ ] **Fertilizer → species calculation** - No algorithm to determine species from input materials
- [ ] **Craft completion handler** - Need MandragoraEndCraftHandler to complete crafts and give items
- [ ] **Material consumption** - BeginCraft doesn't consume materials from inventory yet
- [ ] **Soft soil requirement checks** - No validation for cultivation prerequisites
- [ ] **Expand species asset** - Only 2 of 12 categories have species defined
- [ ] **Achievement integration** - MandragoraSpecies achievement type exists but not wired

**Handlers Needed:**
- `MandragoraEndCraftHandler` - Complete a craft and receive items
- `MandragoraCancelCraftHandler` - Cancel in-progress craft
- `MandragoraStartCultivationHandler` - Begin growing a mandragora
- `MandragoraGetCultivationStatusHandler` - Check growth progress
- `MandragoraHarvestHandler` - Collect grown mandragora

**Retail Mechanics:**
- Unlock: Complete "Extend Garden" + "Strange Creature Investigation" PQs
- 18 real-time hours to grow (1 GG to skip)
- Up to 20 materials as fertilizer
- Adventure Passport = 2 Mandragora instead of 1
- Craftable: Bonus Dungeon Tickets (XP/Rift/BO/Gold), Upgrade Rocks
- Species determined by fertilizer material type bias

---

### 3. Quick Party Matching (40% → 100%) [PENDING]

**Location:** `GameServer/Characters/QuickPartyManager.cs`

**TODO:**
- [ ] Implement matching algorithm (content type, level range)
- [ ] Add quest-specific matching
- [ ] Implement 30-minute registration timeout
- [ ] Add 60-second confirmation system
- [ ] Create party formation from matched players
- [ ] Add registration cancellation handling
- [ ] Implement ready/cancel notifications

---

## Phase 2: Core Content

### 4. Complete Extreme Missions (3/6 → 6/6)

**Current:** Only EXM4 "The Shining Gate" scripted

**TODO:**
- [ ] EXM1: The Call of the Catacombs (Lv58)
- [ ] EXM2: Drawn to Ancient Power (Lv60)
- [ ] EXM3: The Ancient City's Legacy (Lv60)
- [ ] EXM5: Agent of Corruption (Lv65)
- [ ] EXM6: Phantasmic Great Dragon (Lv70)

### 5. Season 3.2-3.4 Main Story Quests

- [ ] Research and script Season 3.2 MSQ
- [ ] Research and script Season 3.3 MSQ
- [ ] Research and script Season 3.4 MSQ

---

## Phase 3: Major Systems

### 6. Grand Mission System (NEW - 8-player raids)
- Requires new 8-player instancing architecture
- Score-based rewards
- Time window scheduling

### 7. War Mission System (NEW - Season 3 endgame)
- Dominion Points currency
- Four Demon Generals bosses
- Ranking system

---

## Key Files Reference

```
GameServer/Characters/
├── PawnExpeditionManager.cs    [95% COMPLETE]
├── MandragoraManager.cs        [PENDING]
├── QuickPartyManager.cs        [PENDING]
└── (future) GrandMissionManager.cs

GameServer/Handler/
├── PawnExpedition*.cs (9 handlers)
├── Mandragora*.cs (5 handlers)
└── QuickParty*.cs (3 handlers)

Database/
├── migration_pawn_expedition.sql (if needed)
└── Model/PawnExpedition models

Shared/Files/Assets/
├── PawnExpeditionRewards.json  [CREATED] - 12 areas with CommonRewards, RareRewards, HotSpotRewards
└── Mandragora.json (exists, expand)

Shared/Asset/
├── PawnExpeditionRewardsAsset.cs [CREATED] - Model classes
└── (other asset models)

Shared/AssetReader/
├── PawnExpeditionRewardsAssetDeserializer.cs [CREATED]
└── (other deserializers)
```

---

## Progress Tracking

| Feature | Start | Current | Target |
|---------|-------|---------|--------|
| Pawn Expedition | 75% | **95%** | 100% |
| Mandragora | 30% | **70%** | 100% |
| Quick Party | 40% | 40% | 100% |
| Extreme Missions | 50% | 50% | 100% |

### Recent Changes (2025-01-25)
- Created `PawnExpeditionRewards.json` with 12 areas and reward tiers
- Created `PawnExpeditionRewardsAsset.cs` model class
- Created `PawnExpeditionRewardsAssetDeserializer.cs`
- Registered asset in `AssetRepository.cs`
- Updated `PawnExpeditionManager.cs` with:
  - Asset-based reward generation
  - Weekly hot spot rotation (seeded random for consistency)
  - Character-specific hot spot discovery tracking
  - Hot spot discovery chance (15%) on expedition completion
- Updated `PawnExpeditionGetSallyRewardHandler.cs` to use new API
- Updated `MandragoraManager.cs` with in-memory craft tracking:
  - `MandragoraCraft` class for tracking active crafts per mandragora
  - `StartCraft()` - validates recipe, calculates craft time from asset
  - `GetActiveCraft()`, `IsCraftComplete()` - craft status queries
  - `CompleteCraft()`, `CancelCraft()` - craft lifecycle methods
- Updated `MandragoraBeginCraftHandler.cs`:
  - Now validates mandragora ownership
  - Calls StartCraft() to initiate timed craft
  - Returns proper response with end time and result item

---

## Notes

- Always check `GameServerSettings.cs` for configurable values
- Use existing asset patterns (JSON deserializers)
- Follow handler naming convention: `{System}{Action}Handler.cs`
- Test with actual game client when possible
