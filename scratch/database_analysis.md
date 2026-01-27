# Database Layer Analysis

## Overview

The database layer for Dragon's Dogma Online server emulator is located in `Arrowgene.Ddon.Database/`. It supports **SQLite**, **PostgreSQL**, and **SQLite In-Memory** databases and provides persistence for the complete game server state.

---

## 1. What Data Is Persisted

### Account & Authentication
| Table | Purpose |
|-------|---------|
| `account` | User accounts with credentials, mail verification, login tokens, state (Banned/User/GameMaster/Admin) |
| `ddon_game_token` | Session tokens linking accounts to characters |
| `ddon_connection` | Active server connections per account |
| `meta` | Database version for migrations |
| `setting` | Key-value configuration storage |

### Character Data
| Table | Purpose |
|-------|---------|
| `ddon_character_common` | Shared base for characters and pawns (job, equipment visibility) |
| `ddon_character` | Player characters (name, account link, game mode, pawn slots, bazaar exhibits) |
| `ddon_edit_info` | Character appearance (75+ fields for customization) |
| `ddon_status_info` | HP, White HP, Revive Points |
| `ddon_character_matching_profile` | Party matching preferences |
| `ddon_character_profile` | Profile backgrounds, titles, comments |
| `ddon_character_job_data` | Per-job stats, level, exp, resistances (35+ stat columns) |
| `ddon_character_playpoint_data` | Play points per job |
| `ddon_binary_data` | Serialized binary character data |

### Pawn System
| Table | Purpose |
|-------|---------|
| `ddon_pawn` | Pawn entities (name, type, state, training, crafting stats) |
| `ddon_pawn_training_status` | Training status blob per job |
| `ddon_pawn_reaction` | Pawn reaction animations |
| `ddon_sp_skill` | Pawn special skills |
| `ddon_pawn_favorites` | Favorited pawns per character |
| `ddon_partner_pawn` | Partner pawn affection tracking (gifts, crafts, adventures) |
| `ddon_partner_pawn_last_affection_increase` | Daily affection action tracking |
| `ddon_partner_pawn_pending_rewards` | Pending partner rewards |
| `ddon_rental_pawn` | Rented pawn snapshots with kill/adventure/craft counts |
| `ddon_rental_pawn_feedback` | Feedback scores for rental pawns |
| `ddon_pawn_craft_progress` | Active crafting operations |

### Skills & Abilities
| Table | Purpose |
|-------|---------|
| `ddon_normal_skill_param` | Learned core skills |
| `ddon_learned_custom_skill` | Learned custom skills per job |
| `ddon_equipped_custom_skill` | Equipped custom skill slots |
| `ddon_learned_ability` | Learned abilities |
| `ddon_equipped_ability` | Equipped ability slots |
| `ddon_unlocked_secret_ability` | Unlocked secret abilities |
| `ddon_preset_ability` | Saved ability presets (10 ability slots per preset) |

### Equipment & Items
| Table | Purpose |
|-------|---------|
| `ddon_storage` | Storage containers per character (slot max, sort order) |
| `ddon_storage_item` | Items in storage (UID, ID, count, safety, color, plus value, equip points) |
| `ddon_equip_item` | Equipped items per job/slot |
| `ddon_equip_job_item` | Job-specific item equipment |
| `ddon_crests` | Item crests/augments |
| `ddon_equipment_limit_break` | Limit break effects on equipment |

### Currency & Wallet
| Table | Purpose |
|-------|---------|
| `ddon_wallet_point` | All currency types (Gold, Rift Points, Blood Orbs, etc.) |

**Supported Wallet Types (from `WalletType.cs`):**
- Gold (G)
- RiftPoints (R)
- BloodOrbs (BO)
- SilverTickets
- GoldenGemstones (GG)
- RentalPoints (RP)
- ResetJobPoints
- ResetCraftSkills
- HighOrbs (HO)
- DominionPoints (DP)
- AdventurePassPoints (BP)
- CustomMadeServiceTickets
- BitterblackMazeResetTicket
- GoldenDragonMark
- SilverDragonMark
- RedDragonMark

### Quests & Progress
| Table | Purpose |
|-------|---------|
| `ddon_quest_progress` | Active quest progress |
| `ddon_completed_quests` | Completed quest history with clear counts |
| `ddon_priority_quests` | Priority/pinned quests |
| `ddon_reward_box` | Quest reward boxes (random reward indices) |
| `ddon_light_quests` | Light quest rotation schedule |

### Dragon Force & Orbs
| Table | Purpose |
|-------|---------|
| `ddon_dragon_force_augmentation` | Dragon Force unlock progression |
| `ddon_orb_gain_extend_param` | Extended stats from orbs (14 stat types) |

