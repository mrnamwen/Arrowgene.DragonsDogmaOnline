# What Works?

This document tracks feature implementation status for this fork of the Dragon's Dogma Online server emulator.

## Status Legend

| Symbol | Meaning |
|--------|---------|
| ✅ | Retail Accurate |
| ✔️ | Approximation of Retail |
| ⚠️ | Partially Implemented (Playable) |
| 🚧 | In Development |
| ⛔ | Partially Implemented (Unplayable) |
| ❌ | Unimplemented |
| ❔ | Status Unknown |

---

## Jobs / Vocations

| Feature | Status | Notes |
|---------|--------|-------|
| Fighter | ✔️ | All skills functional |
| Hunter | ✔️ | All skills functional |
| Priest | ✔️ | All skills functional |
| Shield Sage | ✔️ | All skills functional |
| Seeker | ✔️ | All skills functional |
| Sorcerer | ✔️ | All skills functional |
| Warrior | ✔️ | All skills functional |
| Element Archer | ✔️ | All skills functional |
| Alchemist | ✔️ | All skills functional |
| Spirit Lancer | ✔️ | All skills functional |
| High Scepter | ✔️ | All skills functional |
| Job Switching | ✅ | Instant job change with separate equipment |
| Job Levels | ✅ | Leveling and XP gain functional |
| Job Points (JP) | ✅ | JP accumulation and spending |
| Job Orb Tree (Season 1) | ✔️ | Basic orb tree unlocking |
| Job Orb Tree (Season 2/3) | ❌ | Job Masters not implemented |
| Job Emblems | ✔️ | Emblem equipment functional |
| EX Skills | ✔️ | Extended skills available |

---

## Core Systems

| Feature | Status | Notes |
|---------|--------|-------|
| Character Creation | ✅ | Full customization |
| Character Editing | ✅ | Beauty salon functional |
| Combat System | ✔️ | Full damage calculation, knockdowns, status effects |
| Leveling System | ✅ | XP, PP, and area rank progression |
| Gold / Currency | ✅ | Gold, rift points, blood orbs |
| Inventory Management | ✅ | Bag slots, item stacking |
| Equipment System | ✅ | All equipment slots functional |
| Equipment Presets | ✅ | Save/load equipment configurations |
| Storage System | ✅ | Personal storage, additional bags |
| Storage Expansion | ✅ | Purchasable storage slots via GP Shop |
| NPC Shops | ✅ | Buy/sell items |
| Teleportation | ✅ | Ferrystones, portcrystals, warp points |
| Weather System | ✅ | Dynamic weather and moon phases |
| Death Penalty | ✅ | Persistent across channel changes |
| Revival System | ✅ | Timer persists in database |
| Login Bonuses | ✅ | Stamp bonus system |
| Achievements | ✔️ | Trophy/achievement tracking |

---

## Economy & Trading

| Feature | Status | Notes |
|---------|--------|-------|
| Player Bazaar | ✅ | Full marketplace (exhibit, buy, proceeds) |
| NPC Bazaar | ✅ | NPC vendor selling items for gold |
| GP Shop | ✅ | Premium currency shop with items/bonuses |
| GP Courses | ✅ | Subscription bonuses (XP boost, etc.) |
| CAP to GP Conversion | ✅ | Currency conversion system |
| Gacha System | ✅ | Box gacha with draws, resets, prize pools |
| Event Code Redemption | ✅ | Redeemable promotional codes |

---

## Crafting Systems

| Feature | Status | Notes |
|---------|--------|-------|
| Standard Crafting | ✔️ | Item creation with pawns |
| Craft Time | ✔️ | Configurable (can be instant or timed) |
| Great Success | ✔️ | Configurable odds |
| Quality Enhancement | ✔️ | Item quality upgrades |
| Element Attachment | ✔️ | Add/remove elements |
| Equipment Grading | ✔️ | Grade-up system |
| Equipment Coloring | ✔️ | Color customization |
| Mandragora Crafting | ✅ | Special pawn-based crafting |
| Golden Mandragora | ✅ | Premium crafting variant |
| Craft Skill Leveling | ✔️ | Pawn craft skill progression |
| Ultimate Synthesis | ⚠️ | Basic implementation |

