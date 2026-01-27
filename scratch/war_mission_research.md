# Dragon's Dogma Online - War Mission Research

## Overview

War Mission (ウォーミッション) is a large-scale PvE content system introduced in Dragon's Dogma Online Season 3 "The Flames of a Fallen Kingdom" (亡国の炎). It replaced Grand Missions as the primary endgame 8-player cooperative content and was a core component of the kingdom restoration storyline.

## How War Missions Work

### Basic Structure
- **Party Size**: Maximum 8 players
- **Alternative Mode**: 4-player World Quest variant available (e.g., "Daclaim Fortress Recapture Battle")
- **Mission Types**: Time-limited instances with objective-based gameplay
- **Scheduling**: Missions ran in 7-day cycles, alternating with 7-day intervals between events

### Participation Requirements
- Complete main quest "The Prince's Whereabouts" (王子の行方)
- Minimum Item Rating (IR) requirement varies by mission:
  - Daclaim Fortress: IR 86+
  - Jifule Fortress: IR 96+
  - Misty Forest Battle: IR 106+
  - Resurrected Flame of Despair: IR 116-120+
  - Accershelm War Chronicles: IR 130+

### Registration NPCs
- **8-player War Mission**: NPC "Ashton" at Tines Fortress (near item box)
- **4-player WQ**: NPC "Ashton" at Daclaim Fortress (via Rim teleport)
- **Resurrected Flame**: NPC "Zachary" at Hotaru Mountain Camp
- **Accershelm War Chronicles**: NPC "Raymond" at Megado City Residential Area
- Alternative registration via "Lestania News" menu

## Content Structure

### Scenario Types

#### 1. Standard Scenario: Enemy Annihilation (敵軍殲滅)
- Primary objective: Eliminate enemy forces within time limit
- Points accumulate based on enemy kills
- Random chance to trigger extended "Commander Annihilation" phase

#### 2. Commander Annihilation (大将討滅)
- Triggered after defeating scenario boss
- 10-minute extension with multiplied rewards
- Defeating the commander grants massive Dominion Point bonus

#### 3. Special Scenario: Commander Assault
- Triggered when server-wide commander defeat counts reach thresholds
- All other scenarios pause during this period
- Limited-time bonus content

### Combat Mechanics

#### Fortress Recapture Objectives
1. Secure multiple infiltration routes into fortress
2. Destroy 5 enemy flags at conquest points
3. Defeat area leaders/commanders at each zone
4. Defend conquest points against counterattacks

#### Dynamic Elements
- Cannons placed throughout battlefield for tactical use
- Warp points connect different conquest areas for mobility
- NPC assistants can be strengthened with Dominion Points
- Battle gauge system tracks faction progress in real-time

#### Battle Gauge System
- Completing trials increases player faction gauge
- Failing trials or time expiration increases enemy gauge
- Mission fails if enemy gauge completely fills

## Rewards and Progression

### Dominion Points (DP) System

#### Earning Points
Enemy types yield varying DP amounts:
| Enemy Type | DP Value |
|------------|----------|
| Commander | 1,000 DP |
| Elite Units | 200 DP |
| Officers | 100 DP |
| Basic Infantry | 10 DP |

- Scenario bonuses start at 1,000 DP but decrease over time
- Death penalty reduces total by 5%

#### Multipliers
Golden Stone items enable DP multiplication:
- x2, x3, or x4 reward multipliers available
- Commander kills with x4 multiplier = up to 4,000 DP bonus

### DP Shop Rewards
Exchange DP at dedicated NPC shops for:
- Season-best equipment (highest tier for each update)
- Consumables and materials
- Exclusive items

#### Season Equipment Examples
| Season | Equipment Series | Description |
|--------|-----------------|-------------|
| 3.0 | Accalia Series | Season 3.0 strongest armor |
| 3.1 | Duurlz Series | Season 3.1 strongest gear |
| 3.3 | IR120 Magma-resistant armor | Season 3.3 top tier |

### Ranking Rewards
Performance-based rankings with exclusive rewards:

| Rank | Reward |
|------|--------|
| Top 10 | "Gloria Stole" (Physical ATK/Magic ATK +6) |
| Top 100 | "Ancient Mantle" equipment |
| Top 5,000 | "Hundred Battles Seals" (exchangeable for premium gear via NPC Gregoire) |

## Boss Encounters: The Four Demon Generals (魔軍四将軍)

### 1. Beast General (獣の将)
- Location: Daclaim Fortress
- Type: Dwarf Orc variant
- Abilities: Brute force attacks

### 2. Skeletal General (骸の将)
- Location: Jifule Fortress
- Type: Giant Dwarf Orc with massive hammer
- Abilities: Controls undead forces through domination
- Special: Cursed witch head suspended from back continuously chants curses

