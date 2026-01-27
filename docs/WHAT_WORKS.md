# What Works?

This document tracks feature implementation status for this fork of the Dragon's Dogma Online server emulator. Based on the [upstream wiki](https://github.com/sebastian-heinz/Arrowgene.DragonsDogmaOnline/wiki/What-Works%3F) with updates for fork-specific improvements.

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

All 11 jobs are playable with all skills functional, though progression lacks retail accuracy due to missing Season 2.x/3.x orb tree completion.

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

---

## Systems

| Feature | Status | Notes |
|---------|--------|-------|
| Achievements | ⚠️ | Includes furniture recipe rewards |
| Adventure Guide | ⚠️ | Partial implementation |
| Appraisal | ✔️ | Functional |
| Area Rank | ⚠️ | Season 1 only; S2/S3 unlocked by default |
| Arisen Profile | ✅ | Retail accurate |
| Bazaar | ✅ | Retail accurate |
| Beauty Salon | ✔️ | Free (no currency cost) |
| Caution Spot Enemies | ⚠️ | Season 1 only |
| Character Creation | ✔️ | Functional |
| Clan | ⚠️ | Partial |
| Clan House | ⚠️ | Partial |
| Combat | ✔️ | Occasional party desync |
| Community List (Friends) | ✔️ | Functional |
| Contents Released (Unlocks) | ⚠️ | Reflects endgame player state |
| Crafting | ✔️ | Some recipes unknown; instant crafting available |
| Dragon Abilities | ❌ | Unimplemented |
| Emblem | ⚠️ | Partial |
| Enemy Drops | ⚠️ | Partial |
| Enemy Spawn | ⚠️ | Partial |
| Equipment Color | ✔️ | Functional |
| Equipment Crests | ✔️ | Functional |
| Equipment Disassembly | ✔️ | All types supported |
| Equipment Enhancement | ✔️ | Functional |
| Equipment Extreme Synthesis | ❌ | Unimplemented |
| Equipment Presets | ✅ | **Fork: Implemented** - Save/load configurations |
| Equipment Quality | ✔️ | Functional |
| Equipment Unlimit | ⚠️ | Partial |
| Gacha | ✅ | **Fork: Implemented** - Box gacha with draws/resets |
| Gathering | ⚠️ | Limited locations; missing weather/time mechanics |
| GP Courses | ⚠️ | Server-wide buff instead of per-player |
| GP Shop | ✅ | **Fork: Implemented** - Full shop with purchase tracking |
| Inventory Management | ✔️ | All slots unlocked at start |
| Job Masters | ✔️ | Functional |
| Job Points | ✔️ | Functional |
| Job Training | ⚠️ | Needs refinement but playable |
| Large Delivery Event | ✅ | **Fork: Implemented** - Material delivery for rewards |
| Learn Augment | ✔️ | Functional |
| Learn Core Skill | ⚠️ | Season 3.x tree incomplete |
| Learn Custom Skill | ⚠️ | Season 3.x tree incomplete |
| Lestania News | ✔️ | Functional |
| Leveling | ✔️ | Functional |
| Login Bonus | ✔️ | Functional |
| Mail (Player) | ✅ | **Fork: Implemented** - Character-to-character messages |
| Mail (System) | ✔️ | Functional |
| Mandragora | ✅ | **Fork: Implemented** - Special pawn-based crafting |
| Moon Phases | ✔️ | Functional |
| My Room | ⚠️ | Furniture placement/visiting partial |
| Party | ⚠️ | Partial |
| Pawns (partner) | ⚠️ | Assignment/likability work; limited rewards |
| Pawns (player) | ✔️ | Functional |
| Pawns (hired) | ⚠️ | Partial |
| Pawn Rescue | ✔️ | Dungeon scales by owner level |
| Play Points | ✔️ | Functional |
| Player Death | ✅ | **Fork: Fixed** - Weakness persists across relog/channel change |
| Player Search | ⚠️ | Ignores criteria; returns all players |
| Premium Shop | ❌ | Unimplemented |
| Quests | ⚠️ | Partial |
| Quests in Party | ⚠️ | Partial |
| Quick Party | ✅ | **Fork: Implemented** - Quest-based party matching |
| Recruitment Board | ✔️ | Functional |
| Reward Box | ✔️ | All clears count as first clear |
| Reward Mission | ✅ | **Fork: Implemented** - 6 mission types with milestones |
| Revival Power | ✅ | **Fork: Fixed** - Timer persists in database |
| Season 1.x Orb Tree | ✔️ | Some upgrades non-functional |
| Season 2.x Orb Tree | ✔️ | Functional |
| Season 3.x Orb Tree | ⚠️ | Seasons 3.0/3.1 only |
| Shops | ✔️ | Functional |
| Storage | ✔️ | Functional |
| Teleport (Rift Crystals) | ✔️ | Functional |
| Tutorial (Menu) | ✅ | Retail accurate; needs translation |
| Wallet | ✅ | Retail accurate |
| Weather | ✔️ | Functional |

