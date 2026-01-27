# Feature Implementation Plan - Arrowgene.DragonsDogmaOnline

**Date:** 2025-01-25
**Status:** Proposed

---

## Priority Ranking Criteria

1. **Player Impact** - How much does this feature affect gameplay?
2. **Dependency Chain** - Does other content depend on this?
3. **Implementation Complexity** - How much effort is required?
4. **Community Demand** - What do players want most?

---

## TIER 1: HIGH PRIORITY (Immediate Focus)

### 1.1 Complete Pawn Expedition System (75% → 100%)

**Current State:** Manager and handlers exist, core logic partial
**Effort:** Low-Medium
**Impact:** High - popular daily content

**Tasks:**
- [ ] Complete hot spot discovery system with randomization
- [ ] Implement area-based reward tables
- [ ] Add golden expedition (2-hour) support with gem cost
- [ ] Implement sally count recharge (gems)
- [ ] Add "expedition in progress" pawn restrictions
- [ ] Complete reward drop item system
- [ ] Test daily reset at 5:00 AM JST

**Files to Modify:**
- `GameServer/Characters/PawnExpeditionManager.cs`
- `GameServer/Handler/PawnExpedition*.cs` (9 handlers)
- Asset: Create expedition reward tables

---

### 1.2 Complete Mandragora Cultivation (30% → 100%)

**Current State:** Basic structure, minimal functionality
**Effort:** Medium
**Impact:** High - Season 3 signature feature

**Tasks:**
- [ ] Complete species discovery tracking
- [ ] Implement first discoverer registration
- [ ] Add cultivation time mechanics (18 hours)
- [ ] Implement fertilizer → species calculations
- [ ] Add Mandragora Dictionary functionality
- [ ] Complete craft recipe system for tickets
- [ ] Add soft soil requirement checks
- [ ] Integrate with Achievement system

**Files to Modify:**
- `GameServer/Characters/MandragoraManager.cs`
- `GameServer/Handler/Mandragora*.cs` (5 handlers)
- `Shared/Asset/MandragoraAsset.cs` (expand species data)

---

### 1.3 Complete Quick Party Matching (40% → 100%)

**Current State:** Manager stub, basic handlers
**Effort:** Medium
**Impact:** High - improves multiplayer experience

**Tasks:**
- [ ] Implement matching algorithm (content type, level range)
- [ ] Add quest-specific matching
- [ ] Implement 30-minute registration timeout
- [ ] Add 60-second confirmation system
- [ ] Create party formation from matched players
- [ ] Add registration cancellation
- [ ] Implement ready notifications

**Files to Modify:**
- `GameServer/Characters/QuickPartyManager.cs`
- `GameServer/Handler/QuickParty*.cs` (3 handlers)
- New packet structures as needed

---

### 1.4 Complete Remaining Extreme Missions (3/6 → 6/6)

**Current State:** Only EXM4 fully scripted
**Effort:** Medium per mission
**Impact:** Very High - core endgame content

**Tasks:**
- [ ] Script EXM1: The Call of the Catacombs (Lv58)
- [ ] Script EXM2: Drawn to Ancient Power (Lv60)
- [ ] Script EXM3: The Ancient City's Legacy (Lv60)
- [ ] Script EXM5: Agent of Corruption (Lv65)
- [ ] Script EXM6: Phantasmic Great Dragon (Lv70)
- [ ] Add mission-only consumables system
- [ ] Implement daily rare item limits

**Files to Create:**
- `scripts/quests/exm/q50101000.csx` (EXM1)
- `scripts/quests/exm/q50102000.csx` (EXM2)
- `scripts/quests/exm/q50103000.csx` (EXM3)
- `scripts/quests/exm/q50105000.csx` (EXM5)
- `scripts/quests/exm/q50106000.csx` (EXM6)

---

### 1.5 Main Story Quest Season 3.2-3.4

**Current State:** Season 3.0-3.1 complete
**Effort:** Medium-High
**Impact:** High - story progression

**Tasks:**
- [ ] Research Season 3.2 quest structure
- [ ] Script Season 3.2 MSQ sequence
- [ ] Research Season 3.3 quest structure
- [ ] Script Season 3.3 MSQ sequence
- [ ] Research Season 3.4 quest structure
- [ ] Script Season 3.4 MSQ sequence
- [ ] Test quest chain progression

**Files to Create:**
- `scripts/quests/msq/season3_2/*.csx`
- `scripts/quests/msq/season3_3/*.csx`
- `scripts/quests/msq/season3_4/*.csx`

---

## TIER 2: MEDIUM PRIORITY (Next Phase)

### 2.1 Grand Mission System (NEW)

**Current State:** Not implemented
**Effort:** Very High
**Impact:** High - major content type

**Design Requirements:**
- 8-player instancing (new system)
- Score accumulation mechanics
- Time window scheduling
- Entry board integration for 8 players
- Boss rage/break mechanics

