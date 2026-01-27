# Packet Structures Analysis

Analysis of packet structures in `Arrowgene.Ddon.Shared/Entity/`

## Summary

- **Total Packet Structures**: 1207 files in `PacketStructure/`
- **Total Data Structures**: 451 files in `Structure/`
- **Game Server Handlers**: 492 handlers implemented
- **Login Server Handlers**: 13 handlers implemented

## Packet Categories by Feature Area

### Client-to-Server (C2S) Packets by Feature

| Feature | Count | Description |
|---------|-------|-------------|
| Quest | 47 | Quest system, quests, rewards, progress |
| Skill | 36 | Skills, abilities, learning, presets |
| Pawn | 35 | Pawn creation, management, rental |
| Clan | 32 | Clan management, shop, members |
| Character | 24 | Character management, revive, edit |
| Instance | 21 | Instance management, enemies, gathering |
| Job | 20 | Job changes, orb tree, emblems |
| Craft | 19 | Crafting system, recipes |
| Season | 18 | Season dungeon content |
| Item | 17 | Item management, storage |
| EntryBoard | 15 | Party matching system |
| Party | 14 | Party management |
| Equip | 14 | Equipment management |
| Warp | 12 | Warp/teleport system |
| BattleContent | 12 | Bitterblack Maze and similar |
| Bazaar | 11 | Player marketplace |
| Area | 11 | Area/region management |
| Stage | 10 | Stage/dungeon entry |
| Mail | 9 | Mail system |
| MyRoom | 8 | Personal room/housing |
| Profile | 7 | Character profiles |
| Connection | 7 | Server connection |
| Server | 6 | Server info |
| PartnerPawn | 6 | Partner pawn system |
| Friend | 6 | Friend system |
| Achievement | 6 | Achievement system |
| Mandragora | 5 | Mandragora crafting |
| Inn | 5 | Inn/rest system |
| GP | 5 | Game Points (premium currency) |
| Dispel | 5 | Dispel/lottery system |
| Stamp | 4 | Stamp/login bonus |
| Recycle | 4 | Recycle/exchange system |
| Lobby | 4 | Lobby chat/data |
| Context | 4 | Player context sync |
| Ranking | 3 | Leaderboards |
| OrbDevote | 3 | Orb devotion system |
| Shop | 2 | NPC shop system |

### Server-to-Client (S2C) Packets by Feature

| Feature | Count | Description |
|---------|-------|-------------|
| Quest | 82 | Quest responses and notifications |
| Clan | 48 | Clan responses and notifications |
| Pawn | 47 | Pawn responses and notifications |
| Skill | 46 | Skill responses and notifications |
| Character | 36 | Character responses and notifications |
| Job | 35 | Job responses and notifications |
| Party | 32 | Party responses and notifications |
| Instance | 25 | Instance responses |
| EntryBoard | 25 | Entry board responses |
| SeasonDungeon | 23 | Season dungeon responses |
| Craft | 23 | Craft responses |
| Item | 22 | Item responses and notifications |
| Equip | 20 | Equip responses |
| BattleContent | 19 | Battle content responses |
| Area | 13 | Area responses |
| Stage | 12 | Stage responses |
| Context | 12 | Context sync |
| Connection | 12 | Connection management |
| Bazaar | 12 | Bazaar responses |
| Warp | 11 | Warp responses |
| Mail | 10 | Mail responses |
| Friend | 10 | Friend responses |
| MyRoom | 9 | MyRoom responses |
| Achievement | 8 | Achievement responses |
| Server | 7 | Server info responses |
| Profile | 7 | Profile responses |
| PartnerPawn | 7 | Partner pawn responses |
| GP | 7 | GP/course responses |
| OrbDevote | 6 | Orb devotion responses |
| Mandragora | 5 | Mandragora responses |
| Lobby | 5 | Lobby responses |
| Dispel | 5 | Dispel responses |
| Stamp | 4 | Stamp responses |
| Recycle | 4 | Recycle responses |
| Inn | 4 | Inn responses |
| Ranking | 3 | Ranking responses |
| Shop | 2 | Shop responses |

---

## Shop-Related Packets

### Standard NPC Shop

| Packet | Type | Description |
|--------|------|-------------|
| `C2SShopBuyShopGoodsReq` | Request | Buy goods from NPC shop |
| `C2SShopGetShopGoodsListReq` | Request | Get shop inventory |
| `S2CShopBuyShopGoodsRes` | Response | Buy confirmation |
| `S2CShopGetShopGoodsListRes` | Response | Shop inventory list |

### Bazaar (Player Marketplace)

