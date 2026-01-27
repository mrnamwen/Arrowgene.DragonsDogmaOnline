# Asset System Analysis - Arrowgene.Ddon.Shared

## Overview

The asset system in `Arrowgene.Ddon.Shared` is responsible for loading, parsing, and managing game data from various file formats (JSON, CSV). The central component is `AssetRepository.cs` which orchestrates the loading of all asset files and provides runtime access to game data.

## Asset Repository Architecture

### Location
`/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/AssetRepository.cs`

### Key Features
1. **Hot Reloading**: Uses `FileSystemWatcher` to detect changes to asset files and reload them at runtime
2. **Event System**: Fires `AssetChanged` events when assets are reloaded
3. **Unified Loading**: All assets are registered through `RegisterAsset<T>()` which handles both initial load and file watching
4. **Error Recovery**: Implements retry logic (up to 10 attempts with 1-second delays) when files are locked during hot reload

### Asset File Location
All asset data files are stored in:
`/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Files/Assets/`

---

## Game Data Loaded from Assets

### Client Data
| Asset Key | File | Description |
|-----------|------|-------------|
| `ClientErrorCodes.json` | JSON | Error code definitions for client messages |
| `itemlist.csv` | CSV | Complete item database (~1MB, comprehensive item list) |

### Server Data - Core Game Systems

#### Character & Progression
| Asset Key | File | Type | Description |
|-----------|------|------|-------------|
| `Arisen.csv` | 35KB | CSV | Arisen (player character) configuration |
| `MyPawn.csv` | 7KB | CSV | Pawn character templates |
| `PawnStartGear.csv` | 2KB | CSV | Starting equipment for pawns |
| `LearnedNormalSkills.json` | JSON | Normal skill learning data |
| `DefaultSecretAbilities.json` | JSON | Secret ability definitions |
| `SkillData.json` | 530KB | JSON | Complete skill and ability data per job |
| `LimitBreak.json` | 8KB | JSON | Level cap increase requirements |
| `JobMasters.json` | 587KB | JSON | Job master NPC and training data |

#### Storage & Inventory
| Asset Key | File | Type | Description |
|-----------|------|------|-------------|
| `Storage.csv` | 271B | CSV | Storage slot configuration |
| `StorageItem.csv` | 365B | CSV | Default storage items |
| `MyRoom.csv` | 1KB | CSV | Player room configuration |

#### World Data
| Asset Key | File | Type | Description |
|-----------|------|------|-------------|
| `EnemySpawn.json` | 2.2MB | JSON | **Largest asset** - all enemy spawn definitions |
| `GatheringItem.csv` | 108KB | CSV | Gathering node item drops |
| `GatheringSpotInfo.json` | 1.4MB | JSON | Gathering spot locations and types |
| `DefaultGatheringDrops.json` | 1.5MB | JSON | Default drops for gathering nodes |
| `WarpPoints.csv` | 2KB | CSV | Fast travel locations |
| `GameServerList.csv` | 160B | CSV | Server list for login |
| `AreaRankSpotInfo.csv` | 6KB | CSV | Area rank spot information |
| `AreaRankSupply.json` | 101KB | JSON | Area rank supply definitions |
| `AreaRankRequirements.json` | 20KB | JSON | Area rank requirement definitions |
| `named_param.ndp.json` | 1.7MB | JSON | Named parameters for enemies/NPCs |
| `LoadingInfo.json` | 379B | JSON | Loading screen information |

#### Crafting System
| Asset Key | File | Type | Description |
|-----------|------|------|-------------|
| `CraftingRecipes.json` | 1.8MB | JSON | All crafting recipes |
| `CraftingRecipesGradeUp.json` | 5.1MB | JSON | **Second largest** - grade up recipes |
| `CraftingLegendPawns.json` | 2KB | JSON | Legend pawn crafting bonuses |
| `PawnCraftSkillCostRate.csv` | 5KB | CSV | Pawn craft cost modifiers |
| `PawnCraftSkillSpeedRate.csv` | 2KB | CSV | Pawn craft speed modifiers |
| `CraftAddStatus.json` | 4KB | JSON | Crafting additional status effects |
| `CostExpScalingInfo.json` | 10KB | JSON | Experience and cost scaling data |

