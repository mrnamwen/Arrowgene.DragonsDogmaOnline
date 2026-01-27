# DDON Wiki Research - Feature Documentation

**Source:** The White Dragon Temple (ddon.wikidot.com)
**Research Date:** 2025-01-25

---

## 1. Extreme Missions (エクストリームミッション)

### Overview
- High-difficulty 4-player party quests
- Time-limited missions to defeat enemies
- Rewards include equipment and Custom Skills
- Can be soloed but designed for 4 players
- Repeatable with daily rare item limit

### Participation Rules
- Cannot use personal consumables during EXM
- Given mission-only items for duration
- Must complete EXMs in order (sequential unlock)

### Requirements
- Complete MSQ: "Be Forevermore, White Dragon" (白竜よ永遠なれ)
- Complete Personal Quest: "The Dragon Force's Whirl" (竜力の渦)

### Complete EXM List

| No. | Japanese Name | English Name | Rec. Level |
|-----|--------------|--------------|------------|
| 1 | 地下墓場の誘い | The Call of the Catacombs | 58 |
| 2 | 古き力に魅かれし者 | Drawn to Ancient Power | 60 |
| 3 | 古都の賜物 | The Ancient City's Legacy | 60 |
| 4 | 輝く扉 | The Shining Gate | 60 |
| 5 | 歪みの執行人 | Agent of Corruption | 65 |
| 6 | 淀みし大竜力 | Phantasmic Great Dragon | 70 |

### Current Implementation Status
- EXM4 "The Shining Gate" is scripted
- 3 total scripts exist in codebase
- Need to implement EXM1, EXM2, EXM3, EXM5, EXM6

---

## 2. Pawn Expedition (ポーン遠征隊)

### Overview
- Introduced in Season 2.2
- Send Partner Pawns on expeditions from Clan Hall
- Requires Clan Level 3 (Clan Hall unlocked)

### Mechanics
- Available once per day per pawn
- Expedition duration: 4 hours (normal), 2 hours (golden)
- Pawn returns with random materials and monster drops
- Requires 6 empty inventory slots to collect rewards

### Hot Spots System
- Random chance to discover "hot spots" in areas
- Hot spots persist for one day (until reset)
- Special items available from hot spot expeditions
- 3 GG guarantees success at hot spot

### Facility Requirements
- Pawn Expedition Facility costs 2,000 Clan Points (CP)
- Support options available via Clan Hall upgrades
- 1 GG to resend cancelled expedition

### Reset Time
- Daily reset at 5:00 AM JST

### Current Implementation Status
- PawnExpeditionManager: NEW (35% complete)
- 9 handlers implemented
- Database schema added (migration)
- Hot spot system needs completion
- Reward generation partially implemented

---

## 3. Mandragora Cultivation (マンドラゴラ育成)

### Overview
- Introduced in Season 3.0
- Raise Mandragora creatures in Arisen's Room
- Unlocked via Personal Quests

### Unlock Requirements
1. Complete "Extend Garden" (ガーデン・エクステンド)
2. Complete "Strange Creature Investigation" (奇妙な魔物探索)

### Cultivation Mechanics
- 18 real-time hours to grow
- 1 GG to complete immediately
- Up to 20 materials as "fertilizer"
- Requires Soft Soil (ふかふかの土) for growth
- Adventure Passport: 2 Mandragora instead of 1

### Craftable Items
- Bonus Dungeon Ticket (XP/Rift/BO/Gold variants)
- Quality Attack Upgrade Rock (良質攻錬石)
- Quality Defence Upgrade Rock (上質護錬石)
- Mandragora Leaf (マンドラゴラの葉っぱ)
- Mandragora Twig (マンドラゴラの小枝)
- Mandragora Mushroom (マンドラゴラキノコ)

### Cultivation Hints
- Medicines/consumables boost growth
- Mushrooms + ores are important
- Growth stages matter (seed → leaves → flower)
- Meat, hide, greens make Mandragora grow bigger
- Higher quality materials = better results
- Material type bias affects species outcome
- Battlefield bones → flowering Mandragora

### Species Discovery
- New species registered to player Arisen
- First discoverer listed in Mandragora Dictionary
- Dictionary shows all discovered species

### Current Implementation Status
- MandragoraManager: 30% complete
- 4 handlers implemented
- Asset file created (Mandragora.json)
- 12 species categories defined
- Species discovery tracking: partial
- Crafting system: partial

---

## 4. Grand Missions (グランドミッション)

### Overview
- 8-player raid content (NO pawns allowed)
- Instanced, timed score-based missions
- Higher scores = better rewards
- Scheduled time windows (typically weekends)

### Registration Flow
1. Register via Lestania News or NPCs (Zelkin, Alan)
2. Select "Mission Entry" and battle status
3. Use Entry Board for party formation
4. 8 players gather and select "Ready"
5. Teleport to White Dragon Temple harbor
6. All 8 select "Sortie" to begin

### Combat Mechanics
- Bosses enter rage state with stamina bars
- Support vocations expose weak points
- Season 2+ Break Mechanic: blue meter during rage

### Known Grand Missions

**Season 1:**
- Gritten Fortress Siege (グリッテン砦攻防戦)
- Crucible of Monsters (魔物のるつぼ)
- Ancient Strong Ones (太古の強者)

**Season 2:**
- Dazzling Gold (眩しき黄金) - Golgoran boss
- Lost Order (失われた秩序) - Zuhl boss
- Battle at Ancient Temple (古代神殿の死闘)
- Awakened Dark Monsters (目覚めし闘の魔物)
- The Demon of Darkness Awakens

**Note:** Grand Missions removed in Season 3, merged into EXMs

### Current Implementation Status
- NOT IMPLEMENTED
- Would require:
  - 8-player instancing system
  - Score tracking system
  - Time window scheduling
  - Entry board enhancements
  - New quest type handlers

---

## 5. War Missions (ウォーミッション)

### Overview
- Introduced in Season 3 "The Flames of a Fallen Kingdom"
- Replaced Grand Missions as primary endgame
- 8-player cooperative PvE content
- 4-player variants available

### Objective Types
1. Enemy Annihilation (敵軍殲滅)
2. Commander Annihilation (大将討滅) - 10-minute extended boss
3. Fortress Recapture - secure routes, destroy flags

### Dominion Points (DP) System
- Currency earned based on combat performance
- Commanders: 1,000 DP, Elites: 200 DP, Officers: 100 DP, Infantry: 10 DP
- Multipliers (x2-x4) via Golden Stone items
- Death penalty: -5% total DP

### Four Demon Generals (Bosses)
- Beast General
- Skeletal General (undead control)
- Darkness General (curse abilities)
- War General (heat attacks)

### Rewards
- Season-best equipment (Accalia, Duurlz series)
- Ranking rewards (Top 10/100/5000)
- Exclusive DP shop items

### Current Implementation Status
- NOT IMPLEMENTED
- WalletType.DominionPoints exists
- NpcFunction.DominionPointShop defined
- Quest ID structure documented
- No handlers or manager

---

## 6. Quick Party (クイックパーティ)

### Overview
- Matchmaking system for content
- Find other players seeking same content/quest
- 2-4 member parties
- Registration timeout: 30 minutes
- Confirmation timeout: 60 seconds

### Current Implementation Status
- QuickPartyManager: 40% complete
- 3 handlers implemented
- Matching algorithm: partial
- Notifications: partial

---

## Sources

- http://ddon.wikidot.com/exm:home
- http://ddon.wikidot.com/pawns:pawnexpedition
- http://ddon.wikidot.com/mandragora:home
- Community research on Grand/War Missions
