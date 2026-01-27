# DDON Server Repository - GitHub Issues and PRs Analysis

**Generated:** 2026-01-25
**Repository:** sebastian-heinz/Arrowgene.DragonsDogmaOnline

---

## Table of Contents
1. [Open Issues Summary](#open-issues-summary)
2. [Open Pull Requests](#open-pull-requests)
3. [Closed but Unmerged PRs](#closed-but-unmerged-prs)
4. [Recently Merged PRs](#recently-merged-prs)
5. [Feature Trackers](#feature-trackers)
6. [GP/Cash Shop Status](#gpcash-shop-status)
7. [Unimplemented Features](#unimplemented-features)
8. [Known Bugs](#known-bugs)

---

## Open Issues Summary

### By Category

#### Bugs (High Priority)
| # | Title | Labels | Created |
|---|-------|--------|---------|
| 715 | Spirit Lancer Cure Glasta desync issues | bug | 2025-02-21 |
| 639 | Skill desync in parties | bug | 2024-11-26 |
| 529 | Revival timer resets across channels due to storing time in memory | bug, good first issue | 2024-09-03 |
| 468 | Connection entries don't get removed on disconnection sometimes | bug, help wanted | 2024-08-22 |
| 458 | Enemy OM Data - destructible parts state not cleared on respawn | bug, help wanted | 2024-08-19 |
| 358 | Core skills don't change properly when switching jobs | bug | 2024-06-22 |
| 110 | Party ID pool runs out of IDs over time | bug | 2022-12-17 |

#### Enhancement Requests
| # | Title | Labels | Created |
|---|-------|--------|---------|
| 685 | Implement Season 3.x BO/HO Tree "Special Skill Augmentation" | enhancement | 2025-01-05 |
| 449 | Refactor JobManager SwapEquipmentAndStorage | enhancement | 2024-08-17 |
| 297 | Weather affecting gathering spots | enhancement, good first issue | 2024-05-07 |

#### Trackers (Content Progress)
| # | Title | Labels | Status |
|---|-------|--------|--------|
| 686 | Content Tracker (Main) | tracker | Active - tracks all content |
| 683 | Epitaph Road Tracker | help wanted, tracker | In Progress |
| 477 | Bitterblack Maze Tracker | help wanted, tracker | Partially Complete |

#### Build/Infrastructure
| # | Title | Labels | Created |
|---|-------|--------|---------|
| 101 | make publish.cmd not output scripts unless build was successful | good first issue | 2022-10-04 |

---

## Open Pull Requests

### Active Development PRs

#### 1. GP Shop PR Cleanup (#724)
- **Author:** RyanYappert
- **Created:** 2025-03-07
- **Branch:** feature/gp-shop
- **Status:** Open, waiting for review
- **Description:** Cleans up and preps existing GP shop code from @Sehkah and @pacampbell for merging.
- **Changes:**
  - Ensures constructors and serializers properly instantiate their members and strings
  - Removes raw references to localhost and constructs paths from game settings
  - Removes possibly copyrighted material (needs separate archiving)
  - Handlers blocked off from players with ResponseErrorException (early state)
- **Blockers:** Some handlers still in early state with hardcoded functionality

#### 2. Equipment Presets (#590)
- **Author:** pacampbell
- **Created:** 2024-10-04
- **Branch:** gear_preset
- **Status:** Open
- **Description:** Added the ability to save equipment presets
- **Blockers:** None mentioned

#### 3. Clan Requests (#856)
- **Author:** MrDetonia
- **Created:** 2025-05-26
- **Branch:** develop
- **Status:** Open
- **Description:** Implements Clan Request feature (join requests with approval)
- **Features:**
  - New table "ddon_clan_requests" with migration strategy
  - Handlers for registering, cancelling, approving/rejecting requests
  - Clan permissions respected
  - Weekly scheduled task to remove stale requests (30+ days)
  - Offline characters can be added to clan and notified on login
- **Blockers:** Scouting system remains unimplemented

#### 4. Fix EM4 Skill Unlock (#910)
- **Author:** pacampbell
- **Created:** 2025-06-27
- **Branch:** update_em4_skill_release
- **Status:** Open
- **Description:** Adjusts EM4 skills to unlock only on the job which completes it

#### 5. Move Solo Quest State (#890)
- **Author:** pacampbell
- **Created:** 2025-06-12
- **Branch:** adjust_solo_party_state
- **Status:** Open
- **Description:** Relocates solo quest state from party to client for EXM and Party Finder compatibility

#### 6. Email & Password Features (#961)
- **Author:** D00MK1D
- **Created:** 2025-09-12
- **Branch:** develop
- **Status:** Open (for review)
- **Description:** Implements email requirement and password change features for accounts

#### 7. Update objectives.md (#973)
- **Author:** Sapphiratelaemara
- **Created:** 2025-11-13
- **Branch:** develop
- **Status:** Open
- **Description:** Updated Epitaph Road objectives translations

---

## Closed but Unmerged PRs

### GP Shop Related (Important - Feature Not Merged)

#### Feature/GP Shop (#644) - CLOSED, NOT MERGED
- **Author:** pacampbell (authored by @Sehkah)
- **Created:** 2024-11-29
- **Closed:** 2025-03-25
- **Reason:** Superseded by PR #724
- **Features Implemented:**
  - Allow purchasing GG for CAP (currently for free)
  - Drawing from "normal treasure lot" gacha
  - Drawing from "box treasure lot" gacha
  - Event code input
- **Packets Implemented:**
  - BOX_GACHA_BOX_GACHA_BUY_REQ/RES
  - BOX_GACHA_BOX_GACHA_DRAW_INFO_REQ/RES
  - BOX_GACHA_BOX_GACHA_LIST_REQ/RES
  - GACHA_GACHA_BUY_REQ/RES
  - GACHA_GACHA_LIST_REQ/RES
  - EVENT_CODE_EVENT_CODE_INPUT_REQ/RES
  - GP_CHANGE_CAP_TO_GP_REQ/RES
  - GP_COG_GET_ID_REQ/RES
  - GP_GP_SHOP_DISPLAY_GET_TYPE_REQ/RES
  - ITEM_GET_ITEM_STORAGE_INFO_REQ/RES
- **Open Issues at Close:**
  - Assets for box gacha, normal gacha, event codes needed
  - CAP balance not working in-game
  - Online Shop closes client (malformed packets?)
  - Need more old campaign/banner images

#### Feature/GP Shop (#422) - CLOSED, NOT MERGED
- **Author:** Sehkah
- **Created:** 2024-08-11
- **Closed:** 2024-11-29
- **Reason:** Superseded by #644 (which was then superseded by #724)
- **Dependencies:** Required PR #416

### Other Unmerged PRs

#### Update gmd.csv (#920)
- **Author:** CaptainFlynt-ddo
- **Created:** 2025-07-03
- **Closed:** 2025-07-20
- **Reason:** Unknown
- **Description:** Changed "Waterfall Echo Cave" to "Echo Cascade Cavern"

---

## Recently Merged PRs

### December 2025
| # | Title | Author | Merged |
|---|-------|--------|--------|
| 974 | New release | alborrajo | 2025-12-02 |

### November 2025
| # | Title | Author | Merged |
|---|-------|--------|--------|
| 972 | Replace Console.ReadKey with Console.Read | LostSoru | 2025-11-02 |

### October 2025
| # | Title | Author | Merged |
|---|-------|--------|--------|
| 971 | Gathering Table update | 00MeiMei | 2025-10-16 |
| 970 | Hotfix: BBM Resets and Emblem Recovery | RyanYappert | 2025-10-11 |
| 969 | Gathering Table update | 00MeiMei | 2025-10-10 |
| 968 | Small Fixes + Valuable Item Recovery | RyanYappert | 2025-10-07 |
| 967 | Fix: Incorrect Item Prices | edelarrow | 2025-10-02 |

### September 2025
| # | Title | Author | Merged |
|---|-------|--------|--------|
| 966 | Hotfix: BBM takeaway rerolling items | RyanYappert | 2025-09-24 |
| 965 | Hotfix: Fix crashing issue with logging | RyanYappert | 2025-09-24 |
| 964 | Hotfix: Channel "Meltdown" Mitigation | RyanYappert | 2025-09-24 |
| 963 | Fix AR13 Kingal Canyon Caution Spot | pacampbell | 2025-09-21 |
| 962 | default gathering: Update rewards | pacampbell | 2025-09-21 |
| 958 | Fix: Bugfix Stack 08-21 ~ 09-02 | RyanYappert | 2025-09-17 |
| 957 | **Feat: BBM Sealing and Reset Tickets** | RyanYappert | 2025-09-11 |
| 960 | log camellia key for network troubleshooting | sebastian-heinz | 2025-09-05 |
| 959 | S2 Caution Spots | pacampbell | 2025-09-03 |

### August 2025
| # | Title | Author | Merged |
|---|-------|--------|--------|
| 956 | Hotfix: Fix Postgres migration | RyanYappert | 2025-08-27 |
| 955 | Hotfix: Fix Postgres migration | RyanYappert | 2025-08-26 |
| 953 | **Feat: Craft Additional Status/Slayer Stones** | RyanYappert | 2025-08-25 |
| 948 | **Fix: Pawn EX Skills** | RyanYappert | 2025-08-25 |
| 954 | update: Update default gathering | pacampbell | 2025-08-04 |
| 942 | **Feature: Fashion Commands** | RyanYappert | 2025-08-04 |
| 949 | Feat/Fix: Small Fixes 8/2 | RyanYappert | 2025-08-04 |

### July 2025
| # | Title | Author | Merged |
|---|-------|--------|--------|
| 946 | Fixes: Followup on rental pawns | RyanYappert | 2025-07-28 |
| 947 | fix: Fix area id in default gathering | pacampbell | 2025-07-27 |
| 945 | fix: Fix issues found in 3.1 tree | pacampbell | 2025-07-23 |
| 944 | Hotfix: Fix bugs from small fixes | RyanYappert | 2025-07-23 |

---

## Feature Trackers

### Content Tracker (#686)
**Status:** Active tracking issue

#### Main Story Quests
- **Season 1:** Issues #335, #346, #351
- **Season 2:** Issues #568, #569, #571, #574
- **Season 3:** Issues #877, #897
  - Season 3.2, 3.3, 3.4 - In Progress

#### World Quests
| Area | Issue | Status |
|------|-------|--------|
| Hidell Plains | #311 | Done |
| Breya Coast | #362 | Done |
| Mysree Forest | #433 | Done |
| Volden Mines | #432 | Done |
| Dowe Valley | #437 | Done |
| Mysree Grove | #440 | Done |
| Deenan Woods | #465 | Done |
| Betland Plains | #445 | Done |
| Northern Betland Plains | #470 | Done |
| Zandora Wastelands | #516 | Done |
| Eastern Zandora | #532 | Done |
| Mergoda Ruins | #533 | Done |
| Bloodbane Isle | #593 | Done |
| Elan Water Grove | #604 | Done |
| Farana Plains | #617 | Done |
| Morrow Forest | #623 | Done |
| Kingal Canyon | #623 | Done |
| Rathnite Foothills | #929 | Done |
| Feryana Wilderness | #932 | Done |
| Megadosys Plateau | #933 | Done |
| Urteca Mountains | - | Not Started |
| Memory of Megadosys | - | None Exist |
| Memory of Urteca | - | None Exist |
| Bitterblack Maze | - | None Exist |

#### Board Quests
- Season 1: #616, #635
- Season 3: #931

### Bitterblack Maze Tracker (#477)
**Status:** Partially Complete

#### Completed (Strikethrough in original)
- Reset System (#957)
- Tracking treasure claimed
- Reset progress consuming tickets properly
- Reset tickets awarded weekly
- Bitterblack Equipment Effect Seal (#957)
- Item sort between game modes (#958)
- Inventory deletion on homepoint after death (#511)
- Original quest to start BBM (#543)

#### Still Needed
- Using Pawns in BBM
- Player ready up menu
- Clear timestamp is incorrect
- Some enemies get stuck on terrain
- Players not in party sometimes can't be seen
- Equipment exchange between game modes (not 100%)
- Better loot algorithm (#503)
- Randomized enemy levels (dynamic level algorithm needed)

### Epitaph Road Tracker (#683)
**Status:** In Progress

#### Completed
- Core systems required for content (#619)
- Heroic Spirit Sleeping Path: Rathnite Foothills (#619)
- Heroic Spirit Sleeping Path: Feryana Wilderness (#899)

#### Still Needed
- Special Skill Augmentation (#685) - Season 3.x BO/HO Tree
- Memory of Megadosys
- Memory of Urteca

---

## GP/Cash Shop Status

### Current State
The GP Shop feature is **NOT YET MERGED** into the main codebase.

### History
1. **PR #422** (Aug 2024) - Original implementation by Sehkah, closed
2. **PR #644** (Nov 2024) - Updated by pacampbell, closed as superseded
3. **PR #724** (Mar 2025) - Cleanup PR by RyanYappert, **STILL OPEN**

### Implemented Features (in branch)
- Purchasing Golden Gemstones (GG) for CAP
- Normal treasure lot gacha
- Box treasure lot gacha
- Event code input
- GP Course Effects (merged via #262)

### Related Merged Features
- **PR #379** (Jul 2024) - GP/Golden Gemstones packets and handlers
- **PR #262** (Apr 2024) - GP course effects support (configurable via GpCourseInfo.json)
- **PR #314** (May 2024) - Fix time desync when opening GP shop menus

### Known Issues with GP Shop
1. Assets needed for box gacha, normal gacha, event codes
2. CAP balance not working in-game
3. Online Shop closes client (possibly malformed packets)
4. Need more campaign/banner images
5. Some handlers have hardcoded values that need configuring

---

## Unimplemented Features

### High Priority (Based on Trackers)
1. **Season 3.x Special Skill Augmentation** (#685)
   - Requires new HO currency
   - Upgrades for player and pawn
   - Used to upgrade EX skills

2. **Bitterblack Maze Pawns**
   - Using pawns in BBM not implemented
   - Player ready up menu missing

3. **Memory Dungeons**
   - Memory of Megadosys
   - Memory of Urteca

4. **Season 3 Story Content**
   - Season 3.2, 3.3, 3.4 story quests

### Medium Priority
1. **Clan Scouting System** (mentioned in PR #856)
2. **Weather affecting gathering spots** (#297)
3. **Equipment Presets** (#590 - PR open)

### Community Requests
1. Better pawn AI and commands
2. More quest content
3. Shop functionality (GP/Cash shop)

---

## Known Bugs

### Critical/Game-Breaking
| # | Title | Impact |
|---|-------|--------|
| 110 | Party ID pool runs out of IDs over time | Server becomes unusable after ~3 months |
| 468 | Connection entries don't get removed on disconnection | Players get locked out |

### Multiplayer/Sync Issues
| # | Title | Impact |
|---|-------|--------|
| 715 | Spirit Lancer Cure Glasta desync | Healing doesn't work for party members |
| 639 | Skill desync in parties | Skills don't function for non-leaders |
| 358 | Core skills don't change properly when switching jobs | Wrong skills available |

### Gameplay Issues
| # | Title | Impact |
|---|-------|--------|
| 529 | Revival timer resets across channels | Exploit - unlimited revivals |
| 458 | Enemy OM Data - destructible parts not cleared | Visual/gameplay issues on respawn |
| 449 | JobManager SwapEquipmentAndStorage architecture | Code quality/potential bugs |

### Good First Issues (Marked in Repository)
| # | Title | Labels |
|---|-------|--------|
| 529 | Revival timer resets across channels | bug, good first issue |
| 297 | Weather affecting gathering spots | enhancement, good first issue |
| 101 | make publish.cmd not output scripts unless successful | good first issue |

---

## Active Contributors (Recent)

Based on merged PRs (Aug-Dec 2025):
1. **RyanYappert** - Major feature development (BBM, Fashion, Slayer Stones)
2. **pacampbell** - Quest content, gathering, bug fixes
3. **00MeiMei** - Gathering table updates
4. **sebastian-heinz** - Core infrastructure
5. **alborrajo** - Releases
6. **LostSoru** - Platform compatibility

---

## Development Priorities (Inferred)

Based on recent activity:
1. **Bitterblack Maze completion** - Multiple recent PRs
2. **Gathering/World content** - Regular updates
3. **Bug fixes and stability** - Hotfixes pattern
4. **Epitaph Road content** - Ongoing tracker

### Stalled/Waiting
1. **GP Shop** - PR #724 open since March 2025
2. **Equipment Presets** - PR #590 open since October 2024
3. **Clan Requests** - PR #856 open since May 2025

---

## Recommendations for Contributors

### Easy Entry Points
1. Fix revival timer channel exploit (#529)
2. Implement weather effects on gathering (#297)
3. Improve publish.cmd build script (#101)

### Medium Complexity
1. Help with BBM pawn support (#477)
2. Work on Season 3 content (#686)
3. Review and test GP Shop PR (#724)

### High Complexity
1. Fix multiplayer desync issues (#715, #639)
2. Implement Memory dungeons (#683)
3. Fix Party ID pool leak (#110)