---

## Shop-Related Asset Files

### Main Shop Assets

#### 1. Shop.json (446KB, 14,942 lines)
**Location**: `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Files/Assets/Shop.json`

**Structure**:
```json
{
  "ShopId": 223,
  "Data": {
    "WalletType": 11,
    "GoodsParamList": [
      {
        "Index": 0,
        "ItemId": 18828,
        "Price": 5,
        "Stock": 255
      }
    ]
  }
}
```

**Features**:
- Standard NPC shops with item listings
- Supports different wallet types (Gold, Rift Points, etc.)
- Stock levels and pricing per item

#### 2. SpecialShops.json (1.6MB, 30,942 lines)
**Location**: `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Files/Assets/SpecialShops.json`

**Asset Definition**: `SpecialShopAsset.cs`
```csharp
public Dictionary<ShopType, List<ShopCategory>> SpecialShops { get; set; }
public Dictionary<uint, ShopCategory> ShopCategories { get; set; }
public Dictionary<uint, AppraisalItem> AppraisalItems { get; set; }
```

**Features**:
- Categorized special shops (Trinkets, White Dragon Festival, etc.)
- Appraisal/exchange system with base items and loot pools
- Support for crest attachments on items
- Multiple shop types defined by `ShopType` enum

**Sample Categories**:
- Pick Up
- White Dragon Festival
- Exchange items with crests (Imbued crests with specific values)

#### 3. ClanShop.csv (3KB, 51 entries)
**Location**: `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Files/Assets/ClanShop.csv`

**Asset Definition**: `ClanShopAsset.cs`
```csharp
public ClanShopLineupType Type { get; set; }
public uint LineupId { get; set; }
public uint RequireClanPoint { get; set; }
public byte RequireLevel { get; set; }
```

**Categories**:
- Base Expansions (Clan Flag, Floors, Hidden Room)
- Pawn Expedition Facilities and Equipment
- Pioneering Expeditions (various regions)
- Expedition Support items
- Seasonal decorations (Christmas, Halloween, Anniversary)

#### 4. JobValueShop.csv (621B)
**Location**: `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Files/Assets/JobValueShop.csv`

**Format**: `#jobid,jobvaluetype,lineupid,itemid,price`

**Purpose**: Job-specific items purchasable with job points

---

## Quest-Related Asset Files

### Quest System Overview

#### 1. Quest Asset Directory
**Location**: `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Files/Assets/quests/`
**Count**: ~486 quest JSON files

**Asset Definition**: `QuestAsset.cs`
```csharp
public class QuestAssetData {
    public List<QuestProcess> Processes { get; set; }
    public QuestType QuestType { get; set; }
    public QuestId QuestId { get; set; }
    public QuestId NextQuestId { get; set; }
    public List<QuestRewardItem> RewardItems;
    public List<QuestWalletReward> RewardCurrency;
    public List<QuestOrderCondition> OrderConditions;
    public Dictionary<uint, QuestEnemyGroup> EnemyGroups { get; set; }
    // ... and more
}
```

**Quest JSON Structure** (example from q00000002.json):
```json
{
  "state_machine": "GenericStateMachine",
  "type": "Main",
  "quest_id": 2,
  "next_quest": 3,
  "base_level": 1,
  "order_conditions": [...],
  "rewards": [...],
  "processes": [
    {
      "blocks": [
        {"type": "PlayEvent", ...},
        {"type": "IsStageNo", ...},
        {"type": "TouchNpc", ...}
      ]
    }
  ]
}
```

#### 2. QuestScheduleId.csv (40KB)
Maps quest IDs to schedule IDs for timing/availability

