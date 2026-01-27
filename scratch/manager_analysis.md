# Manager Classes Analysis - Arrowgene.DragonsDogmaOnline

## Overview

This document provides a comprehensive analysis of all Manager classes in the GameServer project, documenting their purposes, implementation completeness, TODO items, and potential missing functionality.

---

## Characters Folder Managers

### AchievementManager.cs (~1013 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/AchievementManager.cs`

**Purpose:** Handles achievement tracking, rewards, and completion detection for various in-game activities including enemy kills, substories, quests, and special conditions.

**Implementation Status:** Mostly complete with several TODO items

**TODOs Found:**
- `HandleCollect`: "This handler needs information about the gathering point to check for progression" - gathering point tracking not implemented
- `HandleMandragoraSpecies`: Not implemented (empty method)
- `ClearSubstory`: "TODO: Handle this case" - substory clearing not fully implemented
- Various achievement types have placeholder returns

**Key Features:**
- Extensive enemy kill tracking maps (by enemy UI ID)
- Achievement background tracking for asynchronous notification
- Support for various achievement categories (kills, substories, quests)

---

### AppraisalManager.cs (~77 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/AppraisalManager.cs`

**Purpose:** Handles item appraisal/identification mechanics, particularly for trinket rewards.

**Implementation Status:** Partial implementation

**Issues Found:**
- `RollBitterBlackMazeEarringPercent()` returns 0 (stub implementation)

**Notes:** Small utility class with static helper methods for rolling trinket rewards.

---

### AreaRankManager.cs (~371 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/AreaRankManager.cs`

**Purpose:** Manages area progression and rank-up systems across different game regions.

**Implementation Status:** Mostly complete with some TODOs

**TODOs Found:**
- `CheckMonsterGatheringSpots`: Needs proper implementation for monster gathering spot tracking
- `CheckPeriodicallyReleasedSpots`: Needs implementation for periodically released content spots

**Key Features:**
- Area rank experience calculation
- Rank-up reward distribution
- Supply item management for area masters

---

### BitterblackMazeManager.cs (~650 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/BitterblackMazeManager.cs`

**Purpose:** Manages Bitterblack Maze (BBM) dungeon content, including reward tiers, chest drops, and content rewards.

**Implementation Status:** Mostly complete

**TODOs Found:**
- "TODO: handle BattleContentRewardBonus.Up" - reward bonus handling not fully implemented

**Key Features:**
- BBM stage/tier management
- Chest drop table management
- Loot rolling logic
- Content reward distribution

---

### BoardManager.cs (~582 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/BoardManager.cs`

**Purpose:** Manages entry board/party matching system for group content.

**Implementation Status:** Mostly complete

**TODOs Found:**
- "TODO: Quest Manager look up min/max" - min/max player requirements lookup

**Key Features:**
- Group entry management
- Ready check system
- Timer management for board entries
- Context management for party formation

---

### CharacterManager.cs (~558 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/CharacterManager.cs`

**Purpose:** Manages character selection, loading, stat updates, and character-related calculations.

**Implementation Status:** Mostly complete

**TODOs Found:**
- "TODO: Based on server settings add additional contents unlocked" - server-configurable content unlocks

**Key Features:**
- Character stat calculation and updates
- HP/Stamina revive handling
- Custom skill and ability unlock management
- Edit info updates

---

### ClanManager.cs (~777 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/ClanManager.cs`

**Purpose:** Manages clan/guild system including membership, permissions, points, and levels.

**Implementation Status:** Mostly complete

**TODOs Found:**
- "TODO: Clan History" - clan history tracking not implemented

**Key Features:**
- Clan point and level management
- Permission system
- Membership management (join/leave/kick)
- Cross-channel clan synchronization via RPC

---

### ContactListManager.cs (~74 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/ContactListManager.cs`

**Purpose:** Utility class for friend/contact list operations.

**Implementation Status:** Complete

**Notes:** Small utility class with static helper methods for contact list management.

---

### CraftManager.cs (~516 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/CraftManager.cs`

**Purpose:** Manages crafting calculations, pawn crafting, and craft rank/experience systems.

