# DDON Community Shop Research

This document compiles research on Dragon's Dogma Online's cash shop, premium services, and gacha systems from community resources.

## Sources

### Primary Documentation
- [The White Dragon Temple Wiki - Cash Shop](http://ddon.wikidot.com/cashshop:home)
- [The White Dragon Temple Wiki - Treasure Lots](http://ddon.wikidot.com/treasurelots:home)
- [DDON Paid Services (Tumblr)](https://julien-schu.tumblr.com/post/124222278341/ddon-paid-services)
- [DDON Masterpost](https://julien-schu.tumblr.com/ddon.html)
- [Archive.org - Official Shop Page](https://web.archive.org/web/20170711054053/https://members.dd-on.jp/shop/payment/stone/top)

### GitHub Resources
- [Arrowgene.DragonsDogmaOnline](https://github.com/sebastian-heinz/Arrowgene.DragonsDogmaOnline) - Main server emulator
- [DDOn-Tools](https://github.com/alborrajo/DDOn-Tools) - Server management tools

### Private Servers
- [LegacyDDON](https://legacyddon.com/) - North American private server
- DDON Discord Server - Community hub with 20,000+ members

---

## Premium Currency System

### Golden Gemstones (GG / 黄金石)
- **Exchange Rate**: 1 GG = 100 CAP = 100 yen (~$1 USD)
- **Purchase Methods**:
  - Credit card via Capcom Online Games website
  - Japanese e-cash services (BitCash, WebMoney)
  - E-cash prepaid cards (available at Play-Asia)
  - PlayStation Store (PS3/PS4 direct purchase)
- **Expiration**: Unused GG expires after 5 months from purchase date

### Silver Tickets
- **Acquisition**: Earned through daily login stamps (via NPC Ophelia)
- **Usage**: Alternative currency for Treasure Lots (30 tickets = 1 draw)
- **Daily Reset**: Clock resets at 5:00 AM JST

---

## Premium Courses

### Adventure Passport (冒険パスポート)
**Price**: 15 GG (1,500 yen) for 30 days | 90-day option available

**Benefits**:
- Unlimited green revival gemstones (vs. 3/day free)
- Expanded storage box access
- Fast pawn revival (50% shorter rescue time)
- 1.5x pawn crafting EXP
- Free inn stays
- Increased quest limit: 20 (vs. 10 free)
- Bazaar: 10 exhibits (vs. 5), no time limit
- Free teleport to favorite locations
- 50% discount on non-favorite teleports
- Double area support items from Area Master
- Material chest auto-accessible for crafting
- Gold storage chests in towns/field bases
- Bouken Points (BP) earned hourly for item exchange

### Growth Support Course (成長サポートコース)
**Price**: 4 GG for 3.5 hours | 8 GG for 1-hour x5 pack | 20 GG for 7-day instant

**Effects**:
- 2x EXP for defeating enemies (player and pawn)
- 2x EXP for completing board/world quests
- 1.5x Area Points from quests
- 2x Play Points (PP) from enemies

### Reward Support Course (報酬サポートコース)
**Price**: 4 GG for 3.5 hours | 8 GG for 1-hour x5 pack | 20 GG for 7-day instant

**Effects**:
- 2x enemy drop rate
- 2x gathering/mining item quantity
- 2x treasure box items
- 1.5x Blood Orb rewards
- Improved rare item odds on World Quest rewards
- 2x Rim rewards from quests

### Reliable Assist Course (安心アシストコース)
**Price**: Not specified in sources

**Effects**:
- 50% incoming damage reduction
- 125% stamina/endurance
- Reduced abnormal condition accumulation
- Pawn auto-rescue assist

### Grand Mission Course (グランドミッションコース)
**Price**: 6 GG for 72 hours

**Effects**:
- Increased mission scores
- Increased Job Points during Grand Missions only

### Double/Triple Activation Benefits
When multiple support courses are active simultaneously:
- **Double Activation**: 2.5x EXP (vs. 2x single)
- **Triple Activation**: 3x EXP + additional bonuses

### Storage Chest Passports
- **Storage Chest Passport 1**: 400 additional item slots
- **Storage Chest Passport 2/3**: Additional expansion tiers

### Internet Cafe Special Course (ネットカフェ特典コース)
- Combined benefits of multiple courses
- Available only at participating Japanese internet cafes

---

## Treasure Lots (Gacha System)

### Overview
- **NPC**: Cameron (キャメロン) at the White Dragon Temple
- **Cost**: 3 GG OR 30 Silver Tickets per single draw
- **Bulk Draw**: 10 GG for 11 draws (bonus draw included)

### Types of Treasure Lots
1. **Permanent Lots**: Always available
2. **Limited/Promotional Lots**: Time-limited events
3. **GG-Only Lots**: Cannot use Silver Tickets

### Item Ranks/Rarities
Items are typically categorized into three ranks:
- **Rank S (Class S)**: Rarest items
- **Rank A**: Mid-tier items
- **Rank B/Standard**: Common items

### Probability Display
- Rates were shown on official members site (members.dd-on.jp)
- Rounded to one decimal place
- Example: "Class S 100.0%" displayed when all items are same rarity

### Bonus System
- Purchase 3+ bulk draws (10 GG x3 = 30 GG) to receive bonus items
- Bonus items distributed after weekly maintenance

---

## Collaboration Events and Special Gacha

### Overlord Collaboration (Example)
**Event Period**: September 6-27, 2018 (2nd collaboration)

**Features**:
- Special Box Treasure Lot in cash shop
- Event currency: Pure Black Jade (純黒翡翠)
- 2x event item drop with collaboration gear equipped
- Higher drop rate with gear in both utility and cosmetic slots

**Rewards**:
- Collaboration furniture and gear
- Puppet Hamsuke furniture (7 missions complete)
- Tapestry - Overlord (3 missions complete)

### Other Known Collaborations
- Street Fighter x DDON costumes
- Calbee promotional event
- Christmas/Seasonal events

---

## Other Shop Services

### Beauty Salon
**Price**: 5 GG (500 yen) per customization session
- Modify Arisen and pawn appearances

### Pawn Voice Packs
**Price**: 9 GG (900 yen) each
- Voiced by popular Japanese voice actors
- Permanent purchase, reusable across multiple pawns
- Gender-locked (male voices for male pawns only)

### Craft Master Pawns Service
**Price**: 5 GG for 30 days
- Use high-quality Craft Master Pawns without RC cost
- Items crafted have improved quality

---

## Existing Implementation Status (Arrowgene)

### Implemented
1. **GpCourseManager** - Handles course effects system
   - Location: `Arrowgene.Ddon.GameServer/Characters/GpCourseManager.cs`
   - Supports multiple concurrent courses
   - Timer-based activation/deactivation
   - Effects include: EXP bonuses, PP bonuses, crafting bonuses, bazaar extensions

2. **GP Course Assets** - Configuration file with course definitions
   - Location: `Arrowgene.Ddon.Shared/Files/Assets/GpCourseInfo.json`
   - Defines 18 course types with effects
   - Uses archived official data from members.dd-on.jp

3. **Gacha System** - Basic treasure lot implementation
   - Handler: `Arrowgene.Ddon.GameServer/Handler/GachaBuyHandler.cs`
   - Asset: `Arrowgene.Ddon.Shared/Files/Assets/Gacha.json`
   - Supports GG and Silver Ticket payments
   - Random draw from weighted item pools

### Course Effects Defined
From `GpCourseInfo.json`, the following effect types exist:
- GP_COURSE_EFFECT_INFINITE_REVIVE
- GP_COURSE_EFFECT_STRAGE_EXTEND
- GP_COURSE_EFFECT_EXTRA_STRAGE_BOX
- GP_COURSE_EFFECT_FREE_MY_WARP_POINT
- GP_COURSE_EFFECT_WARP_DISCOUNT
- GP_COURSE_EFFECT_QUEST_ORDER_LIST_EXTEND
- GP_COURSE_EFFECT_BAZAAR_EXHIBIT_EXTEND
- GP_COURSE_EFFECT_BAZAAR_RE_EXHIBIT_SHORTEN
- GP_COURSE_EFFECT_AREA_MASTER_SUPPLY
- GP_COURSE_EFFECT_ENEMY_EXP_UP
- GP_COURSE_EFFECT_PAWN_ENEMY_EXP_UP
- GP_COURSE_EFFECT_WQ_REWARD_EXP_UP
- GP_COURSE_EFFECT_ENEMY_PP_UP
- GP_COURSE_EFFECT_PAWN_CRAFT_EXP_UP
- GP_COURSE_EFFECT_BLOOD_ORB_UP
- GP_COURSE_EFFECT_DISABLE_PARTY_ADJUST_ENEMY_EXP
- GP_COURSE_EFFECT_INCOMING_DAMAGE_CUT_OFF
- GP_COURSE_EFFECT_ENDURANCE_UP
- GP_COURSE_EFFECT_DEBUFF_DAMAGE_CUT_OFF
- GP_COURSE_EFFECT_PAWN_AUTO_RESCUE
- And many more (70+ unique effects)

### Predefined Courses in Arrowgene
| ID | Name | Japanese | Comment |
|----|------|----------|---------|
| 1 | Adventure Passport | 冒険パスポート | Basic premium features |
| 2 | Reward Support Course | 報酬サポートコース | Drop/reward bonuses |
| 3 | Growth Support Course | 成長サポートコース | EXP bonuses |
| 4 | Reliable Assist Course | 安心アシストコース | Damage reduction |
| 5 | Double Activation Benefit | ダブル発動特典 | Combined bonuses |
| 6 | Triple Activation Benefit | トリプル発動特典 | Combined bonuses |
| 7 | Grand Mission Course | グランドミッションコース | GM-specific bonuses |
| 8 | Internet Cafe Course | ネットカフェ特典コース | All benefits combined |
| 9-12 | Storage Chest Passports | 格納チェストパスポート | Storage expansion |
| 10 | 5x EXP Course | 討伐経験値5倍コース | Event EXP boost |
| 13-16 | PP Acquisition Courses | PP獲得量コース | PP multipliers |
| 15 | All Courses Free | 全コース無料開放 | Free promotion |
| 17 | Arrowgene Blessing | Custom | Server default |
| 18 | Bonus Exp Event | Custom | 3x EXP event |

---

## Implementation Recommendations

### Missing Features for Full Shop Implementation
1. **Shop UI Packet Handlers**: Handlers for browsing/purchasing courses
2. **Per-Character Course Tracking**: Database storage for purchased courses
3. **Course Duration Management**: Start/end time per character
4. **GG/Ticket Purchase System**: Real currency conversion (or admin grants)
5. **Login Stamp System**: Daily stamp tracking for Silver Ticket rewards
6. **Treasure Lot UI**: Full gacha interface with probability display
7. **Collaboration Event Framework**: Time-limited items and bonuses

### Data Requirements
- Item database with all cosmetic/event items
- Voice pack item definitions
- Collaboration gear with special effects
- Event currency mappings

---

## Archive Links

For historical reference, the official shop pages were archived:
- https://web.archive.org/web/20170711054053/https://members.dd-on.jp/shop/payment/stone/top
- https://web.archive.org/web/20170711035052/https://members.dd-on.jp/shop/payment/course/1
- https://web.archive.org/web/20170711035052/https://members.dd-on.jp/shop/payment/course/2

Note: The official members.dd-on.jp site is no longer accessible as the game shut down December 5, 2019.