#### 3. QuestEnemyDrops.json (328KB, 13,953 lines)
**Asset Definition**: `QuestDropItemAsset.cs`
- Maps enemy IDs and levels to drop tables
- Supports level-based drop scaling

#### 4. LightQuests.json (38KB)
**Asset Definition**: `LightQuestAsset.cs`
- Defines procedurally generated light quests
- Enemy nodes and gathering nodes per area
- Quest generation parameters (min/max quests, count, level range)

#### 5. Epitaph Trial Assets
**Location**: `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Files/Assets/epitaph/`

Detailed trial definitions for the Epitaph Road endgame content:
- Feryana Wilderness sections (1-4)
- Main road trials and space trials
- Final trial configurations

### Related Quest Assets

| File | Description |
|------|-------------|
| `EpitaphRoad.json` | Epitaph Road dungeon configuration (151KB) |
| `BitterblackMaze.json` | Bitterblack Maze dungeon data (156KB) |
| `BonusDungeon.json` | Bonus dungeon configuration (1KB) |
| `RecruitmentGroups.json` | Party recruitment board categories |
| `EventDrops.json` | Event-specific item drops (137 lines) |

---

## Asset Reading Infrastructure

### IAssetDeserializer Interface
All asset readers implement `IAssetDeserializer<T>`:
```csharp
public interface IAssetDeserializer<T> {
    T ReadPath(string path);
}
```

### CSV Readers (Csv/ directory)
| Reader | Purpose |
|--------|---------|
| `CsvReaderWriter.cs` | Base CSV parser (RFC 4180 compliant) |
| `ClientItemInfoCsv.cs` | Item database reader |
| `ClanShopCsv.cs` | Clan shop entries |
| `JobValueShopCsv.cs` | Job value shop entries |
| `GatheringItemCsv.cs` | Gathering drops |
| `WarpPointCsv.cs` | Warp point definitions |
| `StampBonusCsv.cs` | Login stamp bonuses |
| `StorageCsv.cs` / `StorageItemCsv.cs` | Storage configuration |
| `PawnCraftSkillCostRateCsv.cs` | Pawn crafting costs |
| `PawnCraftSkillSpeedRateCsv.cs` | Pawn crafting speed |
| `AreaRankSpotInfoCsv.cs` | Area rank spots |
| `QuestScheduleIdCsv.cs` | Quest scheduling |
| `ArisenCsvReader.cs` | Arisen character data (32KB reader) |
| `MyPawnCsvReader.cs` | Pawn template data (26KB reader) |
| `MyRoomCsvReader.cs` | Room configuration |

### JSON Deserializers (AssetReader/ directory)
| Deserializer | Purpose |
|--------------|---------|
| `QuestAssetDeserializer.cs` | **Largest** (57KB) - Quest parsing |
| `EpitaphRoadAssertDeserializer.cs` | Epitaph Road parsing (17KB) |
| `AssetCommonDeserializer.cs` | Common enemy/stage parsing (14KB) |
| `BitterblackMazeAssetDeserializer.cs` | BBM dungeon parsing (11KB) |
| `EnemySpawnAssetDeserializer.cs` | Enemy spawn parsing (10KB) |
| `SpecialShopDeserializer.cs` | Special shop parsing (7KB) |
| `JobMasterAssetDeserializer.cs` | Job master data (8KB) |
| `EpitaphTrialAssetDeserializer.cs` | Epitaph trials (8KB) |
| `EventDropAssetDeserializer.cs` | Event drops (8KB) |
| `AchievementAssetDeserializer.cs` | Achievement parsing |
| `GPCourseInfoDeserializer.cs` | GP course info |
| `LightQuestAssetDeserializer.cs` | Light quest generation |

---

## Missing or Placeholder Assets

### Broken Quest Files
- `!BROKEN!q20990002.json` - Explicitly marked as broken (12KB file)

### TODO Items Found in Code

