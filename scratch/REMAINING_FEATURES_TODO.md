# Remaining Features TODO Tracker

## Tier 1: High Impact, Low Effort

### [ ] Personal Mail System
**Files:** `MailMailGetListHeadHandler.cs`, `MailMailGetListDataHandler.cs`, `MailMailGetListFootHandler.cs`
**Effort:** Medium
**Impact:** High - Players expect mail functionality
**Notes:** System mail already works, needs personal mail DB schema and handler updates

### [ ] Treasure Points - COMPLETED ✅
**Status:** Implemented in Phase 2 with asset system

### [ ] GP Course - COMPLETED ✅
**Status:** Handler fixed in Phase 2

### [ ] Revival Timer - COMPLETED ✅
**Status:** Now persists in database

### [ ] Connection Cleanup - COMPLETED ✅
**Status:** Stale connections cleaned on reconnect

---

## Tier 2: High Impact, Medium Effort

### [ ] Pawn Expeditions
**Files:** `PawnExpeditionGetSallyInfoHandler.cs` + new handlers needed
**Effort:** High
**Impact:** High - Core pawn feature
**Needs:**
- Database schema for expedition tracking
- Multiple new handlers (start, return, claim rewards)
- Integration with pawn system

### [ ] Equipment Presets
**Files:** New handlers needed
**Effort:** Medium
**Impact:** Medium - Quality of life
**Needs:**
- Database schema for preset storage
- Handlers for create/load/delete presets
- Integration with equip system

### [ ] Mandragora System (Real Implementation)
**Files:** `MandragoraBeginCraftHandler.cs`, `MandragoraGetCraftRecipeListHandler.cs`, etc.
**Effort:** High
**Impact:** Medium - Companion breeding system
**Needs:**
- Database schema for mandragora persistence
- Replace hardcoded test data
- Implement breeding/evolution logic

---

## Tier 3: Content Additions

### [ ] MSQ Season 3.2-3.4
**Location:** `scripts/quests/msq/`
**Effort:** Very High
**Impact:** High - Story completion
**Notes:** Requires quest scripting expertise

### [ ] World Quests - Missing Zones
**Zones:**
- [ ] Urteca Mountains
- [ ] Memory of Megadosys
- [ ] Memory of Urteca
- [ ] Bitterblack Maze quests
**Effort:** High per zone
**Impact:** Medium - World completion

### [ ] Personal Quests
**Effort:** Very High
**Impact:** Medium
**Notes:** Not started at all

---

## Tier 4: Major Systems (High Effort)

### [ ] Quick Party Matchmaking
**Files:** No handlers exist
**Effort:** Very High
**Impact:** High - Multiplayer convenience
**Needs:**
- Packet structure definitions
- Multiple new handlers
- Matchmaking algorithm
- Database queue system

### [ ] Grand Missions
**Files:** No handlers exist
**Effort:** Very High
**Impact:** Medium - Endgame content
**Needs:**
- Research original system mechanics
- Packet definitions
- Handler implementations
- Quest/reward integration

### [ ] War Missions
**Files:** No handlers exist
**Effort:** Very High
**Impact:** Medium - Endgame content
**Notes:** Poorly documented, scope unclear

### [ ] Clan Extreme Missions
**Files:** Handlers may exist but no quest content
**Effort:** Very High
**Impact:** Medium - Clan endgame

---

## Tier 5: Premium/Extended Features

### [ ] Gacha System
**Files:** No handlers exist
**Effort:** Very High
**Impact:** Low (monetization feature)
**Needs:**
- Full system design
- Packet structures
- Handler implementations
- Rate configuration system

### [ ] Dragon Ability Synthesis
**Files:** Orb devote handlers exist, synthesis missing
**Effort:** Medium
**Impact:** Medium - Endgame progression
**Needs:**
- Synthesis handlers
- Dismantling handlers
- Recipe/formula system

### [ ] Job Masters / S2-S3 Orb Trees
**Effort:** Very High
**Impact:** High - Skill progression
**Notes:** Affects skill accuracy

---

## Implementation Order Recommendation

**Phase 3 (Next Sprint):**
1. Personal Mail System (Medium effort, high value)
2. Equipment Presets (Medium effort, QoL improvement)
3. Pawn Expeditions (High effort, core feature)

**Phase 4:**
4. Dragon Ability Synthesis (Complete existing system)
5. Mandragora Real Implementation (Replace stubs)
6. Clan Scout Full Implementation (Expand Phase 2 work)

**Phase 5:**
7. Quick Party System (Major undertaking)
8. Grand Missions (Content + handlers)

**Future:**
- MSQ 3.2-3.4 (Requires dedicated quest developer)
- World Quest zones
- War Missions
- Gacha System
