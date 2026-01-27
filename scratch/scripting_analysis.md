# DDON Scripting System Analysis

## Overview

The DDON scripting system uses C# scripts (`.csx` files) to expose server internal details that administrators can configure. The system supports hot-reloading and caching of pre-compiled scripts for improved startup times.

**Scripts Root:** `scripts/` directory inside the assets directory

**Key Features:**
- Hot-loadable configuration
- Script caching with compiled DLLs
- Custom module support for server-specific modifications
- Addendum system for fine-grained edits without replacing entire files

---

## Module Analysis

### 1. settings/

**Purpose:** Defines configurable server settings that can be overridden by administrators.

**Location:** `scripts/settings/`

**Implementation Status:** Partially implemented

**Current Files:**
- `settings/game_items/RookiesRing.csx` - Rookies Ring configuration (constant/dynamic exp bonus modes)
- `settings/uncategorized/equipment_recycle.csx` - Equipment recycling settings

**Expected Templates (generated on server start):**
- `ChatCommandSettings.csx` - Chat command behavior settings
- `GameServerSettings.csx` - Core game server settings
- `PointModifierSettings.csx` - Point modifier configurations
- `SeasonalEventSettings.csx` - Seasonal event timing and enabling

**Configuration Guidelines:**
- Chance values: Use range [0.0, 1.0] (0% to 100%)
- Drop rates: Use constants from `DropRate.csx` (VERY_RARE, RARE, UNCOMMON, COMMON, VERY_COMMON, ALWAYS)
- Enable features: Prefix with `Enable`
- Disable features: Prefix with `Disable`

**Gaps/Missing Features:**
- No visible template files in the source (generated at runtime)
- Limited categorization of settings (only `game_items` and `uncategorized` subdirectories)
- No documented list of all available settings variables

---

### 2. quests/

**Purpose:** Defines quests using C# scripts, allowing complex quest behavior beyond simple JSON definitions.

**Location:** `scripts/quests/`

**Implementation Status:** Active development (noted as "still in development")

**Current Quest Count:** 156 scripts

**Directory Structure:**
- `msq/` - Main Story Quests
  - `season1.0/` - 3 quests (q00000001, q00000004, q00000025)
  - `season3.0/` - 7 quests (q00030010-q00030070)
  - `season3.1/` - 7 quests (q00030080-q00030130)
- `exm/` - Extreme Missions (3 quests)
- `personal/` - Personal quests (18 quests)
- `seasonal_events/` - Event quests
  - `christmas/2018/` - 3 quests
  - `halloween/2017/` - 2 quests
  - `halloween/2018/` - 3 quests
  - `summer/2018/` - 3 quests
  - `valentines/2017/` - 1 quest
- `vocation/` - Job-specific quests (78 quests across 11 jobs + EmblemTrial)
- `trials/` - Trial quests
- `world_manage/` - World management quests

**Quest Script Structure:**
```csharp
public class ScriptedQuest : IQuest
{
    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.ResolutionsAndOmens;
    public override ushort RecommendedLevel => 1;

    protected override void InitializeState() { }
    protected override void InitializeEnemyGroups() { }
    protected override void InitializeBlocks() { }
}
```

**Key Features:**
- Enemy group definitions with placement
- Quest process/block system
- Tutorial integration
- NPC state machines
- Stage jumping and events
- Quest flags (QstLayout, MyQst)

**Gaps/Missing Features:**
- Missing season 1.1, 1.2, 1.3, 2.0, 2.1, 2.2, 2.3 main story quests
- No world quests (area-based) currently implemented in scripts
- Limited seasonal event coverage
- No visible documentation for quest block types

---

### 3. chat_commands/

**Purpose:** Defines in-game chat commands starting with `/`. Commands are restricted based on AccountType.

**Location:** `scripts/chat_commands/`

**Implementation Status:** Well-implemented (36 commands)

**Current Commands:**

| Command | Purpose |
|---------|---------|
| `help` | Shows available commands |
| `warp` / `warpi` | Teleport to stages |
| `giveitem` | Give items to player |
| `givepawn` | Give pawn to player |
| `givepowerfulitems` | Debug item generation |
| `setlevel` | Set character level |
| `godmode` | Invincibility toggle |
| `quest` / `ql` | Quest management |
| `finishquest` / `finishquesttype` | Quest completion |
| `flag` | Flag manipulation |
| `party` / `group` / `invite` | Party management |
| `areapoint` / `arearank` | Area rank management |
| `info` | Information display |
| `time` | Time management |
| `unlock` | Content unlocking |
| `release` / `releaseepitaph` | Release management |
| `schedule` | Schedule management |
| `repop` | Enemy respawn |
| `sl` / `il` | Stage/Item listing |
| `bbminfo` | Bitterblack Maze info |
| `fashion` | Fashion equipment |
| `animalcrossing` | Special feature |
| `version` | Version info |
| `omdata` / `updateom` | Object Manager data |
| `pawntime` | Pawn time management |
| `spawntest` | Enemy spawn testing |
| `motherlode` | Resource generation |