**Implementation Status:** Partial - multiple formula uncertainties

**TODOs Found:**
- Multiple TODOs about "figuring out actual formulas" for:
  - Craft time calculation
  - Pawn craft bonus calculation
  - Quality calculation
  - Great success chance calculation

**Key Features:**
- Craft rank and experience tables
- Pawn crafting bonus calculations
- Recipe requirement validation

---

### DungeonManager.cs (~208 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/DungeonManager.cs`

**Purpose:** Manages dungeon entry, ready checks, and dungeon start procedures.

**Implementation Status:** Complete

**Key Features:**
- Party ready check system
- Content ID tracking
- Support for both bonus dungeons and Epitaph Road

---

### EpitaphRoadManager.cs (~1369 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/EpitaphRoadManager.cs`

**Purpose:** Manages Epitaph Road dungeon content (Season 2+ content) including trials, objectives, buffs, and rewards.

**Implementation Status:** Comprehensive implementation

**Key Features:**
- Complex trial/objective system
- Buff management
- Stone unlock tracking
- Reward distribution
- Section and area management

---

### EquipManager.cs (~432 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/EquipManager.cs`

**Purpose:** Manages equipment changes, item rank calculation, and equipment validation.

**Implementation Status:** Mostly complete with some unknowns

**TODOs Found:**
- Multiple "Unk0" fields that need investigation
- "TODO: Move to other storage types" - storage overflow handling

**Key Features:**
- Equipment slot management
- Item rank calculation
- Visual equipment handling
- Job point deduction for equipment

---

### ExpManager.cs (~812 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/ExpManager.cs`

**Purpose:** Manages experience points, leveling, and job points for characters and pawns.

**Implementation Status:** Complete

**Key Features:**
- Complete EXP tables up to level 120
- BBM mode leveling (separate level cap)
- Party EXP distribution
- Course bonus integration
- Job point awarding

---

### GpCourseManager.cs (~329 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/GpCourseManager.cs`

**Purpose:** Manages GP Course (premium subscription) effects and bonuses.

**Implementation Status:** Complete

**Key Features:**
- Timer-based course activation/deactivation
- Various bonus types (EXP, PP, BO, bazaar, etc.)
- Course stacking support

---

### HubManager.cs (~191 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/HubManager.cs`

**Purpose:** Manages hub/lobby player contexts and visibility in hub stages.

**Implementation Status:** Complete with hotfix

**Notes:** Contains a HOTFIX comment about reducing channel meltdown issues by being aggressive about lobby context distribution.

**Key Features:**
- Hub member tracking
- Context synchronization between clients
- Stage transition handling

---

### ItemManager.cs (~1192 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/ItemManager.cs`

**Purpose:** Core item management including add, remove, move, and consume operations across various storage types.

**Implementation Status:** Mostly complete

**TODOs Found:**
- "TODO: Find all items that add wallet points" - wallet point item identification incomplete
- "TODO: Rollback transaction" - transaction rollback not implemented

**Key Features:**
- Multi-storage item management
- Special item handling (crafting, consumables)
- Wallet conversion support
- Lantern management

---

### JobEmblemManager.cs (~193 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/JobEmblemManager.cs`

**Purpose:** Manages job emblem stats and inheritance (Season 3+ content).

**Implementation Status:** Complete

**Key Features:**
- Emblem stat calculation
- Stat inheritance system
- Integration with scripting module

---

### JobManager.cs (~939 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/JobManager.cs`

**Purpose:** Manages job changes, skills, and abilities for characters and pawns.

**Implementation Status:** Mostly complete

**TODOs Found:**
- "TODO: Reject job change" - validation for job change rejection
- Multiple "Unk0" fields that need investigation

**Key Features:**
- Job change handling
- Skill/ability unlock and management
- Pawn skill synchronization
- Secret ability support

---

### JobMasterManager.cs (~269 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/JobMasterManager.cs`

**Purpose:** Manages job training/mastery system including kill tracking and reward unlocks.

**Implementation Status:** Complete

**TODOs Found:**
- "TODO: Should we log a message?" - minor logging consideration

**Key Features:**
- Enemy kill tracking for job training
- Training task scheduling
- Skill/ability unlock progression
- Partner pawn bonus integration