| Packet | Type | Description | Handler |
|--------|------|-------------|---------|
| `C2SBazaarCancelReq` | Request | Cancel exhibition | Yes |
| `C2SBazaarExhibitReq` | Request | List item for sale | Yes |
| `C2SBazaarGetCharacterListReq` | Request | Get own listings | Yes |
| `C2SBazaarGetExhibitPossibleNumReq` | Request | Get max listing slots | Yes |
| `C2SBazaarGetItemHistoryInfoReq` | Request | Get price history | Yes |
| `C2SBazaarGetItemInfoReq` | Request | Get item details | Yes |
| `C2SBazaarGetItemListReq` | Request | Search marketplace | Yes |
| `C2SBazaarGetItemPriceLimitReq` | Request | Get price limits | Yes |
| `C2SBazaarProceedsReq` | Request | Buy item | Yes |
| `C2SBazaarReceiveProceedsReq` | Request | Collect gold | Yes |
| `C2SBazaarReExhibitReq` | Request | Relist item | Yes |

### GP (Game Points / Premium Currency)

| Packet | Type | Description | Handler |
|--------|------|-------------|---------|
| `C2LGpCourseGetInfoReq` | Request | Get GP course info (Login) | Yes |
| `C2SGpGetGpReq` | Request | Get current GP | Yes |
| `C2SGpGetGpDetailReq` | Request | Get GP details | Yes |
| `C2SGpGetGpPeriodReq` | Request | Get GP period | Yes |
| `C2SGpGetValidChatComGroupReq` | Request | Get valid chat groups | Yes |
| `C2SGpGpEditGetVoiceListReq` | Request | Get voice list | Yes |
| `S2CGpGetGpRes` | Response | GP amount response | - |
| `S2CGpGetGpDetailRes` | Response | GP details | - |
| `S2CGpGetGpPeriodRes` | Response | GP period | - |
| `S2CGPCourseStartNtc` | Notification | Course started | - |
| `S2CGPCourseExtendNtc` | Notification | Course extended | - |
| `S2CGpCourseEndNtc` | Notification | Course ended | - |
| `S2CGpGpCourseGetAvailableListRes` | Response | Available courses | - |

### Clan Shop

| Packet | Type | Description | Handler |
|--------|------|-------------|---------|
| `C2SClanClanShopBuyBuffItemReq` | Request | Buy clan buff | Yes |
| `C2SClanClanShopBuyFunctionItemReq` | Request | Buy clan function | Yes |
| `C2SClanClanShopGetBuffItemListReq` | Request | Get buff list | Yes |
| `C2SClanClanShopGetFunctionItemListReq` | Request | Get function list | Yes |

### Job Value Shop

| Packet | Type | Description | Handler |
|--------|------|-------------|---------|
| `C2SJobJobValueShopBuyItemReq` | Request | Buy with job points | Yes |
| `C2SJobJobValueShopGetLineupReq` | Request | Get job shop lineup | Yes |

### Character Edit Shop

| Packet | Type | Description | Handler |
|--------|------|-------------|---------|
| `C2SCharacterEditGetShopPriceReq` | Request | Get edit prices | Yes |

---

## Packets Without Handlers

The following C2S request packets appear to have no corresponding handler in `Arrowgene.Ddon.GameServer/Handler/`:

### Area/Stage

- `C2SAreaBuyAreaQuestHintReq` - Buy area quest hints
- `C2SAreaGetAreaReleaseListReq` - Get area release list
- `C2SAreaGetAreaRewardInfoReq` - Get area reward info

### Character/Binary

- `C2SBinarySaveSetCharacterBinSaveDataReq` - Save binary character data
- `C2SCertClientChallengeReq` - Client certificate challenge
- `C2SCharacterCreateModeCharacterEditParamReq` - Create mode edit params

### Craft

- `C2SCraftRecipeGetCraftGradeupRecipeReq` - Get gradeup recipes

### Dispel

- `C2SDispelGetLockSettingReq` - Get lock settings
- `C2SDispelLockSettingReq` - Set lock settings

### Entry Board

- `C2SEntryBoardEntryBoardItemInfoChangeReq` - Change item info
- `C2SEntryBoardEntryBoardItemRecreateReq` - Recreate entry
- `C2SEntryBoardEntryBoardItemReq` - Get entry item

### Friend

- `C2SFriendApplyFriendReq` - Apply for friend
- `C2SFriendApproveFriendReq` - Approve friend

### Inn

- `C2SInnStayPenaltyHealInnReq` - Stay at inn with penalty heal

### Item

