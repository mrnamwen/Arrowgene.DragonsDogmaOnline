# DDON Shop/Gacha Packet Format Research

This document contains research findings on packet formats for DDON shop and gacha systems.

## Summary

| System | Group ID | Implementation Status |
|--------|----------|----------------------|
| GP Shop | 28 | Partial (IDs 7, 15 implemented, others pending) |
| Gacha | 39 | Implemented |
| Box Gacha | 57 | Partial (list, buy, draw info implemented) |

---

## Group 28: GP (Golden Premium) System

The GP group handles the premium currency shop, courses, and related features.

### Implemented Handlers

| ID | Packet Name | Handler | Description |
|----|-------------|---------|-------------|
| 0 | GP_GET_GP | GpGetGpHandler | Get GP balance |
| 1 | GP_GET_GP_DETAIL | GpGetGpDetailHandler | Get detailed GP info |
| 2 | GP_GET_GP_PERIOD | GpGetGpPeriodHandler | Get GP period info |
| 3 | GP_GET_CAP | GpGetCapHandler | Get CAP (Capcom Points) balance |
| 4 | GP_GET_CAP_TO_GP_CHANGE_LIST | GpGetCapToGpChangeListHandler | Get CAP to GP conversion rates |
| 5 | GP_CHANGE_CAP_TO_GP | GpChangeCapToGpHandler | Convert CAP to GP |
| 6 | GP_COG_GET_ID | GpCogGetIdHandler | Get COG ID |
| 7 | GP_GP_SHOP_DISPLAY_GET_TYPE | GpShopDisplayTypeHandler | Get shop category list |
| 15 | GP_GP_SHOP_GET_BUY_HISTORY | GpShopGetBuyHistoryHandler | Get purchase history |
| 17 | GP_GP_COURSE_GET_AVAILABLE_LIST | GpGpCourseGetAvailableListHandler | Get available courses |
| 18 | GP_GP_COURSE_GET_VALID_LIST | GpCourseGetValidListHandler | Get active courses |
| 19 | GP_GP_COURSE_USE_FROM_AVAILABLE | GpCourseUseFromAvailableHandler | Activate a course |
| 22 | GP_GP_EDIT_GET_VOICE_LIST | GpGpEditGetVoiceListHandler | Get voice options |
| 33 | GP_GET_VALID_CHAT_COM_GROUP | GpGetValidChatComGroupHandler | Get purchased chat commands |
| 34 | GP_GET_UPDATE_APP_COURSE_BONUS_FLAG | GpGetUpdateAppCourseBonusFlagHandler | Check if course bonus update needed |

### Unimplemented/Stub GP Shop Packets (IDs 7-23)

| ID | Packet Name | Japanese Comment | Purpose |
|----|-------------|------------------|---------|
| 8 | GP_GP_SHOP_DISPLAY_GET_LINEUP | Shop lineup list | Get shop item list for a category |
| 9 | GP_GP_SHOP_DISPLAY_BUY | Shop purchase result | Purchase item response |
| 10 | GP_GP_SHOP_GET_COURSE_LINEUP | Course lineup | Get purchasable courses list |
| 11 | GP_GP_SHOP_GET_ITEM_LINEUP | Item lineup | Get purchasable items list |
| 12 | GP_GP_SHOP_GET_PAWN_LINEUP | Legend pawn lineup | Get legend pawns for purchase |
| 13 | GP_28_13 | (Unknown) | Unknown purpose |
| 14 | GP_28_14 | (Unknown) | Unknown purpose |
| 16 | GP_GP_SHOP_GET_CAP_CHARGE_URL | CAP charge URL | Get URL for charging CAP |
| 20 | GP_GP_COURSE_GET_VERSION | Course version | Get course version info |
| 21 | GP_28_21 | (Unknown) | Unknown purpose |
| 23 | GP_GP_SHOP_CAN_BUY | Can buy check | Check if item is purchasable |

### GP Data Structures

#### CDataGPShopDisplayType
```csharp
public uint ID { get; set; }           // Category ID
public string Name { get; set; }       // Display name
public uint InGameUrlID { get; set; }  // In-game URL reference
```