---

## Pawn System

| Feature | Status | Notes |
|---------|--------|-------|
| Main Pawn Creation | ✅ | Full customization |
| Support Pawn Rental | ✅ | Rift search functional |
| Pawn Combat AI | ✔️ | AI behavior works |
| Pawn Skills | ✅ | All pawn skills learnable |
| Pawn Levels | ✅ | XP and leveling |
| Pawn Orb Tree | ✔️ | Basic implementation |
| Pawn Expeditions (Sally) | ✅ | Full implementation with rewards |
| Golden Sally | ✅ | Premium expedition variant |
| Expedition Rewards | ✅ | Reward collection and drops |
| Official Pawns | ✅ | Legend/official pawn support |
| Free Rental Listings | ✅ | No-cost rentals available |
| Pawn Favorites | ✅ | Mark pawns as favorites |
| Pawn Likability/Gifts | ⚠️ | Partially implemented |
| Pawn Training | ⚠️ | Basic implementation |

---

## Party & Multiplayer

| Feature | Status | Notes |
|---------|--------|-------|
| Party Creation | ✅ | Create and manage parties |
| Party Invite | ✅ | Invite players |
| Party Chat | ✅ | Text and binary messaging |
| Quick Party | ✅ | Quest-based party matching |
| Party Scaling | ✔️ | Content scales with party size |
| Entry Board | ✅ | Party finder system |
| Cross-Channel Party | ⚠️ | Basic support |

---

## Social Features

| Feature | Status | Notes |
|---------|--------|-------|
| Friends List | ✅ | Add/remove/invite friends |
| Contact List | ✅ | Recent players |
| Blacklist | ✅ | Block players |
| Player Profiles | ✅ | View character profiles |
| Lobby Chat | ✅ | Global/area chat |
| Personal Mail | ✅ | Character-to-character messages |
| System Mail | ✅ | Server notifications with items |

---

## Clan / Guild System

| Feature | Status | Notes |
|---------|--------|-------|
| Clan Creation | ✅ | Create and name clans |
| Clan Ranks | ✅ | Hierarchy and permissions |
| Clan Members | ✅ | Invite, kick, leave |
| Clan Search | ✅ | Find clans |
| Clan Base | ✔️ | Instanced clan area |
| Clan Furniture | ✔️ | Furniture placement |
| Clan Shop | ✔️ | Buff items and functions |
| Clan Concierge | ✔️ | NPC management |
| Clan Scout | ⚠️ | Database ready, handlers stubbed |
| Clan Quests | ⚠️ | Limited implementation |
| Clan Extreme Missions | ❌ | Not implemented |

---

## Quest Content

### Main Story Quests (MSQ)

| Season | Status | Notes |
|--------|--------|-------|
| Season 1.0 | ✅ | Complete |
| Season 1.1 | ✅ | Complete |
| Season 1.2 | ✅ | Complete |
| Season 1.3 | ✅ | Complete |
| Season 2.0 | ✅ | Complete |
| Season 2.1 | ✅ | Complete |
| Season 2.2 | ✅ | Complete |
| Season 2.3 | ✅ | Complete |
| Season 3.0 | ✅ | Complete |
| Season 3.1 | ✅ | Complete |
| Season 3.2 | ⚠️ | Partial |
| Season 3.3 | ⚠️ | Partial |
| Season 3.4 | ❌ | Not started |
| Season 3.5 | ❌ | Not started |

### Other Quest Types

| Feature | Status | Notes |
|---------|--------|-------|
| World Quests | ✔️ | Most zones complete |
| Board Quests (S1-S2) | ✅ | Complete |
| Board Quests (S3) | ⚠️ | Partial |
| Light Quests | ✔️ | Daily/repeatable content |
| Personal Quests | ❌ | Not implemented |
| Substory Quests | ❌ | Not implemented |
| Seasonal Events | ⚠️ | Some events scripted |

### Missing World Quest Zones

