# DDON Server Feature Priority List

**Analysis Date:** 2026-01-25
**Based on:** 12 research agents analyzing wiki, GitHub issues/PRs, handlers, managers, database, packets, quests, scripts, and retail documentation

---

## Executive Summary

The DDON server emulator has **~83% of handlers fully implemented** with solid core gameplay. Major gaps exist in:
1. **GP/Premium Shop System** - Has open PR #724 that needs cleanup and merge
2. **Pawn Expeditions** - Not implemented, blocks clan buff shop
3. **Quest Bonus Systems** - 4 stub handlers for area/level/party bonuses
4. **Clan Scout/Recruitment** - 6 stub handlers
5. **Missing Main Story Quests** - Seasons 1.1-2.3 lack scripted implementations

---

## Priority 1: Critical Features (High Impact, Low-Medium Effort)

### 1.1 Merge and Fix GP Shop System (PR #724)

**Status:** Open PR since March 2025, implements gacha/shop functionality
**Impact:** Enables premium currency features, event codes, Golden Gemstone purchases
**Known Issues:**
- CAP balance not working
- Potential client crashes
- Needs cleanup

**Approach:**
1. Review PR #724 files and understand implementation
2. Fix CAP balance calculation
3. Add proper error handling to prevent crashes
4. Test gacha drawing, event code redemption
5. Merge or create improved PR

**Files to modify:**
- `GpCourseManager.cs`
- GP-related handlers in `Handler/`
- Possibly add database tables for purchase history

---

### 1.2 Quest Bonus Systems (4 Stub Handlers)

**Status:** 4 handlers return empty responses
**Impact:** Missing retail-accurate quest rewards

**Stub Handlers:**
- `QuestGetAreaBonusListHandler` - Area-specific bonuses
- `QuestGetLevelBonusListHandler` - Level-based bonuses
- `QuestGetPartyBonusListHandler` - Party composition bonuses
- `QuestGetQuestPartyBonusListHandler` - Quest-specific party bonuses

**Approach:**
1. Research retail bonus systems from wiki documentation
2. Create asset files for bonus definitions
3. Implement handler logic to return appropriate bonuses
4. Add support in `QuestManager` for bonus calculations

**Estimated effort:** Medium (need to understand retail behavior)

---

### 1.3 Stamp Bonus Receive Handlers

**Status:** Packets exist but handlers missing
**Impact:** Players cannot claim daily/total stamp rewards

**Missing Handlers:**
- `C2SStampBonusRecieveDailyReq` - Receive daily stamp
- `C2SStampBonusRecieveTotalReq` - Receive total stamp milestone

**Approach:**
1. `StampManager.cs` already tracks stamps
2. Add handlers that call manager methods
3. Return appropriate rewards from stamp bonus asset

**Estimated effort:** Low (infrastructure exists)

---

## Priority 2: Important Features (Medium Impact)

### 2.1 Clan Scout/Recruitment System (6 Stub Handlers)

**Status:** 6 handlers are stubs
**Impact:** Clans cannot recruit members via in-game system

**Stub Handlers:**
- `ClanClanGetHistoryHandler` - History not stored
- `ClanClanGetJoinRequestedListHandler`
- `ClanClanGetMyJoinRequestListHandler`
- `ClanClanScoutEntrySearchHandler`
- `ClanClanScoutEntryGetInviteListHandler`
- `ClanClanScoutEntryGetInvitedListHandler`

**Approach:**
1. Add database tables for clan history and join requests
2. Add migration scripts
3. Implement handler logic
4. Add RPC support for cross-channel clan operations

**Estimated effort:** Medium-High (needs database schema work)

---

### 2.2 Treasure Point System (2 Stub Handlers)

**Status:** 2 handlers are stubs
**Impact:** Treasure point collection not working

**Stub Handlers:**
- `InstanceTreasurePointGetCategoryListHandler`
- `InstanceTreasurePointGetListHandler`

**Approach:**
1. Research treasure point system in retail
2. Create asset files for treasure point definitions
3. Implement handlers with database backing

**Estimated effort:** Medium

---

### 2.3 Friend Request System

**Status:** Packets exist but handlers missing
**Impact:** Cannot add friends through in-game UI

**Missing Handlers:**
- `C2SFriendApplyFriendReq`
- `C2SFriendApproveFriendReq`

**Approach:**
1. Database `ddon_contact_list` table already exists
2. Add handlers to manage friend requests
3. Send appropriate notifications

**Estimated effort:** Low-Medium

---

### 2.4 Pawn Expeditions

**Status:** Not implemented
**Impact:** Blocks `ClanClanShopBuyBuffItemHandler` (throws exception)

**Affected Systems:**
- Pawn expedition sending/return
- Expedition rewards
- Clan buff shop (requires expedition currency)

**Approach:**
1. Create expedition state management in database
2. Add expedition-related handlers (already have packets)
3. Implement timer-based expedition completion
4. Enable clan buff shop once expedition currency available

**Estimated effort:** High (new major system)