#### CDataGPCourseInfo
```csharp
public uint CourseId { get; set; }
public string CourseName { get; set; }
public bool DoubleCourseTarget { get; set; }
public byte PrioGroup { get; set; }
public byte PrioSameTime { get; set; }
public byte AnnounceType { get; set; }
public List<uint> EffectUIDs { get; set; }
```

#### CDataGPCourseAvailable
```csharp
public uint ID { get; set; }
public string CourseName { get; set; }
public ulong UseLimitTime { get; set; }
public uint CourseID { get; set; }
public uint LineupID { get; set; }
public uint BackIconID { get; set; }
public uint FrameIconID { get; set; }
```

#### CDataGPDetail
```csharp
public uint GP { get; set; }
public uint Max { get; set; }
public bool isFree { get; set; }
public GPDetailType Type { get; set; }
public DateTimeOffset Expire { get; set; }
public DateTimeOffset Created { get; set; }
```

#### CDataGPShopBuyHistoryElement (inferred)
```csharp
public uint ID { get; set; }
public string Name { get; set; }
public uint Price { get; set; }
public ulong AcquisitionTime { get; set; }
```

### Recommended New Structures for GP Shop

#### CDataGPShopLineupItem (proposed)
Based on similar structures and DTI analysis:
```csharp
public uint LineupID { get; set; }      // u32 - unique item ID
public uint ItemID { get; set; }        // u32 - game item ID
public string Name { get; set; }        // MtString
public string Description { get; set; } // MtString
public uint Price { get; set; }         // u32 - GP cost
public uint BasePrice { get; set; }     // u32 - original price
public uint ItemNum { get; set; }       // u32 - quantity
public uint PurchaseLimit { get; set; } // u32 - max purchases
public uint PurchaseCount { get; set; } // u32 - current purchases
public long BeginTime { get; set; }     // s64 - sale start time
public long EndTime { get; set; }       // s64 - sale end time
public uint CategoryID { get; set; }    // u32 - shop category
public uint IconID { get; set; }        // u32 - icon reference
```

---

## Group 39: Gacha System

Standard lottery/gacha system using GP or Silver Tickets.

### Packet List

| ID | SubID | Packet Name | Direction | Description |
|----|-------|-------------|-----------|-------------|
| 0 | 1 | C2S_GACHA_GACHA_LIST_REQ | C2S | Request gacha list |
| 0 | 2 | S2C_GACHA_GACHA_LIST_RES | S2C | Return gacha list |
| 1 | 1 | C2S_GACHA_GACHA_BUY_REQ | C2S | Purchase gacha draw |
| 1 | 2 | S2C_GACHA_GACHA_BUY_RES | S2C | Return draw results |

### Request Structures

#### C2SGachaListReq
```csharp
// Empty request - no parameters
```

#### C2SGachaBuyReq
```csharp
public uint GachaId { get; set; }       // u32 - target gacha ID
public uint DrawGroupId { get; set; }   // u32 - draw group within gacha
public uint SettlementId { get; set; }  // u32 - payment method (1=GP, 2=SilverTicket)
public uint Price { get; set; }         // u32 - cost
```

### Response Structures

#### S2CGachaListRes
```csharp
public List<CDataGachaInfo> GachaList { get; set; }
```

#### S2CGachaBuyRes
```csharp
public uint GachaId { get; set; }
public List<CDataGachaItemInfo> GachaItemList { get; set; }
```

### Data Structures

#### CDataGachaInfo
```csharp
public uint Id { get; set; }                                // u32
public long Begin { get; set; }                             // s64 - start time
public long End { get; set; }                               // s64 - end time
public string Name { get; set; }                            // MtString
public string Description { get; set; }                     // MtString
public string Detail { get; set; }                          // MtString
public byte WeightDispType { get; set; }                    // u8
public string WeightDispTitle { get; set; }                 // MtString
public string WeightDispText { get; set; }                  // MtString
public string ListAddr { get; set; }                        // MtString - thumbnail URL
public string ImageAddr { get; set; }                       // MtString - banner URL
public List<CDataGachaDrawGroupInfo> DrawGroups { get; set; }
```

#### CDataGachaDrawGroupInfo
```csharp
public List<CDataGachaSettlementInfo> GachaSettlementList { get; set; }
public List<CDataGachaDrawInfo> GachaDrawList { get; set; }
```

