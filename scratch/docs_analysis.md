# Documentation Analysis - Arrowgene.DragonsDogmaOnline

## Overview

This document provides a comprehensive analysis of all documentation files found in the `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/docs/` directory. The documentation covers the Dragon's Dogma Online private server emulator implementation.

---

## Table of Contents

1. [File Inventory](#file-inventory)
2. [FAQ and Setup Information](#faq-and-setup-information)
3. [Quest System Documentation](#quest-system-documentation)
4. [Epitaph Road Content](#epitaph-road-content)
5. [Scripting System](#scripting-system)
6. [Implementation Status](#implementation-status)
7. [Feature Information Summary](#feature-information-summary)

---

## File Inventory

### Documentation Files (34 total)

| Path | Description |
|------|-------------|
| `/docs/faq.md` | Frequently asked questions and setup guide |
| `/docs/gathering_nodes.md` | Gathering node OM IDs and types |
| `/docs/scripts/scripting_guide.md` | C# scripting system documentation |
| `/docs/quests/quests.md` | Main quest support documentation |
| `/docs/quests/generic_quest_state_machine.md` | Quest implementation guide |
| `/docs/quests/quest_command_reference.md` | Complete quest command API reference |
| `/docs/quests/endgame_content.md` | Endgame mission (EXM) content list |
| `/docs/quests/WildHunt.md` | Quest background image references |
| `/docs/quests/holiday_personal_quests.md` | Seasonal event documentation |
| `/docs/quests/world_manage_state_quests.md` | World state management quests |
| `/docs/quests/known_world_manage_layout_flags.txt` | Layout flag reference data |
| `/docs/quests/known_world_manage_quest_flags.txt` | Quest flag reference data |
| `/docs/quests/npc_ledger_flags.json` | NPC ledger flag data |
| `/docs/quests/events/st0100.md` | Lestania event IDs |
| `/docs/quests/events/st0120.md` | Season 2 event IDs |
| `/docs/quests/events/st0200.md` | White Dragon Temple events |
| `/docs/quests/events/st0201.md` | Audience Chamber events |
| `/docs/epitaph_road/guide.md` | Epitaph Road dungeon guide |
| `/docs/epitaph_road/objectives.md` | Trial objective definitions |
| `/docs/epitaph_road/rathnite_foothills.md` | Rathnite Foothills loot tables |
| `/docs/epitaph_road/memory_of_megadosys.md` | Memory of Megadosys loot tables |
| `/docs/epitaph_road/reward_buffs.md` | Epitaph buff definitions |

### Image Files (11 total)

Located in `/docs/quests/images/` and `/docs/epitaph_road/images/` directories for visual documentation.

---

## FAQ and Setup Information

### Key Topics Covered

1. **Database Migration**: Instructions for updating the database when features change
   - Use `MigrateDatabase.cmd` or `dotnet run dbmigration`

2. **Translation Updates**: Two methods available
   - DDON Launcher with automatic download
   - Legacy local patcher with `gmd.csv` file

3. **Admin Commands**: Account state must be set to `100` in database
   - Uses SQLite DB Browser recommended for local setups

4. **Monster/Loot Placement**: DDONTools recommended for configuration

5. **Settings Configuration**: EXP multipliers and other settings configurable via scripts

6. **Multiplayer Setup**:
   - Configure IP addresses/ports in `Arrowgene.Ddon.config.json`
   - Update `GameServerList.csv` with public IP
   - Players add server via launcher gear icon

7. **Multiple Channels**: Each channel runs as separate server process
   - Unique Port and Id values required
   - Configure `GameServerList.csv` for channel management

---

## Quest System Documentation

### Generic Quest State Machine

The quest system uses a state machine approach with the following key concepts:

#### Quest Structure
- **Processes**: Akin to threads in the quest state machine
- **Blocks**: Each process has sequence and block numbers
- **Commands**: Two types - Check commands (gating) and Result commands (actions)

#### Block Types Available

| Type | Description |
|------|-------------|
| `NpcTalkAndOrder` | Start quest by talking to NPC |
| `DiscoverEnemy` | Quest starts when enemy encountered |
| `SeekOutEnemiesAtMarkedLocation` | Find enemy group at location |
| `KillGroup` | Kill specified enemy group |
| `CollectItem` | Collect items from shiny points |
| `DeliverItems` | Deliver items to NPC |
| `TalkToNpc` | Converse with NPC |
| `IsStageNo` | Check if player in specific stage |
| `MyQstFlags` | Set/check quest flag values |
| `Raw` | Direct command injection |

#### Quest JSON Format

```json
{
    "state_machine": "GenericStateMachine",
    "type": "World|Main|Tutorial",
    "comment": "string",
    "quest_id": "int",
    "base_level": "int",
    "minimum_item_rank": "int",
    "discoverable": "bool",
    "area_id": "string",
    "news_image": "int",
    "rewards": [],
    "enemy_groups": [],
    "blocks": []
}
```

### Quest Commands Reference

The documentation includes comprehensive command references:

#### Check Commands (166+ commands)
Controls quest progression gates:
- `TalkNpc`, `DieEnemy`, `SceHitIn`, `HaveItem`, `DeliverItem`
- `QstFlagOn/Off`, `MyQstFlagOn/Off`, `EventEnd`
- `EmHpNotLess/Less`, `WeatherEq/NotEq`, `PlJobEq/NotEq`
- And many more...

#### Result Commands (100+ commands)
Executes actions during quest:
- `LotOn/Off`, `HandItem`, `SetAnnounce`, `StageJump`
- `EventExec`, `QstLayoutFlagOn/Off`, `BgmRequest`
- `WorldManageLayoutFlagOn/Off`, `PlayCameraEvent`
- And many more...

#### Notify Commands
- `KilledTargetEnemySetGroup`
- `KilledTargetEmSetGrpNoMarker`
- `KilledTargetEnemySetGroup1`

### World Manage State Quests

Controls world state elements like doors, gates, NPCs, and invisible walls.

#### Season-Based Quest IDs

| Season | Quest ID | Description |
|--------|----------|-------------|
| 1.0 | q70000001 | Initial world state |
| 1.1 | q70001001 | Season 1.1 additions |
| 1.2 | q70002001 | Season 1.2 additions |
| 1.3 | q70003001 | Season 1.3 additions |
| 1.4 | q70004001 | Season 1.4 additions |
| 2.0 | q70020001 | Season 2.0 world state |
| 2.1-2.3 | q7002x001 | Season 2 updates |
| 3.0-3.4 | q7003x001 | Season 3 updates |

---

## Epitaph Road Content

### Dungeon Types

| Dungeon | Type | Season |
|---------|------|--------|
| Rathnite Foothills | Legacy | 3.0, 3.1 |
| Memory of Megadosys | New | 3.2 |
| Memory of Urteca | New | 3.3 |

### Legacy vs New Dungeon Differences

**Legacy Dungeons:**
- Multiple "spaces" with soul-cost walls
- Main road with 3-4 free trials
- Gold/bronze chests for weekly rewards
- Stone statues that unlock BO/HO tree progress

**New Dungeons:**
- No main road/spaces distinction
- Trials have unlock costs
- Buff reward system after trials (up to 4 levels)
- Big trials replace golden chests

### Important Object Manager (OM) IDs

| Type | OM ID | Description |
|------|-------|-------------|
| Bare Walls | om522922 | Walls removed by spending souls |
| NPC Wall | om523102 | Cleared via NPC interaction |
| Stone Statue Space Door | om511320 | Red door to statue area |
| Stone Statue | om511321 | Completes dungeon sections |
| Mysterious Doors | om522554 | Unlockable green light doors |
| Mysterious Powers | om522552 | Green light pillars (gathering) |
| Iron Chest | om513050 | Standard chest |
| Gold Chest | om513055 | Weekly reward chest |
| Bronze Chest | om513053 | Weekly reward chest |

### Trial Objectives

| Objective | Japanese | Parameters |
|-----------|----------|------------|
| Eliminate the enemy | 敵を全滅せよ | Unk0=8 |
| Cannot die more than once | 1回以上死亡してはならない | Unk0=4 |
| Item not used more than once | アイテムを1回以上使用してはならない | Unk0=7 |
| No abnormal status 3+ times | 3回以上状態異常にかかってはいけない | Unk0=5 |
| Complete within time limit | 制限時間以内に条件を達成せよ | Unk0=10 |

### Reward Buffs

Categories:
- **Stats Increase**: HP, Stamina, Attack Power, Defenses
- **Status Resist**: Poison, Slow, Sleep, Stun, etc. (Max +100)
- **Status Attack**: Various debuffs and status effects
- **Reward Increase**: Soul drops, Gold drops, Rift drops

---

## Scripting System

### Overview

Uses Roslyn .NET compiler for C# scripts with `.csx` extension.

### Script Locations

All scripts located in `Arrowgene.Ddon.Shared/Files/Assets/scripts/`

### Module System

Modules handle related script collections:
- Each module has dedicated file system watcher
- Supports hot-loading via `EnableHotLoad` property
- Special `libs` module for shared script libraries

### Creating New Modules

1. Create file in `Scripting/Modules` directory
2. Extend `ScriptModule` class
3. Configure `ModuleRoot`, `Filter`, `ScanSubdirectories`, `EnableHotLoad`
4. Override `Options()` and `EvaluateResult()`

### Script Libraries

| Library | Project | Description |
|---------|---------|-------------|
| LibUtils | Server/GameServer | Utility functions |
| LibDdon | GameServer | DDON-specific functions |
| DropRate.csx | Scripts | Drop rate constants |
| SeasonalEvents.csx | Scripts | Seasonal quest utilities |

---

## Implementation Status

### Main Story Quests (MSQ)

#### Season 1.0 - IMPLEMENTED
| Quest | Status | Notes |
|-------|--------|-------|
| Resolutions and Omens | Working | Minor NPC FSM issues |
| The Slumbering God | Working | Well |
| Envoy of Reconciliation | Working | Well |
| Soldiers of the Rift | Working | Well |
| A Servants Pledge | Working | Pawn Dungeon needs more mobs |
| The Crimson Crystal | Working | Well |
| The Dull Grey Ark | Working | Well |
| The Girl in the Forest | Working | Boss fight requires all enemies dead |
| The Goblin King | Working | Well |
| The House of Steam | Working | Well |
| The Assailed Fort | Working | Some orcs missing, needs NPC FSM |
| The Castle of Dusk | Working | Cutscene trigger issue |
| The Gods Awakening | Working | Well |

#### Season 1.1 - IMPLEMENTED
| Quest | Status | Notes |
|-------|--------|-------|
| The Girl Clad in Darkness | Working | NPC location issues |
| The Stolen Heart | Working | Key gimmick issues |
| The Roars of A Thousand | Working | Monster HP issues |
| Return to Yore | Working | Mob placement needs work |
| A Friendly Visit | Working | OM marking issue |
| The Course of Life | Working | Well |

#### Season 1.2 - IMPLEMENTED
Multiple quests implemented with various minor issues noted.

#### Season 2.0 - IMPLEMENTED
- The Storm That Brought A Tragedy
- The Girl Who Lost Her Memories
- The Corruption and the Knights
- Exploring the Den of Monsters
- Eliminate the Corrosion Infestation
- The Man From Another Land
- The Fate of Lestania

#### Season 3.3
- Hopes' Bitter End - **Disabled**

### World Quests - IMPLEMENTED

#### Hidell Plains
- Request For Medicine
- The Woes of A Merchant
- An Assistant's Assistant
- Crackdown on Store Vandals
- A Heart Throbs Once More
- Fabio's Collectibles
- Confrontation With Scouts
- And more...

#### Breya Coast
- Dispatch A Clamor of Harpies
- Boats Buddy (spawn issues)
- Beach Bandits (spawn issues)

### Endgame Content (EXM)

#### 4-Player Content - Various Implementation Status
- The Call of the Catacombs - Implemented
- Drawn to Ancient Power - Implemented
- The Ancient City's Legacy - Implemented
- The Shining Gate - Implemented
- Agent of Corruption - Implemented
- Phantasmic Great Dragon - Implemented
- Earth's Fury - Implemented
- Onset of Darkness - Implemented
- Arisen of the Black Darkness - Implemented

#### 8-Player Content
- Battle for Gritten Fort - Various modes
- Ancient Warrior - Implemented
- The Dazzling Gold - Implemented
- The Dragon Awakened - Implemented

### Seasonal Events

#### Halloween
- 2016: Not Implemented
- 2017: Implemented (q60301000, q60301001)
- 2018: Implemented (q60301052, q60301053, q60301054)

#### Christmas
- 2016: Not Implemented
- 2017: Not Implemented
- 2018: Implemented (q60301055, q60301056, q60301057)

---

## Feature Information Summary

### Known Limitations

1. **Party Play Issues**: Quest system doesn't work well with multiple players
   - Only party leader gets quest banners

2. **Progress Saving**: Only saved on quest completion
   - Intermediate steps lost on disconnect

3. **Homepoint Issues**: Adjusted homepoint during battles not implemented

4. **NPC Dialogue**: May be incorrect (translation issues)

5. **First-Time Completion**: Server treats every completion as first time

### Gathering Node Types

| Type | OM ID |
|------|-------|
| Grass | om520000 |
| Flower | om520012 |
| Mushroom | om520024 |
| Wood | om520030 |
| Mining (Gemstone) | om520050 |
| Mining (Ore) | om520162 |
| Sand | om520060 |
| Box | om520070 |
| Water | om520090 |
| Shells | om520100 |
| Antique | om520111 |
| Alchemy | om520080/81 |
| Corpse | om520170 |
| One off | om520171 |
| Twinkle | om522552 |

### Treasure Chest Types

| Type | OM ID |
|------|-------|
| Iron Chest | om513050 |
| Brown Chest | om513051 |
| Treasure Chest | om513052 |
| Bronze Chest | om513053 |
| Silver Chest | om513054 |
| Gold Chest | om513055 |
| Purple Chest | om513056 |
| Orange Sealed (BBM) | om513130 |
| Blue Sealed (BBM) | om513133 |

### Required Tools for Quest Development

- [ripgrep](https://github.com/BurntSushi/ripgrep) - Text search
- [git](https://git-scm.com/download/win) - Version control
- [arctool](http://www.fluffyquack.com/tools/ARCtool.rar) - Archive extraction
- [DDON-translation](https://github.com/Sapphiratelaemara/DDON-translation) - Translation files
- [DDOn-Tools](https://github.com/alborrajo/DDOn-Tools) - Game data tools
- [ddon-data](https://github.com/ddon-research/ddon-data) - Extracted game data

---

## Document Generated

Date: 2026-01-25

Source Directory: `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/docs/`

Total Files Analyzed: 34 (23 markdown/text files, 11 images)