---

## Priority 3: Content Expansion

### 3.1 Main Story Quest Scripts (Seasons 1.1-2.3)

**Status:** Only Season 3.0+ has scripted quests; earlier seasons rely on incomplete JSON
**Impact:** Story progression blocked or buggy

**Missing Seasons:**
- Season 1.1 - 6+ quests needed
- Season 1.2 - Multiple quests
- Season 1.3 - Multiple quests
- Season 2.0-2.3 - Many quests

**Approach:**
1. Use existing quest JSON assets as base
2. Convert to scripted quests for proper flow
3. Add missing NPC dialogue, events, flags
4. Test quest chains end-to-end

**Estimated effort:** Very High (content creation)

---

### 3.2 Wild Hunt Quest System

**Status:** Framework exists (`QuestType.WildHunt = 15`) but no implementations
**Impact:** Missing endgame content

**Approach:**
1. Research Wild Hunt mechanics from retail documentation
2. Create quest scripts following existing patterns
3. Add rotation/scheduling support

**Estimated effort:** High

---

### 3.3 Grand Missions

**Status:** Not implemented
**Impact:** Missing 8-player raid content

**Approach:**
1. Research Grand Mission mechanics
2. Implement matchmaking/queue system
3. Create mission instances
4. Add scoring and reward systems

**Estimated effort:** Very High (major new system)

---

## Priority 4: Polish and Fixes

### 4.1 Critical Bug Fixes (from GitHub Issues)

**#110 - Party ID Pool Exhaustion**
- Server becomes unusable after ~3 months
- PartyManager needs ID reclamation fix

**#468 - Connection Cleanup**
- Players locked out when connection entries not removed
- Fix cleanup in disconnect handling

**#639/#715 - Skill Desync in Parties**
- Skills show incorrect state for party members
- Context synchronization issue

---

### 4.2 CraftManager Formula Implementation

**Status:** Multiple TODOs for "figuring out actual formulas"
**Impact:** Craft time, quality, great success may be inaccurate

**TODOs:**
- Craft time calculation
- Pawn craft bonus calculation
- Quality calculation
- Great success chance calculation

**Approach:**
1. Research retail formulas from community data
2. Implement proper calculations
3. Add configuration for server-specific tuning

---

### 4.3 Mandragora Crafting System

**Status:** Hardcoded test values
**Impact:** Mandragora cultivation not working properly

**Handler:** `MandragoraBeginCraftHandler`

**Approach:**
1. Research retail Mandragora mechanics
2. Create proper asset files
3. Implement growth/crafting logic

---

## Priority 5: Future Features (Low Priority)

### 5.1 Mail System (Player-to-Player)

**Status:** System mail works, player mail not implemented
**Impact:** Cannot send items/messages to other players

### 5.2 Equipment Extreme Synthesis

**Status:** Handler throws `ERROR_CODE_NOT_IMPLEMENTED`
**Impact:** Ultimate equipment enhancement not available

### 5.3 Quick Party System

**Status:** Not implemented
**Impact:** Cannot use quick party matching

### 5.4 Reward Missions

**Status:** Not implemented

### 5.5 Dragon Abilities

**Status:** Not implemented

---

## Implementation Approach Summary

### Phase 1 (Immediate - 1-2 weeks)
1. Review and merge GP Shop PR #724 with fixes
2. Implement Stamp Bonus receive handlers
3. Fix Party ID pool exhaustion bug (#110)

### Phase 2 (Short-term - 2-4 weeks)
1. Implement Quest Bonus handlers (4 stubs)
2. Implement Friend Request handlers
3. Fix connection cleanup bug (#468)

### Phase 3 (Medium-term - 1-2 months)
1. Implement Clan Scout/Recruitment system
2. Implement Treasure Point system
3. Start Pawn Expedition framework

### Phase 4 (Long-term - Ongoing)
1. Script main story quests (Seasons 1.1-2.3)
2. Implement Wild Hunt system
3. Implement Grand Missions
4. Polish and formula fixes

---

## Technical Considerations

### Database Changes Needed
- Clan history table
- Clan join requests table
- Treasure points table
- Pawn expedition state tables
- GP transaction logging (optional)

### Asset Files Needed
- Quest bonus definitions
- Treasure point definitions
- Wild Hunt quest data
- Grand Mission definitions

### Handler Patterns to Follow
- Use existing `*Manager` classes for business logic
- Support `DbConnection?` for transactions
- Use `PacketQueue` for batched responses
- Follow existing naming conventions

---

## References

- Wiki analysis: `scratch/wiki_analysis.md`
- Handler analysis: `scratch/handler_analysis.md`
- Manager analysis: `scratch/manager_analysis.md`
- Quest analysis: `scratch/quest_analysis.md`
- Database analysis: `scratch/database_analysis.md`
- Packet structures: `scratch/packet_structures.md`
- Retail documentation: `scratch/retail_documentation.md`
- GitHub issues/PRs: `scratch/github_issues_prs.md`