- `C2SItemGetEmbodyPayCostReq` - Get embody cost
- `C2SItemSortSetItemSortDataBinReq` - Set item sort data

### Job

- `C2SJobOrbTreeGetJobOrbTreeGetAllJobOrbElementListReq` - Get all orb elements

### Lobby

- `C2SLobbyChatMsgReq` - Send lobby chat
- `C2SLobbyJoinReq` - Join lobby
- `C2SLobbyLeaveReq` - Leave lobby

### MyRoom

- `C2SMyRoomOtherRoomGetLayoutReq` - Get other room layout

### NPC

- `C2SNpcGetNpcExtendedFacilityReq` - Get NPC extended facility

### Party

- `C2SPartyPartyMemberSetValueReq` - Set party member value

### Pawn

- `C2SPawnGetMypawnDataReq` - Get my pawn data

### Quest

- `C2SQuestGetAdventureGuideQuestNtcReq` - Get adventure guide
- `C2SQuestGetQuestCompleteListReq` - Get complete list
- `C2SQuestPlayerStartReq` - Player start
- `C2SQuestQuestCancelReq` - Cancel quest
- `C2SQuestSetNavigationQuestReq` - Set navigation

### Season Dungeon

- `C2SSeasonDungeonGetSoulOrdealListfromOmReq` - Get soul ordeal list
- `C2SSeasonDungeonReceiveSoulOrdealRewardBuffReq` - Receive buff reward

### Server

- `C2SServerGameTimeGetBaseInfoReq` - Get game time base info

### Stamp

- `C2SStampBonusRecieveDailyReq` - Receive daily stamp
- `C2SStampBonusRecieveTotalReq` - Receive total stamp

---

## Incomplete/Unknown Packet Definitions

### Unnamed Packets (Numeric IDs)

These packets use numeric identifiers instead of proper names:

| Packet | Notes |
|--------|-------|
| `C2S_QUEST_11_60_16_NTC` | Unknown quest notification |
| `C2S_SEASON_62_40_16_NTC` | Unknown season notification |
| `C2S_SEASON_DUNGEON_62_12_16_NTC` | Season dungeon notification |
| `C2SContext_35_5_16_Ntc` | Context notification |
| `S2C_63_0_16_NTC` | Unknown notification |
| `S2C_63_7_16_NTC` | Unknown notification |
| `S2C_63_10_16_NTC` | Unknown notification |
| `S2C_63_11_16_NTC` | Unknown notification |
| `S2C_BATTLE_71_13_16_NTC` | Battle notification |
| `S2C_CHARACTER_2_5_16_NTC` | Character notification |
| `S2C_CHARACTER_2_33_16_NTC` | Character notification |
| `S2C_CHARACTER_2_34_16_NTC` | Character notification |
| `S2C_CHARACTER_2_35_16_NTC` | Character notification |
| `S2C_CHARACTER_2_36_16_NTC` | Character notification |
| `S2C_EQUIP_65_0_16_NTC` | Equip notification |
| `S2C_INSTANCE_13_36_16_NTC` | Instance notification |
| `S2C_SEASON_62_22_16_NTC` | Season notification |
| `S2C_SEASON_62_28_16_NTC` | Season notification |
| `S2C_SEASON_62_39_16_NTC` | Season notification |
| `S2CJob_33_3_16_Ntc` | Job notification |
| `S2CJob24_5_16_NTC` | Job notification |
| `S2CCraft_30_21_16_NTC` | Craft notification |
| `S2CQuest_11_125_16_Ntc` | Quest notification |

### Packets with Unknown ("Unk") Fields

These 116 files contain `Unk` fields indicating incomplete understanding:

**Battle Content:**
- `CDataBattleContentUnk4` - Multiple Unk fields
- `CDataBattleContentUnk5` - Completely unknown
- `CDataBattleContentUnk6` - Completely unknown
- `S2CBattleContentInstantClearInfoRes` - Uses unknown structures

**Season Dungeon:**
- `CDataSeasonDungeonUnk0` - Unknown structure
- `CDataSeasonDungeonUnk2` - Unknown structure

**Craft:**
- `CDataS2CCraftGetCraftSettingResUnk0Unk6` - Deeply nested unknowns
- `CDataS2CCraftStartQualityUpResUnk0` - Unknown quality up data
- `CDataCraftStartEquipGradeUpUnk0` - Unknown gradeup data

**Equip:**
- `CDataS2CEquipEnhancedGetPacksResUnk0Unk9` - Unknown enhance data
- `CDataS2CEquipEnhancedGetPacksResUnk0Unk10` - Unknown enhance data
- `CDataEquipEnhanceUnk0` - Unknown enhance field