### Bitterblack Maze
| Table | Purpose |
|-------|---------|
| `ddon_bbm_character_map` | BBM character ID mapping |
| `ddon_bbm_progress` | Maze progress (tier, mode, death status) |
| `ddon_bbm_rewards` | Mark rewards per stage |
| `ddon_bbm_content_treasure` | Opened treasures |
| `ddon_bbm_reset_ticket` | Reset ticket usage |
| `ddon_bbm_reset_gg` | Golden Gemstone reset tracking |

### Clan System
| Table | Purpose |
|-------|---------|
| `ddon_clan_param` | Clan data (level, points, emblem, settings) |
| `ddon_clan_membership` | Character-clan relationships with rank/permissions |
| `ddon_clan_shop_purchases` | Purchased clan shop items |
| `ddon_clan_base_customization` | Clan base furniture |

### Social Features
| Table | Purpose |
|-------|---------|
| `ddon_contact_list` | Friend/block list with status and favorites |
| `ddon_shortcut` | Action bar shortcuts |
| `ddon_communication_shortcut` | Chat/emote shortcuts |
| `ddon_released_warp_point` | Unlocked teleport locations |

### Mail System
| Table | Purpose |
|-------|---------|
| `ddon_system_mail` | System mail messages |
| `ddon_system_mail_attachment` | Mail attachments (items, currency) |

### Miscellaneous
| Table | Purpose |
|-------|---------|
| `ddon_bazaar_exhibition` | Player market listings |
| `ddon_stamp_bonus` | Login stamp tracking |
| `ddon_epitaph_road_unlocks` | Epitaph Road progression |
| `ddon_epitaph_claimed_weekly_rewards` | Weekly reward claims |
| `ddon_area_rank` | Area rank progression |
| `ddon_area_rank_supply` | Area rank supply rewards |
| `ddon_rank_record` | Leaderboard records |
| `ddon_achievement` | Achievement completion dates |
| `ddon_achievement_progress` | Achievement progress tracking |
| `ddon_achievement_unique_crafts` | Unique crafted items for achievements |
| `ddon_unlocked_items` | Unlocked recipes/furniture/backgrounds |
| `ddon_myroom_customization` | My Room furniture layout |
| `ddon_recycle_equipment` | Equipment recycling attempt tracking |
| `ddon_schedule_next` | Scheduler timestamps for various task types |
| `ddon_job_master_released_elements` | Job Master progression |
| `ddon_job_master_active_orders` | Active Job Master orders |
| `ddon_job_master_active_orders_progress` | Order completion progress |
| `ddon_skill_augmentation_released_elements` | Skill augmentation tree progress |
| `ddon_job_emblem` | Job emblem stat allocations |
| `ddon_dispel_seals` | Dispel seal collection |

---

## 2. Features With Database Support

### Fully Implemented Features
- **Account Management**: Registration, authentication, login tokens, account states
- **Character System**: Full character creation, customization, multiple game modes
- **Pawn System**: Creation, rental, favorites, partner pawns with affection
- **Job System**: All jobs with stats, skills, abilities, presets
- **Equipment**: Full inventory, equipped items, crests, limit breaks
- **Crafting**: Pawn crafting progress, recipes
- **Quests**: Progress tracking, completion history, priority quests, light quests
- **Bazaar/Market**: Player item listings with state tracking
- **Clans**: Full clan management, membership, shop, base customization
- **Social**: Contact lists, mail system with attachments
- **Bitterblack Maze**: Full progress, rewards, treasure tracking
- **Area Rank**: Progression and supply rewards
- **Achievements**: Progress and completion tracking
- **Scheduler**: Automated task scheduling (reset timers, etc.)
- **Job Master**: Order system with progress tracking
- **Skill Augmentation**: Tree unlocks
- **Job Emblems**: Emblem stat customization
- **Epitaph Road**: Unlocks and weekly rewards
- **My Room**: Furniture customization
- **Leaderboards**: Rank records per quest

---

## 3. TODO Comments & Incomplete Methods

### Active TODOs in Database Code

1. **`DdonSqlDbCharacter.cs:534`**
   ```csharp
   /// TODO: Optimize connection handling here and avoid nested loops re-using
   /// a single connection which is not supported with Npgsql.
   ```
   Issue: Nested database reads require opening new connections as a workaround.

2. **`DdonSqlDbCharacterCommon.cs:418`**
   ```csharp
   // TODO: Figure out why this is happening
   ```
   Issue: Characters sometimes have HP value of 0 in database; defensive code sets to 760.

3. **`DdonSqlDbPawn.cs:38`**
   ```csharp
   // TODO: Filter by CraftSkillList, LevelMin/LevelMax, ItemRankMin/ItemRankMax,
   // IsFriend, IsClan and DragonAbilitiesList
   ```
   Issue: Pawn search filtering is incomplete - several filter criteria not implemented.

