# Orb Devote System and OrbGainExtendParam Research

## Overview

The **Orb Devote System** in DDON is a progression mechanic that allows both players and pawns to unlock "Dragon Force Augmentation Upgrades" (referred to as "Orbs") to gain permanent stat bonuses and special abilities. This is a complex system with 4 pages of upgrades, multiple categories, and sophisticated unlock restrictions.

---

## Key Handler: OrbDevoteGetOrbGainExtendParamHandler

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.GameServer/Handler/OrbDevoteGetOrbGainExtendParamHandler.cs`

### Handler Implementation
```csharp
public class OrbDevoteGetOrbGainExtendParamHandler : PacketHandler<GameClient>
{
    public override PacketId Id => PacketId.C2S_ORB_DEVOTE_GET_ORB_GAIN_EXTEND_PARAM_REQ;

    public override void Handle(GameClient client, IPacket packet)
    {
        S2COrbDevoteGetOrbGainExtendParamRes Result = new S2COrbDevoteGetOrbGainExtendParamRes()
        {
            ExtendParam = client.Character.CalculateFullExtendedParams()
        };
        client.Send(Result);
    }
}
```

### What It Does
- Responds to client requests for the current extended parameters from orb upgrades
- Calls `CalculateFullExtendedParams()` on the character to compute combined stats
- Returns a response packet with the calculated extended parameter data

---

## Data Structure: CDataOrbGainExtendParam

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Entity/Structure/CDataOrbGainExtendParam.cs`

### Properties (all ushort)
- `HpMax` - Maximum HP bonus
- `StaminaMax` - Maximum stamina bonus
- `Attack` - Physical attack bonus
- `Defence` - Physical defense bonus
- `MagicAttack` - Magic attack bonus
- `MagicDefence` - Magic defense bonus
- `AbilityCost` - Ability cost reduction (allows more abilities to be equipped)
- `JewelrySlot` - Jewelry/ring slot count
- `UseItemSlot` - Consumable item slot count
- `MaterialItemSlot` - Material item slot count
- `EquipItemSlot` - Equipment item slot count
- `MainPawnSlot` - Main pawn slot count
- `SupportPawnSlot` - Support pawn rental slot count

### Overloaded Addition Operator
The structure supports addition via `+` operator, allowing stats to be accumulated from multiple sources.

### Serialization
Uses `CDataOrbGainExtendParam.Serializer` for binary packet serialization (26 bytes total: 13 ushort values).

---

## Response Packet Structure

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Entity/PacketStructure/S2COrbDevoteGetOrbGainExtendParamRes.cs`

```csharp
public class S2COrbDevoteGetOrbGainExtendParamRes : ServerResponse
{
    public override PacketId Id => PacketId.S2C_ORB_DEVOTE_GET_ORB_GAIN_EXTEND_PARAM_RES;
    
    public CDataOrbGainExtendParam ExtendParam { get; set; }
}
```

### Packet Flow
1. Client sends: `C2S_ORB_DEVOTE_GET_ORB_GAIN_EXTEND_PARAM_REQ`
2. Server responds with: `S2C_ORB_DEVOTE_GET_ORB_GAIN_EXTEND_PARAM_RES`
3. Response contains calculated extended parameters

---

## OrbGainParamType Enumeration

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Model/OrbGainParamType.cs`

### All-Jobs Parameters (Values 1-9, 15-18)
- `AllJobsHpMax` (1) - +HP for all jobs
- `AllJobsStaminaMax` (2) - +Stamina for all jobs
- `AllJobsPhysicalAttack` (3) - +Physical attack for all jobs
- `AllJobsPhysicalDefence` (4) - +Physical defense for all jobs
- `AllJobsMagicalAttack` (5) - +Magic attack for all jobs
- `AllJobsMagicalDefence` (6) - +Magic defense for all jobs
- `AbilityCost` (9) - Ability cost reduction
- `AccessorySlot` (10) - Jewelry slot count
- `UseItemSlot` (16) - Consumable item slots
- `MaterialItemSlot` (17) - Material item slots
- `EquipItemSlot` (18) - Equipment item slots