---

### JobOrbUnlockManager.cs (~250 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/JobOrbUnlockManager.cs`

**Purpose:** Manages skill augmentation tree unlocks (Season 2/3 orb trees).

**Implementation Status:** Complete

**Key Features:**
- Orb tree element unlocking
- Extended job parameter management
- EX skill propagation to pawns
- Completion percentage tracking

---

### OrbUnlockManager.cs (~700+ lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/OrbUnlockManager.cs`

**Purpose:** Manages Blood Orb skill tree unlocks and progression.

**Implementation Status:** Complete

**Key Features:**
- Orb tree navigation
- Node unlock tracking
- Stat bonus calculation
- Integration with database persistence

---

### PartnerPawnManager.cs (~331 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/PartnerPawnManager.cs`

**Purpose:** Manages partner pawn system including likability, rewards, and adventure timers.

**Implementation Status:** Complete

**Key Features:**
- Likability tracking (gifts, crafts, adventures)
- Reward distribution at likability milestones
- Adventure timer management
- Stage area change handling

---

### PartyQuestContentManager.cs (~199 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/PartyQuestContentManager.cs`

**Purpose:** Manages party quest content including vote-to-abandon and content timers.

**Implementation Status:** Complete

**Key Features:**
- Vote to abandon system
- Content timer management
- Timer extension support

---

### PlayPointManager.cs (~105 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/PlayPointManager.cs`

**Purpose:** Manages play points (PP) for job value shop purchases.

**Implementation Status:** Complete

**Key Features:**
- PP addition with bonus support
- PP removal for purchases
- Max PP capping

---

### QuestManager.cs (Static class, ~700+ lines first section)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/QuestManager.cs`

**Purpose:** Static quest management including quest lookup, registration, and type identification.

**Implementation Status:** Complete (core functionality)

**Key Features:**
- Quest registration and lookup by various IDs
- Quest type categorization
- Quest schedule management
- World quest rolling

---

### RentalPawnManager.cs
**Location:** `/Arrowgene.Ddon.GameServer/Characters/RentalPawnManager.cs`

**Purpose:** Manages rental pawn system for borrowing other players' pawns.

**Implementation Status:** Complete

**Key Features:**
- Pawn rental tracking
- Rental history management
- Rift point transactions

---

### RewardManager.cs
**Location:** `/Arrowgene.Ddon.GameServer/Characters/RewardManager.cs`

**Purpose:** Manages reward distribution for various game activities.

**Implementation Status:** Complete

**Key Features:**
- Item reward distribution
- Wallet reward handling
- Integration with quest completion

---

### StageManager.cs (~700+ lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/StageManager.cs`

**Purpose:** Manages stage/map information, transitions, and area identification.

**Implementation Status:** Complete

**Key Features:**
- Hub stage identification
- Safe area detection
- Stage layout ID management
- Area-to-stage mapping

---

### TimerManager.cs
**Location:** `/Arrowgene.Ddon.GameServer/Characters/TimerManager.cs`

**Purpose:** General-purpose timer management for various game systems.

**Implementation Status:** Complete

**Key Features:**
- Timer creation, start, pause, cancel
- Timer extension support
- Time remaining queries

---

### WalletManager.cs (~131 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Characters/WalletManager.cs`

**Purpose:** Manages wallet/currency operations (Gold, Rift Points, Blood Orbs, etc.).

**Implementation Status:** Complete

**Key Features:**
- Add/remove from wallet with limits
- Scaled wallet amounts (server modifiers)
- Multiple wallet type support

---

## Other Location Managers

### AssetManager.cs (~41 lines)
**Location:** `/Arrowgene.Ddon.GameServer/AssetManager.cs`

**Purpose:** Abstract base class for managing asset loading and hot-reloading.

**Implementation Status:** Complete (base class)

**Key Features:**
- Asset change event handling
- Abstract load method for derived classes

---

### BazaarManager.cs (~231 lines)
**Location:** `/Arrowgene.Ddon.GameServer/BazaarManager.cs`

**Purpose:** Manages player-to-player bazaar/marketplace system.