4. **`DdonSqlDbPawn.cs:288`**
   ```csharp
   Updated = DateTimeOffset.UtcNow, // TODO: Updated
   ```
   Issue: Pawn "Updated" timestamp not properly tracked from database.

5. **`DdonSqlDbPawn.cs:382`**
   ```csharp
   // TODO: Make this less super dangerous.
   ```
   Issue: Some pawn operation needs safety improvements.

### NotImplementedException Locations

- `DdonDatabaseBuilder.cs:61` - Unhandled database type case
- `DdonSqlDb.cs:123` - Abstract method stub
- `SqlDb.cs:31, 134, 140, 639, 644` - Driver-dependent methods requiring override

---

## 4. Shop/GP/Cash Shop Related Tables

### Current Shop-Related Schema

#### Clan Shop
```sql
CREATE TABLE IF NOT EXISTS "ddon_clan_shop_purchases"
(
    "clan_id"   INTEGER NOT NULL,
    "lineup_id" INTEGER NOT NULL,
    CONSTRAINT "pk_ddon_clan_shop_purchases" PRIMARY KEY ("clan_id", "lineup_id"),
    CONSTRAINT "fl_ddon_clan_shop_purchases_clan_id" FOREIGN KEY ("clan_id")
        REFERENCES "ddon_clan_param" ("clan_id") ON DELETE CASCADE
);
```

#### Player Market (Bazaar)
```sql
CREATE TABLE IF NOT EXISTS "ddon_bazaar_exhibition"
(
    "bazaar_id"       INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    "character_id"    INTEGER NOT NULL,
    "sequence"        INTEGER NOT NULL,
    "item_id"         INTEGER NOT NULL,
    "num"             INTEGER NOT NULL,
    "price"           INTEGER NOT NULL,
    "exhibition_time" DATETIME NOT NULL,
    "state"           SMALLINT NOT NULL,
    "proceeds"        INTEGER NOT NULL,
    "expire"          DATETIME NOT NULL,
    CONSTRAINT "fk_ddon_bazaar_exhibition_character_id"
        FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);
```

#### Currency Wallet (Includes Premium Currencies)
```sql
CREATE TABLE IF NOT EXISTS ddon_wallet_point
(
    "character_id" INTEGER NOT NULL,
    "type"         SMALLINT NOT NULL,  -- Maps to WalletType enum
    "value"        INTEGER NOT NULL,
    CONSTRAINT "pk_ddon_wallet_point" PRIMARY KEY ("character_id", "type"),
    CONSTRAINT "fk_ddon_wallet_point_character_id"
        FOREIGN KEY ("character_id") REFERENCES "ddon_character" ("character_id") ON DELETE CASCADE
);
```

### Premium Currency Types (WalletType Enum)
| ID | Type | Description |
|----|------|-------------|
| 5 | GoldenGemstones | Premium currency (GG) |
| 4 | SilverTickets | Premium tickets |
| 12 | CustomMadeServiceTickets | Special customization tickets |
| 11 | AdventurePassPoints | Battle pass points (BP) |
| 13 | BitterblackMazeResetTicket | BBM reset tickets |

### What's NOT in Database (No Cash Shop Tables)

The database currently does **NOT** have dedicated tables for:
- **Item Shop Catalog** - No tables for NPC shop inventories
- **Premium Shop / Gacha** - No dedicated cash shop tables
- **Purchase History** - No transaction logging for premium purchases
- **Limited/Timed Offers** - No special offer tracking
- **GP (Game Points) Transactions** - No GP-specific tables

Shop inventories and pricing appear to be defined in asset files rather than database, with only wallet balances persisted.

---

## 5. Database Architecture Notes

### Supported Databases
- SQLite (primary development)
- PostgreSQL (production recommended)
- SQLite In-Memory (testing)

### Key Design Patterns
- Partial classes for feature organization (`DdonSqlDb` split across 60+ files)
- Cascading deletes for data integrity
- Composite primary keys for many-to-many relationships
- BLOB storage for serialized data (training status, pawn snapshots)
- Comprehensive indexing for common query patterns

### Migration Support
- 50+ migration scripts in `Files/Database/Script/`
- Version tracked in `meta` table
- `DatabaseMigrator` handles schema updates

---

## Summary

The database layer is **comprehensive and well-structured**, supporting nearly all game features. The main gaps are:
1. **Shop system** has minimal database backing (only clan shop purchases tracked)
2. **Pawn search filtering** is incomplete
3. **Connection handling** needs optimization for PostgreSQL
4. No dedicated **transaction logging** or **premium shop** infrastructure