#### CDataGachaSettlementInfo
```csharp
public uint DrawGroupId { get; set; }       // u32
public uint Id { get; set; }                // u32 - settlement type
public uint Price { get; set; }             // u32
public uint BasePrice { get; set; }         // u32
public uint PurchaseNum { get; set; }       // u32
public uint PurchaseMaxNum { get; set; }    // u32
public uint SpecialPriceNum { get; set; }   // u32
public uint SpecialPriceMaxNum { get; set; } // u32
public uint Unk1 { get; set; }              // u32
```

#### CDataGachaDrawInfo
```csharp
public uint Num { get; set; }                               // u32 - number of draws
public bool IsBonus { get; set; }                           // bool
public List<CDataGachaItemInfo> GachaItemInfo { get; set; }
```

#### CDataGachaItemInfo
```csharp
public uint ItemId { get; set; }        // u32
public uint ItemNum { get; set; }       // u32
public uint Rank { get; set; }          // u32 - rarity tier
public uint Effect { get; set; }        // u32
public double Probability { get; set; } // f64 - drop rate
```

---

## Group 57: Box Gacha System

Box gacha is a special lottery where items are removed from the pool after being won.

### Packet List

| ID | SubID | Packet Name | Direction | Description |
|----|-------|-------------|-----------|-------------|
| 0 | 1 | C2S_BOX_GACHA_BOX_GACHA_LIST_REQ | C2S | Request box gacha list |
| 0 | 2 | S2C_BOX_GACHA_BOX_GACHA_LIST_RES | S2C | Return box gacha list |
| 1 | 1 | C2S_BOX_GACHA_BOX_GACHA_BUY_REQ | C2S | Purchase box gacha draw |
| 1 | 2 | S2C_BOX_GACHA_BOX_GACHA_BUY_RES | S2C | Return draw results |
| 2 | 1 | C2S_BOX_GACHA_BOX_GACHA_RESET_REQ | C2S | Reset box contents |
| 2 | 2 | S2C_BOX_GACHA_BOX_GACHA_RESET_RES | S2C | Reset confirmation |
| 3 | 1 | C2S_BOX_GACHA_BOX_GACHA_DRAW_INFO_REQ | C2S | Get current box contents |
| 3 | 2 | S2C_BOX_GACHA_BOX_GACHA_DRAW_INFO_RES | S2C | Return current box contents |

### Data Structures

#### CDataBoxGachaInfo
```csharp
public uint Id { get; set; }                                    // u32
public long Begin { get; set; }                                 // s64
public long End { get; set; }                                   // s64
public bool Unk1 { get; set; }                                  // bool
public string Name { get; set; }                                // MtString
public string Description { get; set; }                         // MtString
public string Detail { get; set; }                              // MtString
public byte WeightDispType { get; set; }                        // u8
public string FreeSpaceText { get; set; }                       // MtString
public string ListAddr { get; set; }                            // MtString
public string ImageAddr { get; set; }                           // MtString
public List<CDataBoxGachaSettlementInfo> SettlementList { get; set; }
public List<CDataBoxGachaItemInfo> BoxGachaSets { get; set; }
```

#### CDataBoxGachaSettlementInfo
```csharp
public uint DrawId { get; set; }            // u32
public uint Id { get; set; }                // u32 - settlement type
public uint Price { get; set; }             // u32
public uint BasePrice { get; set; }         // u32
public uint DrawNum { get; set; }           // u32 - items per draw
public uint BonusNum { get; set; }          // u32
public uint PurchaseNum { get; set; }       // u32
public uint PurchaseMaxNum { get; set; }    // u32
public uint SpecialPriceNum { get; set; }   // u32
public uint SpecialPriceMaxNum { get; set; } // u32
public uint Unk1 { get; set; }              // u32
```

#### CDataBoxGachaItemInfo
```csharp
public uint ItemId { get; set; }        // u32
public uint ItemNum { get; set; }       // u32 - quantity per win
public uint ItemStock { get; set; }     // u32 - remaining in box
public uint Rank { get; set; }          // u32 - rarity
public uint Effect { get; set; }        // u32
public double Probability { get; set; } // f64 - current odds
public uint DrawNum { get; set; }       // u32 - times drawn
```