**Implementation Status:** Mostly complete

**TODOs Found:**
- "TODO: Figure out what _flag is for" - unknown flag parameter
- "TODO: Fetch from DB" - comment in ReExhibit method
- "TODO: Verify if items are supposed to go to the storage box" - item destination uncertainty

**Key Features:**
- Item exhibition with taxes (5%)
- Re-exhibition and cancellation
- Proceeds handling
- GP Course integration for cooldowns

---

### ChatManager.cs (~292 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Chat/ChatManager.cs`

**Purpose:** Manages in-game chat system including various chat types and delivery.

**Implementation Status:** Complete

**Key Features:**
- Multiple chat types (Say, Shout, Party, Clan, Tell)
- Handler-based message processing
- Cross-channel chat via RPC
- Quick-chat support

---

### ContextManager.cs (~261 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Context/ContextManager.cs`

**Purpose:** Manages entity contexts and master assignment for networked entities.

**Implementation Status:** Mostly complete

**TODOs Found:**
- "TODO: Test if returning to host is correct?" - master delegation behavior

**Key Features:**
- UID generation for various entity types
- Master assignment and delegation
- Context storage for party groups

---

### GameTimeManager.cs (~65 lines)
**Location:** `/Arrowgene.Ddon.GameServer/GameTimeManager.cs`

**Purpose:** Manages in-game time calculation and day/night cycles.

**Implementation Status:** Complete

**Key Features:**
- Game time conversion from real time
- Day/night detection
- Time string parsing

---

### GameServerScriptManager.cs (~128 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Scripting/GameServerScriptManager.cs`

**Purpose:** Manages server-side scripting modules for extensibility.

**Implementation Status:** Complete

**Key Features:**
- Multiple scripting modules (Quest, Chat, Items, etc.)
- Hot-reload support
- Addendum reapplication

---

### InstanceAssetManager.cs (~107 lines)
**Location:** `/Arrowgene.Ddon.GameServer/InstanceAssetManager.cs`

**Purpose:** Abstract base class for managing instanced game assets (enemies, items, etc.).

**Implementation Status:** Complete (base class)

**Key Features:**
- Lazy asset instancing
- Stage-based asset storage
- Abstract fetch and instance methods

---

### InstanceDropItemManager.cs (~99 lines)
**Location:** `/Arrowgene.Ddon.GameServer/GatheringItems/InstanceDropItemManager.cs`

**Purpose:** Manages instanced drop items from enemies.

**Implementation Status:** Complete

**Key Features:**
- Multiple drop generators (table, Epitaph, event, default)
- Drop assignment with collision handling
- Report generation for debugging

---

### InstanceEnemyManager.cs (~172 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Enemies/InstanceEnemyManager.cs`

**Purpose:** Manages instanced enemies for game sessions.

**Implementation Status:** Complete

**Key Features:**
- Time-based spawn filtering
- Enemy data storage per stage
- Subgroup tracking

---

### InstanceGatheringItemManager.cs (~107 lines)
**Location:** `/Arrowgene.Ddon.GameServer/GatheringItems/InstanceGatheringItemManager.cs`

**Purpose:** Manages instanced gathering items for resource nodes.

**Implementation Status:** Complete

**Key Features:**
- Multiple gathering generators
- Gathered/empty spot tracking
- Lazy generation on access

---

### InstanceShopManager.cs (~35 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Shop/InstanceShopManager.cs`

**Purpose:** Manages per-instance shop inventory cloning.

**Implementation Status:** Complete

**Key Features:**
- Shop inventory cloning
- Per-instance stock tracking

---

### LightQuestManager.cs (~643 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Quests/LightQuests/LightQuestManager.cs`

**Purpose:** Manages light (board) quests including generation and rotation.

**Implementation Status:** Complete

**TODOs Found:**
- "TODO: This should be configurable" - board quest duration configuration

**Key Features:**
- Dynamic quest generation (hunt/delivery)
- Enemy and item summarization by area
- Quest decay handling
- Reward calculation via mixins

---

### OmManager.cs (~108 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Instance/OmManager.cs`

**Purpose:** Manages OM (Object Marker) data for interactive objects.

