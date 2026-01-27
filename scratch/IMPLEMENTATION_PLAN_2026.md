# DDON Server Implementation Plan - January 2026

**Analysis Date:** 2026-01-25
**Based on:** Handler analysis, Manager analysis, GitHub issues/PRs, retail documentation, community resources

---

## Current Implementation Status

### Summary Statistics
- **Total Handlers:** 492
- **Fully Implemented:** ~410 handlers (83%)
- **Partially Implemented:** ~69 handlers (14%)
- **Stub/Placeholder:** ~13 handlers (3%)
- **Total Managers:** 38+
- **Complete/Mostly Complete:** 35+

### Recent Session Fixes (Already Completed)
1. **GP Course Handler** - Fixed `GpGpCourseGetAvailableListHandler` to return active courses
2. **Party ID Pool Bug (#110)** - Added thread safety to `PartyManager.RecalculateIdPool()`
3. **Quest Bonus System** - Created asset system and implemented all 4 handlers

---

## Priority 1: Merge Open PRs (Low Effort, High Impact)

### 1.1 Equipment Presets PR #590
**Status:** Open since October 2024
**Author:** pacampbell
**Effort:** Review and merge

**Features:**
- Save equipment presets
- Quick equipment swapping
- No blocking issues noted

**Action:** Review, test, and merge

---

### 1.2 Clan Requests PR #856
**Status:** Open since May 2025
**Author:** MrDetonia
**Effort:** Review and merge

**Features:**
- New table `ddon_clan_requests` with migrations
- Join request registration/approval
- Weekly cleanup of stale requests (30+ days)
- Offline notification on login

**Action:** Review, test migration, merge

---

### 1.3 GP Shop PR #724 (Complex)
**Status:** Open since March 2025
**Author:** RyanYappert (original by Sehkah)
**Effort:** Review, fix known issues, merge

**Features Implemented:**
- Golden Gemstone purchase for CAP
- Normal treasure lot gacha
- Box treasure lot gacha
- Event code input

**Known Issues:**
1. CAP balance not working in-game
2. Online Shop closes client (malformed packets?)
3. Some handlers have ResponseErrorException blocking

**Packets Implemented:**
- BOX_GACHA_BOX_GACHA_BUY_REQ/RES
- BOX_GACHA_BOX_GACHA_DRAW_INFO_REQ/RES
- GACHA_GACHA_BUY_REQ/RES
- EVENT_CODE_EVENT_CODE_INPUT_REQ/RES
- GP_CHANGE_CAP_TO_GP_REQ/RES
- GP_GP_SHOP_DISPLAY_GET_TYPE_REQ/RES

**Action:** Pull branch, investigate CAP balance issue, test, fix, merge

---

## Priority 2: Stub Handler Implementation (Medium Effort)

### 2.1 Clan Scout System (6 Stub Handlers)
**Impact:** Enables clan recruitment via in-game UI

**Handlers:**
| Handler | Current Status |
|---------|----------------|
| `ClanClanScoutEntrySearchHandler` | Returns `new()` empty |
| `ClanClanScoutEntryGetInviteListHandler` | Returns empty |
| `ClanClanScoutEntryGetInvitedListHandler` | Returns empty |
| `ClanClanGetHistoryHandler` | Returns empty |
| `ClanClanGetJoinRequestedListHandler` | Returns empty |
| `ClanClanGetMyJoinRequestListHandler` | Returns empty |

**Database Changes Required:**
```sql
CREATE TABLE ddon_clan_scout_entry (
    id INTEGER PRIMARY KEY,
    clan_id INTEGER NOT NULL,
    entry_text TEXT,
    play_style INTEGER,
    activity_level INTEGER,
    created_at DATETIME NOT NULL,
    FOREIGN KEY (clan_id) REFERENCES ddon_clan(id)
);

CREATE TABLE ddon_clan_history (
    id INTEGER PRIMARY KEY,
    clan_id INTEGER NOT NULL,
    event_type INTEGER NOT NULL,
    character_id INTEGER,
    details TEXT,
    created_at DATETIME NOT NULL
);
```

**Approach:**
1. Create migration scripts for new tables
2. Add repository methods in `DatabaseBuilder`
3. Implement `ClanManager` methods for scout operations
4. Implement all 6 handlers

---

### 2.2 Treasure Point System (2 Stub Handlers)
**Impact:** Enables treasure point collection in dungeons

**Handlers:**
| Handler | Current Status |
|---------|----------------|
| `InstanceTreasurePointGetCategoryListHandler` | Stub |
| `InstanceTreasurePointGetListHandler` | Only echoes CategoryId |

**Research Needed:**
- Determine what treasure points are in retail
- Create asset file for treasure point definitions

**Approach:**
1. Research retail treasure point system
2. Create `TreasurePointAsset.cs` and JSON
3. Implement handlers to return treasure point data

---

### 2.3 Clan Shop Buff Item (1 Blocking Handler)
**Impact:** Enables clan buff purchases

**Handler:** `ClanClanShopBuyBuffItemHandler`
**Current Status:** Throws `ResponseErrorException(ERROR_CODE_FAIL)`
**Reason:** Requires pawn expedition currency

**Dependencies:**
- Pawn Expedition system (not implemented)

**Action:** Either implement minimal expedition support OR create alternative currency source

---

## Priority 3: Critical Bug Fixes

### 3.1 Connection Cleanup Bug (#468)
**Impact:** Players get locked out when connections not removed
**Effort:** Low

**Location:** Disconnect handling code
**Action:** Ensure connection entries are always cleaned up

---

### 3.2 Skill Desync in Parties (#639, #715)
**Impact:** Skills show incorrect state for party members
**Effort:** Medium-High

**Issues:**
- Spirit Lancer Cure Glasta desync
- General skill desync

**Location:** Context synchronization in party handlers
**Action:** Investigate context propagation, ensure skill state syncs properly

---

### 3.3 Core Skills Job Switch (#358)
**Impact:** Core skills don't change properly when switching jobs
**Effort:** Medium

**Location:** `JobManager.cs`
**Action:** Fix skill reset on job change

---

### 3.4 Revival Timer Channel Exploit (#529)
**Impact:** Unlimited revivals by channel hopping
**Effort:** Low

**Location:** Revival timer storage (currently in memory)
**Action:** Persist revival timer in database

---

## Priority 4: Feature Enhancements

### 4.1 CraftManager Formula Implementation
**Impact:** Accurate crafting calculations
**Effort:** Medium (research required)

**TODOs in CraftManager:**
- Craft time calculation
- Pawn craft bonus calculation
- Quality calculation
- Great success chance calculation

**Approach:**
1. Research retail formulas from community data
2. Implement proper calculations
3. Make configurable via server settings

---

### 4.2 Mandragora Crafting System
**Impact:** Working Mandragora cultivation
**Effort:** Medium

**Handler:** `MandragoraBeginCraftHandler`
**Current Status:** Hardcoded test values

**Approach:**
1. Research retail Mandragora mechanics
2. Create asset file for growth data
3. Implement proper crafting logic

---

### 4.3 Weather Affecting Gathering (#297)
**Impact:** More realistic gathering experience
**Effort:** Low
**Label:** Good first issue

**Approach:**
1. Use existing `WeatherManager`
2. Modify gathering spot generation based on weather

---

## Priority 5: Content Expansion (High Effort)

### 5.1 Season 3.x Special Skill Augmentation (#685)
**Impact:** Endgame character progression
**Effort:** High

**Features:**
- New HO currency
- Player and pawn upgrades
- EX skill upgrades

---

### 5.2 Pawn Expeditions
**Impact:** New gameplay loop, enables clan buff shop
**Effort:** Very High

**Systems Required:**
- Expedition state management
- Timer-based completion
- Reward calculation
- Expedition currency tracking

---

### 5.3 Main Story Quests (Seasons 1.1-2.3)
**Impact:** Complete story progression
**Effort:** Very High (content creation)

**Current Status:** Only Season 3.0+ has scripted quests

---

### 5.4 Grand Missions (8-player raids)
**Impact:** Major endgame content
**Effort:** Very High

**Systems Required:**
- 8-player matchmaking
- Mission instances
- Scoring system
- Raid-specific rewards

---

## Implementation Roadmap

### Phase 1: Quick Wins (1-2 weeks)
1. ✅ Quest Bonus System (Completed)
2. ✅ GP Course Handler (Completed)
3. ✅ Party ID Bug Fix (Completed)
4. [ ] Merge PR #590 (Equipment Presets)
5. [ ] Merge PR #856 (Clan Requests)
6. [ ] Fix Connection Cleanup (#468)
7. [ ] Fix Revival Timer (#529)

### Phase 2: Medium Effort (2-4 weeks)
1. [ ] Review and merge GP Shop PR #724
2. [ ] Implement Treasure Point System
3. [ ] Fix Skill Desync (#639, #715)
4. [ ] Implement Weather Gathering (#297)

### Phase 3: Clan System (1-2 months)
1. [ ] Database migrations for clan scout
2. [ ] Implement Clan Scout handlers (6)
3. [ ] Implement Clan History
4. [ ] Add RPC support for cross-channel scout

### Phase 4: Formulas and Polish (Ongoing)
1. [ ] CraftManager formulas
2. [ ] Mandragora system
3. [ ] Unknown field investigation

### Phase 5: Major Features (Long-term)
1. [ ] Pawn Expeditions
2. [ ] Special Skill Augmentation
3. [ ] Main Story Quests
4. [ ] Grand Missions

---

## Technical Patterns to Follow

### Handler Pattern
```csharp
public class ExampleHandler : GameRequestPacketHandler<C2SExampleReq, S2CExampleRes>
{
    public override S2CExampleRes Handle(GameClient client, C2SExampleReq request)
    {
        // Use manager for business logic
        // Return populated response
    }
}
```

### Asset Pattern
```csharp
// 1. Create Asset class
public class ExampleAsset { ... }

// 2. Create Deserializer
public class ExampleAssetDeserializer : IAssetDeserializer<ExampleAsset> { ... }

// 3. Register in AssetRepository
RegisterAsset(value => ExampleAsset = value, "Example.json", new ExampleAssetDeserializer());
```

### Database Migration Pattern
```sql
-- Files/Database/Script/migration/sql_lite/ver_XX_YY_ZZ_description.sql
CREATE TABLE IF NOT EXISTS new_table (...);
ALTER TABLE existing_table ADD COLUMN new_column TYPE;
```

---

## Files for Next Implementation Session

### Clan Scout System Files
```
Arrowgene.Ddon.GameServer/Handler/ClanClanScoutEntrySearchHandler.cs
Arrowgene.Ddon.GameServer/Handler/ClanClanScoutEntryGetInviteListHandler.cs
Arrowgene.Ddon.GameServer/Handler/ClanClanScoutEntryGetInvitedListHandler.cs
Arrowgene.Ddon.GameServer/Handler/ClanClanGetHistoryHandler.cs
Arrowgene.Ddon.GameServer/Handler/ClanClanGetJoinRequestedListHandler.cs
Arrowgene.Ddon.GameServer/Handler/ClanClanGetMyJoinRequestListHandler.cs
Arrowgene.Ddon.GameServer/Characters/ClanManager.cs
Arrowgene.Ddon.Database/Sql/Core/DdonSqlDbClan.cs
Files/Database/Script/migration/
```

### Treasure Point System Files
```
Arrowgene.Ddon.GameServer/Handler/InstanceTreasurePointGetCategoryListHandler.cs
Arrowgene.Ddon.GameServer/Handler/InstanceTreasurePointGetListHandler.cs
Arrowgene.Ddon.Shared/Asset/TreasurePointAsset.cs (new)
Arrowgene.Ddon.Shared/AssetReader/TreasurePointAssetDeserializer.cs (new)
Arrowgene.Ddon.Shared/Files/Assets/TreasurePoints.json (new)
```

---

## References

- Handler Analysis: `scratch/handler_analysis.md`
- Manager Analysis: `scratch/manager_analysis.md`
- GitHub Issues/PRs: `scratch/github_issues_prs.md`
- Feature Priority List: `scratch/FEATURE_PRIORITY_LIST.md`
- Retail Documentation: `scratch/retail_documentation.md`

---

*Generated: 2026-01-25*