**Tasks:**
- [ ] Design GrandMissionManager architecture
- [ ] Implement 8-player instance system
- [ ] Create score tracking system
- [ ] Add time window scheduling (weekend availability)
- [ ] Implement Entry Board for 8-player parties
- [ ] Create GM-specific quest type handlers
- [ ] Script Gritten Fortress Siege
- [ ] Script Crucible of Monsters
- [ ] Script Ancient Strong Ones

**New Files Required:**
- `GameServer/Characters/GrandMissionManager.cs`
- `GameServer/Handler/GrandMission*.cs` (estimated 10+ handlers)
- Quest scripts for each GM

---

### 2.2 War Mission System (NEW)

**Current State:** Wallet type exists, no implementation
**Effort:** Very High
**Impact:** High - Season 3 endgame

**Design Requirements:**
- Similar to GM but with Dominion Points
- Multiple objective types
- Boss mechanics (Four Demon Generals)
- Ranking system integration

**Tasks:**
- [ ] Design WarMissionManager architecture
- [ ] Implement Dominion Points earning/spending
- [ ] Create objective type handlers (annihilation, fortress)
- [ ] Implement DP shop functionality
- [ ] Add ranking/leaderboard system
- [ ] Script Boss encounters (4 Demon Generals)

**New Files Required:**
- `GameServer/Characters/WarMissionManager.cs`
- Expand NPC shop for DP items
- Quest scripts for War Mission content

---

### 2.3 Bitterblack Maze Completion (70% → 100%)

**Current State:** Core mechanics work, gaps in rewards
**Effort:** Medium
**Impact:** Medium-High

**Tasks:**
- [ ] Complete reward bonus handling
- [ ] Fix unclear reward distribution logic
- [ ] Add better error codes for tier progression
- [ ] Complete mark reward system
- [ ] Test all tier/stage combinations

---

### 2.4 Epitaph Road Completion (70% → 100%)

**Current State:** Most mechanics work, some unclear
**Effort:** Medium
**Impact:** Medium

**Tasks:**
- [ ] Clarify progress reset/persistence logic
- [ ] Complete PosId value assignments
- [ ] Add configurable difficulty settings
- [ ] Test weekly reward claiming

---

### 2.5 Job Master System

**Current State:** Partial implementation
**Effort:** Medium
**Impact:** Medium - affects skill unlocks

**Tasks:**
- [ ] Implement job master NPCs
- [ ] Add training unlock progression
- [ ] Complete skill/augment gating
- [ ] Add job master dialogue/quests

---

## TIER 3: LOWER PRIORITY (Future Work)

### 3.1 Season 2.x/3.x Orb Trees (50% → 100%)
- Complete blood orb tree progression
- Add holy orders tree
- Implement all stat unlocks

### 3.2 Personal Quests Completion
- Fill gaps in personal quest coverage
- Add missing vocation quests

### 3.3 World Manage Quests
- Implement server-wide event system
- Add custom event quest distribution

### 3.4 Dragon Abilities System
- Design dragon ability unlock mechanics
- Implement dragon ability skills

### 3.5 Equipment Extreme Synthesis
- Add advanced crafting tier
- Implement synthesis recipes

### 3.6 Clan Extreme Missions
- Clan-specific high-difficulty content
- Clan-wide rewards

### 3.7 Premium Shop / Gacha
- Cash shop item delivery
- Gacha mechanics (low priority)

---

## Implementation Schedule Recommendation

### Phase 1 (Immediate - 2-4 weeks)
1. Complete Pawn Expedition (1.1)
2. Complete Mandragora Cultivation (1.2)
3. Complete Quick Party (1.3)

### Phase 2 (Short-term - 1-2 months)
4. Complete Extreme Missions (1.4)
5. Season 3.2-3.4 MSQ (1.5)
6. Bitterblack Maze completion (2.3)

### Phase 3 (Medium-term - 2-4 months)
7. Grand Mission System (2.1)
8. Epitaph Road completion (2.4)
9. Job Master System (2.5)

### Phase 4 (Long-term - 4+ months)
10. War Mission System (2.2)
11. Tier 3 features as resources allow

---

## Resource Requirements

### Development Focus Areas
1. **Quest Scripting** - MSQ, EXM content
2. **Manager Development** - GM/WM systems
3. **Database Work** - New tables for features
4. **Asset Creation** - Reward tables, species data

### Testing Requirements
- Multi-player testing for party content
- Time-based testing for expeditions
- Economy balancing for rewards

---

## Success Metrics

- [ ] All 6 Extreme Missions playable
- [ ] Pawn Expedition fully functional with hot spots
- [ ] Mandragora cultivation working with 12 species
- [ ] Quick Party matching functional
- [ ] Season 3.2-3.4 MSQ completable
- [ ] Grand Missions playable (if implemented)

---

## Notes

This plan prioritizes completing partially-implemented features before starting entirely new systems. The recently added features (Pawn Expedition, Mandragora, Quick Party) represent significant progress and should be finished first to provide a more complete gameplay experience.

Grand Missions and War Missions are complex 8-player systems that will require substantial architecture work. These should be tackled after the simpler features are complete and stable.