**Command Structure:**
```csharp
public class ChatCommand : IChatCommand
{
    public override AccountStateType AccountState => AccountStateType.Admin;
    public override string CommandName => "help";
    public override string HelpText => "usage: `/help` - Shows this text";

    public override void Execute(DdonGameServer server, string[] command,
        GameClient client, ChatMessage message, List<ChatResponse> responses)
    {
        // Implementation
    }
}
```

**Account Types:**
- `AccountStateType.User` - Regular players
- `AccountStateType.GameMaster` - Game masters
- `AccountStateType.Admin` - Administrators (most debug commands)

**Gaps/Missing Features:**
- Most commands are debug-only (Admin level)
- Limited user-facing commands documented
- Warning: Non-User/GameMaster commands are unsupported for general use

---

### 4. enemies/

**Purpose:** Handles enemy-related scripting including drop generation and instance properties.

**Location:** `scripts/enemies/`

**Implementation Status:** Basic implementation

**Current Structure:**
- `drop_generators/` - Custom drop generators
  - `bbm/` - Bitterblack Maze (empty)
  - `normal/` - Normal mode (empty)
- `instance_properties/` - Enemy instance modifiers
  - `bloodorbs.csx` - Blood orb generation for enemies
  - `the_rift.csx` - Rift enemy properties

**Drop Generator Interface:**
```csharp
public interface IInstanceEnemyDropGenerator
{
    public GameMode GameMode { get; }
    public List<InstancedGatheringItem> Generate(GameClient client, InstancedEnemy enemyKilled);
}
```

**Instance Property Generator Interface:**
```csharp
public class PropertyGenerator : IInstanceEnemyPropertyGenerator
{
    public override void ApplyChanges(GameClient client, StageLayoutId stageLayoutId,
        byte subGroupId, InstancedEnemy enemy)
    {
        // Modify enemy properties
    }
}
```

**Blood Orb System:**
- Epitaph Road calculation
- Randomized BO enemy spawning (configurable)
- Level and scale adjustments
- Boss gauge multipliers

**Gaps/Missing Features:**
- Empty drop generator directories (no actual drop scripts)
- No dungeon boss drop generators
- Limited instance property scripts
- No documentation for enemy scaling formulas

---

### 5. extended_facilities/

**Purpose:** Inject new NPC menu options using NpcExtendedFacilities.

**Location:** `scripts/extended_facilities/`

**Implementation Status:** Well-implemented (9 NPC scripts)

**Current NPCs:**
- `Anita1.csx` - Epitaph Road barrier NPC
- `Damad1.csx`
- `Holya1.csx`
- `Ira1.csx`
- `Isel1.csx`
- `Kemal1.csx`
- `Pehr1.csx`
- `Seabell0.csx`
- `Yuiato1.csx`

**Interface:**
```csharp
public class NpcExtendedFacility : INpcExtendedFacility
{
    public NpcExtendedFacility()
    {
        NpcId = NpcId.Anita1;
    }

    public override void GetExtendedOptions(DdonGameServer server, GameClient client,
        S2CNpcGetNpcExtendedFacilityRes result)
    {
        // Add custom menu options
        result.ExtendedMenuItemList.Add(new CDataNpcExtendedFacilityMenuItem() {
            FunctionClass = NpcFunction.WarMissions,
            FunctionSelect = NpcFunction.GiveSpirits
        });
    }
}
```

**Available NPC Functions:** Defined in `NpcFunction.cs`

**Gaps/Missing Features:**
- Limited to 9 NPCs
- No comprehensive NPC coverage
- Function types not fully documented in scripts

---

### 6. game_items/

**Purpose:** Handles server-side item implementation requiring special logic.

**Location:** `scripts/game_items/`

**Implementation Status:** Minimal (1 item)

**Current Items:**
- `RookiesRing.csx` - Rookie's Ring of Blessing (EXP bonus item)

**Item Interface:**
```csharp
public class GameItem : IGameItem
{
    public override ItemId ItemId => ItemId.RookiesRingOfBlessing;

    public override void OnUse(GameClient client)
    {
        // Item use behavior
    }

    public override double GetBonusMultiplier(CharacterCommon characterCommon)
    {
        // Calculate bonus effects
    }
}
```

**Rookies Ring Features:**
- Two modes: Constant and Dynamic
- Configurable max level
- Dynamic exp bands based on character level

**Gaps/Missing Features:**
- Only 1 item implemented
- No other special items (potions, consumables, etc.)
- No documentation of items requiring server-side logic

---

### 7. point_modifiers/

**Purpose:** Defines point modifiers for EXP, JP (Job Points), PP (Play Points), and AP (Area Points).

**Location:** `scripts/point_modifiers/`

**Implementation Status:** Well-implemented (14 modifiers)

**Current Modifiers:**