- Urteca Mountains
- Memory of Megadosys
- Memory of Urteca

---

## Endgame Content

| Feature | Status | Notes |
|---------|--------|-------|
| Extreme Missions | ⚠️ | Limited implementation |
| Bitterblack Maze | ⚠️ | Basic dungeon, missing quests |
| BBM Treasure/Upgrades | ⚠️ | Basic implementation |
| BBM Sealing | ✔️ | Seal mechanics functional |
| Epitaph Road | ⚠️ | Dungeons exist, incomplete |
| Area Ranking | ✔️ | Competitive activities |
| Battle Content | ⚠️ | Arenas partially available |
| War Missions | ❌ | Not implemented |
| Grand Missions | ❌ | Not implemented |

---

## Daily / Weekly Systems

| Feature | Status | Notes |
|---------|--------|-------|
| Reward Missions | ✅ | 6 mission types with milestones |
| Daily Reset (5 AM JST) | ✅ | Automatic progress tracking |
| Milestone Rewards | ✅ | 3/5/8 mission completion rewards |
| Large Delivery Events | ✅ | Material delivery for rewards |
| Weekly Reset (Monday) | ✅ | Delivery event resets |
| Treasure Points | ✔️ | Map treasure locations |

---

## Dragon / Orb Systems

| Feature | Status | Notes |
|---------|--------|-------|
| Orb Devotion | ✔️ | Element devotion works |
| Pawn Orb Elements | ✔️ | Pawn-specific orbs |
| Dragon Ability Synthesis | ⚠️ | Basic implementation |
| White Dragon Powers | ⚠️ | Partial implementation |

---

## Premium Features

| Feature | Status | Notes |
|---------|--------|-------|
| GP Shop | ✅ | Full shop implementation |
| GP Courses | ✅ | Subscription bonuses |
| Gacha | ✅ | Box gacha system |
| Premium Storage | ✅ | Storage expansion items |
| Premium Pawns | ✅ | Official pawn support |

---

## Technical Features

| Feature | Status | Notes |
|---------|--------|-------|
| SQLite Database | ✅ | File-based persistence |
| PostgreSQL Database | ✅ | Scalable persistence |
| Multi-Channel | ⚠️ | RPC system for channel coordination |
| Scripting System | ✅ | Hot-reloadable C# scripts |
| Asset Configuration | ✅ | JSON-based game content |
| Database Migrations | ✅ | 63 migrations for feature tracking |

---

## Fork-Specific Improvements

These features are unique to this fork or significantly enhanced compared to upstream:

| Feature | Status |
|---------|--------|
| Gacha System | ✅ New |
| GP Shop Management | ✅ New |
| Pawn Expeditions | ✅ New |
| Mandragora Crafting | ✅ New |
| Quick Party Matching | ✅ New |
| Equipment Presets | ✅ New |
| Personal Mail System | ✅ New |
| Reward Mission System | ✅ New |
| Large Delivery Events | ✅ New |
| Event Code Redemption | ✅ New |
| Death Penalty Persistence | ✅ Fixed |
| Revival Timer Persistence | ✅ Fixed |
| GP Course System | ✅ Enhanced |
| Storage Expansion | ✅ Enhanced |

---

## Summary

| Category | Implemented | Partial | Not Started |
|----------|-------------|---------|-------------|
| Jobs/Vocations | 15 | 1 | 1 |
| Core Systems | 16 | 0 | 0 |
| Economy/Trading | 7 | 0 | 0 |
| Crafting | 9 | 2 | 0 |
| Pawn System | 12 | 2 | 0 |
| Party/Multiplayer | 6 | 1 | 0 |
| Social Features | 7 | 0 | 0 |
| Clan System | 8 | 2 | 1 |
| Quest Content | 13 | 6 | 4 |
| Endgame Content | 1 | 5 | 2 |
| Daily/Weekly | 5 | 1 | 0 |
| Dragon/Orb | 2 | 2 | 0 |
| Premium Features | 5 | 0 | 0 |

**Overall Estimation:** ~90% of core gameplay features implemented and playable.

---

*Last Updated: January 2026*