**Implementation Status:** Complete

**Key Features:**
- Static utility methods for OM data manipulation
- Thread-safe data access
- Stage-based data organization

---

### PartyManager.cs (~227 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Party/PartyManager.cs`

**Purpose:** Manages party creation, invitations, and lifecycle.

**Implementation Status:** Mostly complete

**TODOs Found:**
- "TODO: Thread safety, logs, error handling" - in RecalculateIdPool method

**Key Features:**
- Party ID pool management
- Invitation system with timeouts
- Party lookup and disbanding
- Cleanup on client exit

---

### QuestStateManager.cs (~500+ lines first section)
**Location:** `/Arrowgene.Ddon.GameServer/Quests/QuestStateManager.cs`

**Purpose:** Abstract base class for managing quest state and progression.

**Implementation Status:** Complete (base class)

**Key Features:**
- Active quest tracking
- Enemy instance management for quests
- Delivery and hunt request tracking
- Quest completion/cancellation

---

### RpcManager.cs (~441 lines)
**Location:** `/Arrowgene.Ddon.GameServer/RpcManager.cs`

**Purpose:** Manages Remote Procedure Calls between game server instances for cross-channel communication.

**Implementation Status:** Complete

**Key Features:**
- Server list management
- Player tracking across channels
- Cross-channel chat (Shout, Clan, Tell)
- Packet announcement to other servers

---

### ScheduleManager.cs (~126 lines)
**Location:** `/Arrowgene.Ddon.GameServer/ScheduleManager.cs`

**Purpose:** Manages scheduled tasks like weekly resets and rotations.

**Implementation Status:** Mostly complete

**TODOs Found:**
- "TODO: Load from server config" - task configuration hardcoded

**Key Features:**
- Multiple scheduled tasks (Epitaph, Area Points, Rankings, etc.)
- Timer-based task execution
- Database persistence of task timestamps

---

### ShopManager.cs (~38 lines)
**Location:** `/Arrowgene.Ddon.GameServer/Shop/ShopManager.cs`

**Purpose:** Manages shop inventory loading and access.

**Implementation Status:** Complete

**Key Features:**
- Asset-based shop loading
- Hot-reload support via base class

---

### StampManager.cs (~159 lines)
**Location:** `/Arrowgene.Ddon.GameServer/StampManager.cs`

**Purpose:** Manages daily/total stamp bonus system.

**Implementation Status:** Complete

**Key Features:**
- Daily and total stamp tracking
- Consecutive stamp reset logic (JST timezone)
- Bonus distribution

---

### WeatherManager.cs (~201 lines)
**Location:** `/Arrowgene.Ddon.GameServer/WeatherManager.cs`

**Purpose:** Manages weather system and forecasting.

**Implementation Status:** Complete

**Key Features:**
- Randomized weather sequence generation
- Weather forecasting
- Moon phase calculation
- Game time conversion

---

## Summary

### Implementation Completeness

| Category | Complete | Mostly Complete | Partial | Stub |
|----------|----------|-----------------|---------|------|
| Characters Managers | 18 | 8 | 2 | 0 |
| Other Managers | 15 | 4 | 0 | 0 |

### Critical TODOs Requiring Attention

1. **CraftManager** - Multiple formula implementations needed
2. **AchievementManager** - Mandragora species handling, gathering point tracking
3. **AreaRankManager** - Monster gathering spots, periodically released spots
4. **ItemManager** - Transaction rollback, wallet point item identification
5. **ScheduleManager** - Task configuration should be externalized
6. **BazaarManager** - Unknown flag parameter, item destination verification

### Missing/Stub Implementations

1. `AppraisalManager.RollBitterBlackMazeEarringPercent()` - Returns 0
2. `AchievementManager.HandleMandragoraSpecies()` - Empty method
3. Various "Unk0" fields throughout the codebase need investigation

### Patterns Observed

1. Extensive use of `DbConnection?` optional parameters for transaction support
2. Consistent `PacketQueue` pattern for batched packet sending
3. Lock-based thread safety throughout managers
4. Asset hot-reload support via event handlers
5. RPC-based cross-channel communication for multiplayer features