| File | Type | Description |
|------|------|-------------|
| `base_ap_scaledpoints.csx` | Base/AP | Area points scaling |
| `base_exp_enemyscaledpoints.csx` | Base/EXP | Enemy EXP scaling |
| `base_exp_pawncatchup.csx` | Base/EXP | Pawn catchup mechanics |
| `base_exp_penalty_ptlvdiff.csx` | Base/EXP | Party level difference penalty |
| `base_exp_penalty_tgtlvdiff.csx` | Base/EXP | Target level difference penalty |
| `base_exp_questscaledpoints.csx` | Base/EXP | Quest EXP scaling |
| `base_jp_scaledpoints.csx` | Base/JP | Job points scaling |
| `base_pp_scaledpoints.csx` | Base/PP | Play points scaling |
| `bonus_ap_questcourse.csx` | Bonus/AP | Quest course AP bonus |
| `bonus_exp_enemycourse.csx` | Bonus/EXP | Enemy course EXP bonus |
| `bonus_exp_enemyequipment.csx` | Bonus/EXP | Equipment EXP bonus |
| `bonus_exp_questcourse.csx` | Bonus/EXP | Quest course EXP bonus |
| `bonus_exp_questequipment.csx` | Bonus/EXP | Quest equipment EXP bonus |
| `bonus_pp_enemycourse.csx` | Bonus/PP | Enemy course PP bonus |

**Modifier Types:**
- **Base Modifier**: Applied to base point amount
- **Bonus Modifier**: Applied after all base modifiers

**Modifier Actions:**
- **Additive**: Added together first
- **Multiplicative**: Applied after additive

**Interface:**
```csharp
public class PointModifier : IPointModifier
{
    public override PointType PointType => PointType.ExperiencePoints;
    public override PointModifierType ModifierType => PointModifierType.BaseModifier;
    public override PointModifierAction ModifierAction => PointModifierAction.Multiplicative;
    public override RewardSource Source => RewardSource.Enemy;
    public override PlayerType PlayerTypes => (PlayerType.Player | PlayerType.MyPawn);

    public override double GetMultiplier(GameMode gameMode, CharacterCommon characterCommon,
        PartyGroup party, InstancedEnemy enemy, QuestType questType)
    {
        return multiplier;
    }
}
```

**Gaps/Missing Features:**
- No documented GP (Gold Points) modifiers
- Limited bonus modifiers compared to base
- No rental/pawn point modifiers

---

## Additional Modules

### libs/

**Purpose:** Shared script libraries that can be included by other scripts.

**Files:**
- `DropRate.csx` - Drop rate constants
- `ExtremeMissionUtils.csx` - EXM utilities
- `PartyUtils.csx` - Party management utilities
- `ScriptUtils.csx` - General script utilities
- `SeasonalEvents.csx` - Seasonal event helpers

### mixins/

**Purpose:** Configurable functionality shared across server components.

**Files:**
- `bitterblack_earring.csx` - BBM earring mechanics
- `default_gathering.csx` - Default gathering drop generation
- `enemy_exp.csx` - Enemy experience calculations
- `equipment_recycle.csx` - Equipment recycling
- `light_delivery_quest_reward.csx` - Delivery quest rewards
- `light_hunt_quest_reward.csx` - Hunt quest rewards
- `rental_cost.csx` - Rental costs
- `rental_point.csx` - Rental points

### addendums/

**Purpose:** Fine-grained edits to server scripts without replacing entire files.

**Status:** Placeholder only (empty directory with README)

**Use Case:** Override quest rewards, parameters without maintaining full quest files

### custom/

**Purpose:** Complete custom script replacements for server-specific modifications.

**Status:** Placeholder only (empty directory with README)

**Use Case:** Full quest/module replacement when addendums aren't sufficient

### area_rank/

**Purpose:** Area rank configuration

**Subdirectories:**
- `monster_caution_spots/` - Monster spawn configuration

### job_orb_tree/

**Purpose:** Job orb tree configurations

**Subdirectories:**
- `skill_augmentation/` - Skill augmentation data
- `special_conditions/` - Special unlock conditions
- `special_skill_augmentation/` - Special skill augmentations

### emblems/

**Purpose:** Emblem system configuration

**Subdirectories:**
- `stats/` - Emblem stat definitions

---

## Summary of Gaps and Missing Features

### Critical Gaps:
1. **Quest Coverage**: Missing multiple seasons of main story quests (1.1-2.3)
2. **World Quests**: No area-based world quests implemented in scripts
3. **Drop Generators**: Empty implementation directories
4. **Game Items**: Only 1 item implemented (Rookies Ring)

### Documentation Gaps:
1. No comprehensive list of all configurable settings
2. Quest block types not documented
3. NPC function types not catalogued
4. Enemy scaling formulas undocumented

### Feature Gaps:
1. Limited seasonal event coverage
2. No GP (Gold) point modifiers
3. Limited NPC extended facilities coverage
4. No instance enemy drop scripts

### Structural Gaps:
1. Settings templates only generated at runtime (not in source)
2. Custom/Addendum directories are empty placeholders
3. Area rank monster caution spots README is empty

---

## Recommendations

1. **Documentation**: Create comprehensive documentation for all interfaces and expected implementations
2. **Templates**: Include example templates in source for all module types
3. **Quest Expansion**: Prioritize implementing missing main story quest seasons
4. **Drop System**: Implement actual drop generator scripts for different game modes
5. **Item System**: Expand game items module with more special items
6. **Settings Visibility**: Document all available settings variables in README files