---

## Quests

| Category | Status | Notes |
|----------|--------|-------|
| Season 1.x Main Story | ✔️ | Functional |
| Season 2.x Main Story | ✔️ | Functional |
| Season 3.x Main Story | ⚠️ | 3.0/3.1 only |
| Season 1.x World Quests | ⚠️ | Partial |
| Season 2.x World Quests | ⚠️ | Partial |
| Season 3.x World Quests | ⚠️ | 2 areas implemented |
| Season 1.x Board Quests | ✔️ | Rewards not retail-accurate |
| Season 2.x Board Quests | ✔️ | Rewards not retail-accurate |
| Season 3.x Board Quests | ⚠️ | 3.0/3.1 only |
| Clan Board Quests | ✔️ | Functional |
| Pawn Expeditions | ✅ | **Fork: Implemented** - Full sally system with rewards |
| Personal Quests | ✔️ | Functional |
| Seasonal Events | ⚠️ | Implemented as personal quests |
| Substory Quests | ❌ | Unimplemented |
| Time Limited Quests | ❔ | Status unknown |
| Tutorial Quests | ⚠️ | Some require relog |
| Wild Hunt Quests | 🚧 | Can complete; unique systems missing |
| World Manage Quests | ❌ | Unimplemented |

---

## Endgame Content

| Feature | Status | Notes |
|---------|--------|-------|
| Bitterblack Maze | ⚠️ | Tracker issue #477 |
| Bonus Dungeon | ⚠️ | Dungeon scaling unimplemented |
| Clan Extreme Missions | ❌ | System functional; none implemented |
| Epitaph Road | ⚠️ | Tracker issue #683 |
| Extreme Missions | ⚠️ | Handful of quests implemented |
| Grand Missions | ❌ | Replaced by EXM |
| Rusted Weapons | ❔ | Status unknown |
| War Mission | ❌ | Unimplemented |

---

## Fork-Specific Improvements

These features have been implemented or significantly improved in this fork compared to upstream:

| Feature | Upstream Status | Fork Status | Notes |
|---------|-----------------|-------------|-------|
| Equipment Presets | 🚧 In Dev | ✅ | Save/load equipment configurations |
| Gacha | 🚧 In Dev | ✅ | Box gacha with draws, resets, prize pools |
| GP Shop | ❌ | ✅ | Full shop with purchase tracking |
| Large Delivery Event | ❌ | ✅ | Material delivery system with weekly reset |
| Mail (Player) | ❌ | ✅ | Character-to-character messaging |
| Mandragora | ❌ | ✅ | Special pawn-based crafting + golden variant |
| Pawn Expeditions | ❌ | ✅ | Full sally system with rewards |
| Quick Party | ❌ | ✅ | Quest-based party matching |
| Reward Mission | ❌ | ✅ | 6 mission types with 3/5/8 milestones |
| Player Death | ✔️ (bug) | ✅ | Weakness now persists across relog |
| Revival Power | ✅ | ✅ | Timer now persists in database |
| Event Codes | ❌ | ✅ | Redeemable promotional codes |
| NPC Bazaar | ❌ | ✅ | NPC vendor selling items for gold |
| Official Pawns | ⚠️ | ✅ | Legend/official pawn support |
| Storage Expansion | ⚠️ | ✅ | Purchasable via GP Shop |

---

## Summary

Based on upstream wiki with fork improvements applied:

- **Core Systems**: Most functional with approximations
- **Fork Additions**: 15 features implemented or significantly enhanced
- **Major Gaps**: Dragon Abilities, Premium Shop, Clan EXM, War/Grand Missions, Substory Quests

---

*Last Updated: January 2026*
*Based on upstream wiki with fork-specific updates*
