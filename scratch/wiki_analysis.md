# DDON Server Project Wiki Analysis

**Source:** https://github.com/sebastian-heinz/Arrowgene.DragonsDogmaOnline/wiki
**Analysis Date:** 2026-01-25
**Repository:** Arrowgene.DragonsDogmaOnline (214 stars, 74 forks)

---

## Wiki Structure

The project wiki contains three main pages:

1. **Home** - Welcome page and landing (last edited January 15, 2022)
2. **What Works?** - Comprehensive feature status tracking
3. **Reverse engineering Packet Format(s)** - Technical documentation for packet analysis

---

## Feature Status Summary

### Jobs/Vocations

**Status: WORKING**

All 10 jobs are playable with working skills:
- Alchemist
- Element Archer
- Fighter
- High Scepter
- Hunter
- Priest
- Seeker
- Shield Sage
- Sorcerer
- Spirit Lancer
- Warrior

**Retail Behavior Note:** Skill progression is NOT retail-accurate. Job Masters and Season 2.x/3.x orb trees are not implemented.

---

## Systems (58 Total Features Tracked)

### WORKING Features (27 items)

| Feature | Status |
|---------|--------|
| Appraisal | Working |
| Arisen Profile | Working |
| Bazaar | Working |
| Beauty Salon | Working |
| Character Creation | Working |
| Crafting | Working |
| Combat | Working |
| Community List | Working |
| Equipment Color | Working |
| Equipment Crests | Working |
| Equipment Disassembly | Working |
| Equipment Enhancement | Working |
| Equipment Quality | Working |
| Inventory Management | Working |
| Job Points | Working |
| Leveling | Working |
| Login Bonus | Working |
| Moon Phases | Working |
| Pawn Rescue | Working |
| Play Points | Working |
| Player Death | Working |
| Recruitment Board | Working |
| Reward Box | Working |
| Revival Power | Working |
| Season 1.x/2.x Orb Trees | Working |
| Shops | Working |
| Storage | Working |
| Teleport | Working |
| Tutorial | Working |
| Wallet | Working |
| Weather | Working |

### PARTIALLY IMPLEMENTED Features (20 items)

| Feature | Status | Notes |
|---------|--------|-------|
| Achievements | Partial | - |
| Adventure Guide | Partial | - |
| Area Rank | Partial | - |
| Clan | Partial | - |
| Clan House | Partial | - |
| Contents Released | Partial | - |
| Enemy Drops | Partial | - |
| Enemy Spawn | Partial | - |
| Emblem | Partial | - |
| Gathering | Partial | - |
| GP Courses | Partial | - |
| Job Training | Partial | - |
| Learn Custom Skills | Partial | - |
| Learn Core Skills | Partial | - |
| My Room | Partial | - |
| Party | Partial | - |
| Pawns (assigned) | Partial | - |
| Pawns (hired) | Partial | - |
| Player Search | Partial | - |
| Quests | Partial | - |
| Season 3.x Orb Tree | Partial | - |
| Equipment Unlimit | Partial | - |

### IN DEVELOPMENT Features (2 items)

| Feature | Status |
|---------|--------|
| Equipment Presets | In Development |
| Gacha | In Development |

### NOT IMPLEMENTED Features (10 items)

| Feature | Status |
|---------|--------|
| Dragon Abilities | Not Implemented |
| Equipment Extreme Synthesis | Not Implemented |
| Large Delivery Event | Not Implemented |
| Mail (Player) | Not Implemented |
| Mandragora | Not Implemented |
| Premium Shop | Not Implemented |
| Quick Party | Not Implemented |
| Reward Mission | Not Implemented |
| Pawn Expeditions | Not Implemented |
| World Manage Quests | Not Implemented |

---

## Quest System Status (13 Categories)

### IMPLEMENTED Quests

- Clan Board Quests
- Personal Quests
- Season 1.x Main Story
- Season 2.x Main Story
- Season 1.x Board Quests
- Season 2.x Board Quests

### PARTIALLY IMPLEMENTED Quests

- Season 3.x Content
- World Quests
- Tutorial Quests
- Seasonal Events

### NOT IMPLEMENTED Quests

- Substory Quests
- Time Limited Quests
- Wild Hunt Progression Systems
- World Management Quests

---

## Endgame Content Status

### PARTIALLY IMPLEMENTED

| Content | Status | Notes |
|---------|--------|-------|
| Bitterblack Maze | Partial | - |
| Bonus Dungeon | Partial | - |
| Extreme Missions | Partial | Few quests available |
| Epitaph Road | Partial | - |
| Clan Extreme Missions | Partial | System exists, no quests |

### NOT IMPLEMENTED

| Content | Status |
|---------|--------|
| Grand Missions | Not Implemented |
| War Mission | Not Implemented |
| Rusted Weapons | Status Unknown |

---

## Technical Documentation: Packet Format Reverse Engineering

### Prerequisites for Development

- IDA Pro 7.5 with HexRays decompiler
- PS4 module loader plugin for IDA
- Unpacked PC client (v03.04.007)
- Debug PS4 client (v02020005)

### CPacket Structure Definition

```c
void *vft;
unsigned __int8 *data_start;
unsigned __int8 *data_end;
unsigned __int8 *read_offset;
unsigned __int8 *write_offset;
```

### Key PC Client Function Addresses

| Function | Address |
|----------|---------|
| ReadInt64 | 0x008BC660 |
| ReadByte | 0x008BC410 |

These functions handle endianness swapping and buffer offset management.

### Reverse Engineering Methodology

1. **Locate packet handlers** using existing documentation references
2. **Identify parser functions** within handler code
3. **Compare implementations** across PS4 debug and PC versions
4. **Transfer symbol names** where protocol versions align

### Protocol Evolution Notes

The documentation notes that packet structures evolved between versions. Example:
- PS4 version: `uint16` / `uint8` fields
- PC version: Both fields updated to `uint32`

This illustrates how network protocols change across game versions and why careful version comparison is necessary.

### Implementation Output

Decoded packet structures become entity serializer classes registered in the shared codebase's `EntitySerializer` component for network communication handling.

---

## Summary Statistics

| Category | Working | Partial | In Dev | Not Implemented |
|----------|---------|---------|--------|-----------------|
| Jobs | 10/10 | - | - | - |
| Systems | 27 | 20 | 2 | 10 |
| Quests | 6 cat. | 4 cat. | - | 4 cat. |
| Endgame | - | 5 | - | 3 |

**Overall Assessment:** The server emulator has substantial functionality with core gameplay systems working. Main gaps are in Season 3.x content, endgame missions, and some social/premium features.

---

## Retail Behavior Notes

1. **Skill Progression:** Not retail-accurate due to missing Job Masters and Season 2.x/3.x orb trees
2. **Protocol Differences:** Network packet structures differ between PS4 and PC client versions
3. **Season Content:** Season 1.x and 2.x content is more complete than Season 3.x

---

*This analysis was generated from the official project wiki pages.*