### Job-Specific Parameters (Values 20-25)
- `JobHpMax` (20)
- `JobStaminaMax` (21)
- `JobPhysicalAttack` (22)
- `JobPhysicalDefence` (23)
- `JobMagicalAttack` (24)
- `JobMagicalDefence` (25)

### Pawn/Slot Parameters
- `MainPawnSlot` (14) - Main pawn slot count
- `SupportPawnSlot` (15) - Support pawn (rental) slot count
- `MainPawnLostRate` (13) - Pawn death rate modifier (pawn-only)

### Special Parameters
- `SecretAbility` (19) - Unlock secret/hidden ability
- `Rim` (7) - Rift Points currency
- `Gold` (8) - Gold currency
- `PawnAdventureNum` (11) - Pawn adventure count (TODO: unhandled)
- `PawnCraftNum` (12) - Pawn craft count (TODO: unhandled, possibly pre-Season 3 relic)

### Helper Method
```csharp
public static bool IsJobOnlyParam(this OrbGainParamType type)
{
    // Returns true for job-specific parameters (20-25)
}
```

---

## Character Extension Method: CalculateFullExtendedParams

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Model/CharacterCommon.cs`

```csharp
public CDataOrbGainExtendParam CalculateFullExtendedParams()
{
    return new CDataOrbGainExtendParam()
    {
        HpMax = (ushort)StatusInfo.GainHP,
        StaminaMax = (ushort)StatusInfo.GainStamina,
        Attack = (ushort)StatusInfo.GainAttack,
        Defence = (ushort)StatusInfo.GainDefense,
        MagicAttack = (ushort)StatusInfo.GainMagicAttack,
        MagicDefence = (ushort)StatusInfo.GainMagicDefense,
        AbilityCost = ExtendedParams.AbilityCost,
        JewelrySlot = ExtendedParams.JewelrySlot,
        UseItemSlot = ExtendedParams.UseItemSlot,
        MaterialItemSlot = ExtendedParams.MaterialItemSlot,
        EquipItemSlot = ExtendedParams.EquipItemSlot,
        MainPawnSlot = ExtendedParams.MainPawnSlot,
        SupportPawnSlot = ExtendedParams.SupportPawnSlot
    };
}
```

### Key Insight
- Combat stats (HP, Stamina, Attack, Defense, Magic) come from `StatusInfo.Gain*` properties (calculated from job levels via the StatusInfo system)
- Slot bonuses (jewelry, items, pawns) come from `ExtendedParams` (stored orb progress)

---

## Related Handlers

### OrbDevoteGetReleaseOrbElementListHandler
**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.GameServer/Handler/OrbDevoteGetReleaseOrbElementListHandler.cs`

Retrieves the list of orbs already unlocked by the player:
```csharp
Response.OrbElementList = _Database.SelectOrbReleaseElementFromDragonForceAugmentation(client.Character.CommonId);
```

### OrbDevoteReleaseOrbElementHandler
**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.GameServer/Handler/OrbDevoteReleaseOrbElementHandler.cs`

Handles unlock requests for player orbs - delegates to `OrbUnlockManager.UnlockDragonForceAugmentationUpgrade()`

### OrbDevoteGetPawnReleaseOrbElementListHandler
**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.GameServer/Handler/OrbDevoteGetPawnReleaseOrbElementListHandler.cs`

Gets list of pawn orb unlocks for a specific pawn:
```csharp
Pawn pawn = client.Character.PawnById(request.PawnId, PawnType.Main);
response.OrbElementList = Database.SelectOrbReleaseElementFromDragonForceAugmentation(pawn.CommonId);
```

### OrbDevoteReleasePawnOrbElementHandler
**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.GameServer/Handler/OrbDevoteReleasePawnOrbElementHandler.cs`

Unlocks orbs for pawns via the OrbUnlockManager.

---

## Core Manager: OrbUnlockManager

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.GameServer/Characters/OrbUnlockManager.cs`

### Main Method: UnlockDragonForceAugmentationUpgrade

This method:
1. Validates the upgrade exists and is available for the character/pawn type
2. Checks unlock restrictions (level gates, orb costs, page unlocks)
3. Deducts currency if required
4. Updates extended parameter data in database
5. Stores unlock record in database
6. Sends response packet with remaining orbs and gain value
7. Sends notification updates to client

### Dragon Force Upgrades Structure