### 3. Darkness General (闇の将)
- Location: Misty Forest
- Signature Ability: "Otherworldly Descent" curse
- Mechanic: Afflicted characters dragged to another dimension within time limit
- Counter: Remove all curse sources to enable boss damage

### 4. War General (戦の将)
- Location: Observation Castle
- Type: Multi-weapon specialist
- Abilities: Axes with scorching flames, heat-based attacks
- Mechanic: Weaponizes heat for unpredictable combat

### Wicked Dragon (悪しき竜)
- Featured in "Resurrected Flame of Despair" mission
- High-level boss requiring coordinated team effort

## War Mission List

Based on documentation from the server emulator project:

| Mission Name (Japanese) | IR Requirement | Scenarios |
|------------------------|----------------|-----------|
| ダクレイム砦奪還戦 | IR 86 | Enemy/Commander Annihilation, Commander Rally |
| ジフール砦攻略戦 | IR 96 | Enemy/Commander Annihilation, Commander Rally |
| 霧の森の死闘 | IR 106 | Enemy/Commander Annihilation, Commander Rally |
| 蘇りし絶望の炎 | IR 116-120 | Mountain Summit, Wicked Dragon Rebirth, Restricted Stage |
| アッカーシェラン追懐戦記 | IR 130 | Multiple sub-quests (q90040000-q90040004) |

### Quest ID Structure
War Missions use unique quest ID ranges:
- q90030100-q90030105: Daclaim Fortress variants
- q90030200-q90030205: Jifule Fortress variants
- q90030300-q90030305: Misty Forest variants
- q90040000-q90040004: Accershelm War Chronicles

## Technical Implementation Notes

### Server Emulator Status
Based on the Arrowgene.DragonsDogmaOnline codebase:

#### Implemented Components
- `WalletType.DominionPoints` (value 10) - Currency tracking
- `NpcFunction.DominionPointShop` (value 70) - Shop functionality
- `TutorialId.DominionPointShop` (value 348) - Tutorial trigger
- `ItemId.DominionPoint0/1/2` (18643-18645) - Point item variants

#### Quest Structure
War Missions follow a multi-file quest structure:
- Base quest file (e.g., q90030100_00)
- Scenario variants (_01, _02, etc.)
- Enemy Annihilation variant (_04)
- Commander Annihilation variant (_05)

Example for Misty Forest:
```
q90030300_00: Management
q90030301_00: Main Event
q90030302_00: Ranking
q90030303_00: Reward Receipt
q90030304_00: Enemy Annihilation
q90030305_00: Commander Annihilation
```

### Not Yet Implemented
- Full War Mission handler system
- Dynamic scenario triggering
- Server-wide kill count tracking
- Ranking system
- Time-limited event scheduling

## Sources

### Primary Japanese Sources
- [DDON Beginner's Diary - 8 Points to Remember for War Mission](https://ddon38.jp/blog-entry-401.html)
- [PlayStation Blog Japan - Battle in the Misty Forest (2018/06/14)](https://blog.ja.playstation.com/2018/06/14/20180614-ddon/)
- [PlayStation Blog Japan - Season 3.1 War Mission (2017/12/07)](https://blog.ja.playstation.com/2017/12/07/20171207-ddon/)
- [PlayStation Blog Japan - Season 3.2 New Features (2018/03/29)](https://blog.ja.playstation.com/2018/03/29/20180329-ddon/)
- [Dengeki Online - Accalia Equipment from War Mission](https://dengekionline.com/elem/000/001/569/1569931/)
- [4Gamer - Resurrected Flame of Despair (2018/10/11)](https://www.4gamer.net/games/289/G028968/20181011048/)
- [Game Exploration Blog - DDON War Mission Overview](https://browsegames.net/ddon/ddon-wm)

### English Sources
- [Dragon's Dogma Online Guide Wiki - Grand Missions](https://dragons-dogma-online-guide.fandom.com/wiki/Grand_Missions)
- [The White Dragon Temple - Extreme Missions](http://ddon.wikidot.com/exm:home)
- [Dragon's Dogma Wiki - Dragon's Dogma Online](https://dragonsdogma.fandom.com/wiki/Dragon's_Dogma_Online)
- [Wikipedia - Dragon's Dogma Online](https://en.wikipedia.org/wiki/Dragon's_Dogma_Online)

### Technical Reference
- [Arrowgene.DragonsDogmaOnline GitHub Repository](https://github.com/sebastian-heinz/Arrowgene.DragonsDogmaOnline)
- Internal codebase documentation: `/docs/quests/endgame_content.md`

---

*Note: Dragon's Dogma Online shut down on December 5, 2019. This documentation is based on archived sources and the server emulator project.*