1. **EpitaphRoadAsset.cs:174**
   ```csharp
   Unk4 = new CDataSeasonDungeonUnk0() // TODO: This type got renamed incorrect/confused
   ```

2. **EpitaphTrialAsset.cs:78**
   ```csharp
   // TODO: Come up with a way to internationalize these strings
   ```

3. **EpitaphTrialAsset.cs:113**
   ```csharp
   label = $"TODO: Implement label for objective: {Type}";
   ```
   This appears when `SoulOrdealObjective` types don't have defined labels.

### Minimal/Stub Data Files
| File | Size | Status |
|------|------|--------|
| `LoadingInfo.json` | 379B | Minimal data (13 lines) |
| `RecruitmentGroups.json` | 449B | Minimal data (30 lines) |
| `TrainingRoom.json` | 1.2KB | Basic configuration (66 lines) |
| `LearnedNormalSkills.json` | 3KB | Limited skill data (56 lines) |
| `DefaultSecretAbilities.json` | 635B | Small definition (13 lines) |
| `BonusDungeon.json` | 1.1KB | Basic dungeon definition |

### Unknown/Unk Fields
Multiple asset structures contain `Unk` prefixed fields indicating undocumented or reverse-engineered data:
- `Shop.json`: `Unk0`, `Unk1`, `Unk4-7` in goods entries
- `CDataSeasonDungeonUnk0`, `CDataSeasonDungeonUnk2` in epitaph code

---

## Asset Size Summary

### Largest Assets (by file size)
1. `CraftingRecipesGradeUp.json` - 5.1MB
2. `EnemySpawn.json` - 2.2MB
3. `CraftingRecipes.json` - 1.8MB
4. `named_param.ndp.json` - 1.7MB
5. `SpecialShops.json` - 1.6MB
6. `DefaultGatheringDrops.json` - 1.5MB
7. `GatheringSpotInfo.json` - 1.4MB
8. `itemlist.csv` - 1.1MB

### Total Lines in JSON Assets
~932,244 lines of JSON data

### Quest Data
- 486 individual quest files in `quests/` directory
- ~80 epitaph trial files in `epitaph/` directory

---

## Architecture Diagram

```
AssetRepository.cs
    |
    +-- RegisterAsset<T>() -----> IAssetDeserializer<T>
    |       |                           |
    |       +-- Load()                  +-- JsonReaderWriter<T>
    |       +-- RegisterFileSystemWatcher()    +-- CsvReaderWriter<T>
    |                                          +-- Custom Deserializers
    |
    +-- Asset Properties
            |
            +-- ClientErrorCodes (Dictionary)
            +-- ClientItemInfos (ClientItemInfoAsset)
            +-- ShopAsset (List<Shop>)
            +-- SpecialShopAsset (SpecialShopAsset)
            +-- ClanShopAsset (Dictionary<uint, ClanShopAsset>)
            +-- QuestAssets (QuestAsset)
            +-- QuestDropItemAsset
            +-- LightQuestAsset
            +-- EpitaphRoadAssets
            +-- EpitaphTrialAssets
            +-- BitterblackMazeAsset
            +-- ... (40+ asset properties)
```

---

## Key Observations

1. **Comprehensive Data Coverage**: The asset system covers nearly all game systems from character creation to endgame dungeons.

2. **Dual Format Support**: Uses both CSV (for simpler tabular data) and JSON (for complex nested structures).

3. **Hot Reload Capability**: Server can reload assets without restart, useful for development and live patches.

4. **Quest System Complexity**: The quest deserializer alone is 57KB, indicating sophisticated quest logic including state machines, process blocks, and reward systems.

5. **Endgame Content**: Significant investment in Epitaph Road and Bitterblack Maze systems with detailed asset definitions.

6. **Event System**: Supports time-limited events through EventDrops and specialized quest handling.

7. **Internationalization Gap**: Some hardcoded English strings in epitaph objectives need i18n work.