**Two Static Dictionaries:**
- `gPlayerDragonForceUpgrades` - Player-specific upgrades (elements 0x01-0x94)
- `gPawnDragonForceUpgrades` - Pawn-specific upgrades (elements 0x95-0x128)

### Upgrade Organization: 4 Pages × 5 Groups

Each page has 5 groups:
1. **Group 1 (Vitality)** - HP/Stamina bonuses + abilities (Efficacy, Soft Touch, Flow, etc.)
2. **Group 2 (Adventure)** - Item slots, ability cost, currency (Gold/Rim)
3. **Group 3 (Magick)** - Magic attack/defense + abilities (Rain Defense, Rakshasa, Moonlight Defense)
4. **Group 4 (Combat)** - Physical attack/defense + abilities (Rain Attack, Yasha, Moonlight Assault)
5. **Group 5 (Other)** - Page unlock gates + accessory slots

### Unlock Restriction Types

```csharp
private enum LvlUpRestrictionType
{
    None = 0,
    Orbs = 1,           // Cost in Blood Orbs (wallet currency)
    TotalLevels = 2,    // Gated by total character levels across all jobs
    PageUnlocked = 3    // Gated by completing previous page
}
```

### Page Progression
- **Page 1 (Entry):** No level/orb requirements for most early unlocks
  - Group 5: Accessibility unlocks (10+ entry items)
  - Groups 1-4: Orb-gated (10-75 orbs)
- **Page 2:** Higher requirements
  - Group 5: Level 16-28 total gates
  - Groups 1-4: 70-250 orbs
- **Page 3:** Mid-game progression
  - Group 5: Level 32-44 total gates
  - Groups 1-4: 300-1000 orbs
- **Page 4:** End-game progression
  - Group 5: Level 48-60 total gates
  - Groups 1-4: 1250-5000 orbs

### Parameter Updates in UpdateExtendedParamData

When an upgrade is unlocked, stat updates include:

**Combat Stats:**
```csharp
case OrbGainParamType.AllJobsHpMax:
    obj.HpMax += (ushort)upgrade.Amount;
    break;
// ... similar for Stamina, Attack, Defense, MagicAttack, MagicDefense
```

**Slot Bonuses:**
```csharp
case OrbGainParamType.AbilityCost:
    obj.AbilityCost += (ushort)upgrade.Amount;
    break;
case OrbGainParamType.MainPawnSlot:
    obj.MainPawnSlot += (ushort)upgrade.Amount;
    client.Character.MyPawnSlotNum += (byte)upgrade.Amount;
    _Server.Database.UpdateMyPawnSlot(...);
    break;
```

**Abilities:**
```csharp
case OrbGainParamType.SecretAbility:
    queue.Enqueue(client, _Server.JobManager.UnlockSecretAbility(...));
    break;
```

**Currency:**
```csharp
case OrbGainParamType.Rim:
    client.Enqueue(_Server.WalletManager.AddToWalletNtc(...));
    break;
case OrbGainParamType.Gold:
    client.Enqueue(_Server.WalletManager.AddToWalletNtc(...));
    break;
```

---

## Data Structures: Related

### CDataReleaseOrbElement
**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Entity/Structure/CDataReleaseOrbElement.cs`

Tracks each unlocked orb element:
```csharp
public class CDataReleaseOrbElement
{
    public UInt32 ElementId { get; set; }  // Orb identifier
    public byte PageNo { get; set; }       // Page location (1-4)
    public byte GroupNo { get; set; }      // Category location (1-5)
    public byte Index { get; set; }        // Position within group
}
```

### CDataOrbPageStatus
**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Entity/Structure/CDataOrbPageStatus.cs`

UI-related tracking for each page:
```csharp
public class CDataOrbPageStatus
{
    public byte PageNo { get; set; }
    public List<CDataOrbCategoryStatus> CategoryStatusList { get; set; }
}
```

### CDataOrbCategoryStatus
**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Entity/Structure/CDataOrbCategoryStatus.cs`

Completion tracking per category:
```csharp
public class CDataOrbCategoryStatus
{
    public byte CategoryId { get; set; }      // Group 1-5
    public byte ReleaseNum { get; set; }      // Count of unlocked items in category
}
```

---

## Character Data Integration

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.Shared/Model/CharacterCommon.cs`