**Context:**
- `CDataContextBaseUnk0` - Unknown context base

**Mandragora:**
- `CDataMyMandragoraUnk1Unk7` - Unknown mandragora data
- `CDataMyMandragoraUnk3` - Unknown mandragora data

---

## Data Structures Summary

### Shop-Related Structures

| Structure | Description |
|-----------|-------------|
| `CDataGoodsParam` | NPC shop item definition with price, stock, requirements |
| `CDataGoodsParamRequirement` | Shop item purchase requirements |
| `CDataBazaarItemInfo` | Bazaar listing with ID, base info, exhibition time |
| `CDataBazaarItemBaseInfo` | Base bazaar item info |
| `CDataBazaarItemHistoryInfo` | Price history data |
| `CDataBazaarCharacterInfo` | Character's bazaar listings |
| `CDataBazaarItemNumOfExhibitionInfo` | Exhibition count info |
| `CDataClanShopInfo` | Clan shop state (points, functions, buffs) |
| `CDataClanShopBuffInfo` | Clan buff information |
| `CDataClanShopBuffItem` | Purchasable clan buff |
| `CDataClanShopFunctionInfo` | Clan function information |
| `CDataClanShopFunctionItem` | Purchasable clan function |
| `CDataClanShopConciergeItem` | Clan concierge item |
| `CDataClanShopLineupName` | Clan shop lineup name |
| `CDataJobValueShopItem` | Job point shop item |
| `CDataCharacterEditPrice` | Character edit pricing |
| `CDataCharacterEditPriceInfo` | Edit price info |

### GP/Course Structures

| Structure | Description |
|-----------|-------------|
| `CDataGPCourseInfo` | GP course definition with ID, name, effects |
| `CDataGPCourseAvailable` | Available course info |
| `CDataGPCourseEffectParam` | Course effect parameters |
| `CDataGPCourseValid` | Course validity info |
| `CDataGPDetail` | Detailed GP information |
| `CDataGPPeriod` | GP period information |

### Structure Categories

| Category | Count | Examples |
|----------|-------|----------|
| Quest | 26 | Quest definitions, progress, rewards |
| Job | 26 | Job data, emblems, orbs |
| Clan | 22 | Clan members, shop, history |
| Pawn | 17 | Pawn info, skills, training |
| Mandragora | 17 | Housing/garden system |
| Character | 16 | Character info, stats, equip |
| Dispel | 14 | Lottery/gacha system |
| Craft | 14 | Crafting materials, recipes |
| Context | 11 | Network context sync |
| Stage | 10 | Stage/dungeon data |
| Item | 10 | Item lists, storage |
| BattleContent | 10 | Bitterblack/combat content |
| Area | 9 | Area info, ranks, spots |
| SeasonDungeon | 8 | Season dungeon data |
| Mail | 8 | Mail system |
| Equip | 8 | Equipment data |
| Entry | 8 | Party board system |
| Soul | 7 | Soul ordeal system |
| Party | 7 | Party data |
| Bazaar | 5 | Marketplace |
| Stamp | 5 | Login bonus |
| Reward | 5 | Reward data |
| Cycle | 5 | Cycle content |
| GP | 6 | Premium currency |
| Goods | 2 | Shop items |

---

## Recommendations

### High Priority - Missing Handlers

1. **Lobby System** - `C2SLobbyChatMsgReq`, `C2SLobbyJoinReq`, `C2SLobbyLeaveReq`
2. **Stamp Bonus** - `C2SStampBonusRecieveDailyReq`, `C2SStampBonusRecieveTotalReq`
3. **Friend System** - `C2SFriendApplyFriendReq`, `C2SFriendApproveFriendReq`
4. **Quest Cancel** - `C2SQuestQuestCancelReq`

### Medium Priority - Incomplete Definitions

1. **Battle Content** - Many `Unk` structures, needs reverse engineering
2. **Season Dungeon** - `CDataSeasonDungeonUnk0/2` need identification
3. **Craft Settings** - Nested unknown structures

### Low Priority - Unnamed Packets

1. Identify purpose of `S2C_63_*` packets
2. Identify purpose of `S2C_CHARACTER_2_*` packets
3. Rename `Unk` fields as functionality is discovered

---

## File Locations

- **Packet Structures**: `/Arrowgene.Ddon.Shared/Entity/PacketStructure/`
- **Data Structures**: `/Arrowgene.Ddon.Shared/Entity/Structure/`
- **Game Handlers**: `/Arrowgene.Ddon.GameServer/Handler/`
- **Login Handlers**: `/Arrowgene.Ddon.LoginServer/Handler/`