---

## DTI Class References

From `dti_prop_dump.h`, relevant UI classes found:

### GUI Classes
- `uGUIGPShop` (Size: 0x4BC0) - Main GP shop interface
- `uGUIGPShopDialog` (Size: 0x1890) - GP shop dialog
- `uGUIGachaAnnounce` (Size: 0x9C0) - Gacha announcement
- `uGUIBoxGachaInfo` (Size: 0x87F0) - Box gacha info display
- `uGUIShopItem` (Size: 0xD90) - Generic shop item display

### Talk State Classes (NPC Interaction)
- `cTalkStateGacha` (Size: 0x10) - Gacha NPC state
- `cTalkStateBoxGacha` (Size: 0x10) - Box gacha NPC state

### Scroll List Classes (GP Shop)
- `uGUIGPShop::cScrollShopItemList` (Size: 0x20)
- `uGUIGPShop::cScrollShopDispList` (Size: 0x84)
- `uGUIGPShop::cScrollHistoryItemList` (Size: 0x20)
- `uGUIGPShop::cScrollHistoryDispList` (Size: 0x4C)
- `uGUIGPShop::cScrollStatusItemList` (Size: 0x24)
- `uGUIGPShop::cScrollStatusDispList` (Size: 0x88)
- `uGUIGPShop::cScrollChargeItemList` (Size: 0x20)
- `uGUIGPShop::cScrollChargeDispList` (Size: 0x80)
- `uGUIGPShop::cScrollTicketItemList` (Size: 0x20)
- `uGUIGPShop::cScrollTicketDispList` (Size: 0x48)
- `uGUIGPShop::cScrollCOGItemList` (Size: 0x20)
- `uGUIGPShop::cScrollCOGDispList` (Size: 0x1B0)

### Box Gacha UI Classes
- `uGUIBoxGachaInfo::cItemInfo` (Size: 0x20)
- `uGUIBoxGachaInfo::cListInfo` (Size: 0x40)
- `uGUIBoxGachaInfo::cListItem` (Size: 0x1A0)
- `uGUIBoxGachaInfo::cLotInfo` (Size: 0x178)
- `uGUIBoxGachaInfo::cLotList` (Size: 0x298)
- `uGUIBoxGachaInfo::cSumList` (Size: 0x9C)

---

## Implementation Recommendations

### Priority 1: GP Shop Lineup Packets
The following handlers should be implemented:
1. `GP_GP_SHOP_DISPLAY_GET_LINEUP` (ID 8) - Core shop functionality
2. `GP_GP_SHOP_DISPLAY_BUY` (ID 9) - Purchase flow
3. `GP_GP_SHOP_CAN_BUY` (ID 23) - Purchase validation

### Priority 2: Course/Item Lineups
1. `GP_GP_SHOP_GET_COURSE_LINEUP` (ID 10)
2. `GP_GP_SHOP_GET_ITEM_LINEUP` (ID 11)
3. `GP_GP_SHOP_GET_PAWN_LINEUP` (ID 12)

### Priority 3: Box Gacha Reset
1. Complete `BoxGachaResetHandler` for box reset functionality
2. Track per-character box state in database

### Database Considerations
- GP Shop purchase history needs persistent storage
- Box Gacha state (items remaining) needs per-character tracking
- Gacha statistics/history for anti-manipulation

---

## Existing Asset References

The server uses asset files for gacha configuration:
- `Server.AssetRepository.GachaAsset.GachaInfoList` - Dictionary of gacha configurations
- Asset loading handled in `AssetRepository`

---

## Notes

1. Settlement IDs appear to map to payment types:
   - 1 = Golden Gemstones (GP)
   - 2 = Silver Tickets

2. Japanese comments from packet dump provide context:
   - ID 7: "Get shop menu" (shop categories)
   - ID 8: "Get shop item list"
   - ID 9: "Get purchase result"
   - ID 10: "Get course lineup"
   - ID 15: "Get purchase history"

3. The GP shop appears to have multiple display modes based on category ID:
   - ID 1: Selection
   - ID 3: Passport
   - IDs 6-15: Various item categories (commented out in handler)