### Related Character Properties
```csharp
public CDataOrbGainExtendParam ExtendedParams { get; set; } = new();
public Dictionary<JobId, CDataOrbGainExtendParam> ExtendedJobParams { get; set; } = [];
public Dictionary<JobId, HashSet<uint>> ReleasedExtendedJobParams { get; set; } = [];
public List<CDataReleaseOrbElement> OrbRelease { get; set; } = [];
```

These properties store:
- `ExtendedParams` - Dragon Force augmentation stat bonuses (all-jobs)
- `ExtendedJobParams` - Job Orb Tree stat bonuses (job-specific)
- `ReleasedExtendedJobParams` - Tracking which job-specific orbs are unlocked
- `OrbRelease` - Complete list of Dragon Force orbs unlocked

---

## Job Orb Tree System (Related but Separate)

**File:** `/home/nam/Documents/Arrowgene.DragonsDogmaOnline/Arrowgene.Ddon.GameServer/Characters/JobOrbUnlockManager.cs`

This is a separate system (Job Orb Tree / Skill Augmentation) that runs parallel to Dragon Force:
- Manages job-specific stat bonuses and ability unlocks
- Uses script-defined upgrades from `ScriptManager.SkillAugmentationModule`
- Tracks released elements per job
- Uses different currency system (wallet types from scripts)
- Includes Season 2 and Special Skill Augmentation variants

---

## Database Integration

### Key Database Methods (inferred from code)
- `SelectOrbReleaseElementFromDragonForceAugmentation(commonId)` - Get all unlocked orbs
- `InsertIfNotExistsDragonForceAugmentation(commonId, elementId, pageNo, groupNo, indexNo)` - Store unlock
- `UpdateOrbGainExtendParam(commonId, extendParams)` - Persist stat changes
- `GetSkillAugmentationReleasedElements(characterId, jobId)` - Job orb tracking

---

## Complete Packet Flow

### Orb Gain Extended Param Request
1. **Client → Server:** `C2S_ORB_DEVOTE_GET_ORB_GAIN_EXTEND_PARAM_REQ`
2. **Server Processing:**
   - Fetch character from client
   - Call `character.CalculateFullExtendedParams()`
   - Aggregate all stat bonuses
3. **Server → Client:** `S2C_ORB_DEVOTE_GET_ORB_GAIN_EXTEND_PARAM_RES`
   - Contains: `CDataOrbGainExtendParam`

### Orb Release (Unlock) Workflow
1. **Client → Server:** `C2S_ORB_DEVOTE_RELEASE_ORB_ELEMENT_REQ` (ElementId)
2. **Server Processing:**
   - Validate element exists and matches upgrade table
   - Check unlock conditions (level, orbs, page completion)
   - Execute transaction:
     - Update extended params
     - Insert to OrbRelease list
     - Save to database
     - Calculate achievement progress
3. **Server → Client:** `S2C_ORB_DEVOTE_RELEASE_ORB_ELEMENT_RES`
   - Contains: RestOrb (remaining orbs), GainParamType, GainParamValue
4. **Server → Client:** Notification packets
   - Character extended params update (if applicable)
   - Pawn slot notification (if applicable)
   - Wallet update (if currency gained)
   - Achievement progress (if applicable)

---

## Summary

The **Orb Devote System** is a sophisticated progression mechanic that:

1. **Provides persistent stat upgrades** through Dragon Force Augmentation (separate from Job Orb Tree)
2. **Manages 4 progression pages** with increasingly expensive requirements
3. **Supports dual progression** - both player and pawn upgrades
4. **Integrates with multiple game systems:**
   - Wallet system (Blood Orbs currency)
   - Database persistence
   - Achievement tracking
   - Character stat calculation
   - Pawn slot management
   - Ability unlocking

5. **Uses a tiered restriction system:**
   - Early game: Total level gates
   - Mid game: Orb cost gates
   - Endgame: Page completion gates

6. **Supports 30+ different stat/ability types** via OrbGainParamType enum

The `OrbDevoteGetOrbGainExtendParamHandler` is a simple query endpoint that aggregates all the stat bonuses a character has accumulated through their Dragon Force upgrades, making the data easily accessible to the client for UI display and stat calculations.
