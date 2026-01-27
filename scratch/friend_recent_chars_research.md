# Recent Characters List System Research

## Overview
Research into the "Recently Played Players" (最近遊んだプレイヤー) feature in the Arrowgene DDON server emulator.

---

## 1. Handler Implementation

### FriendGetRecentCharacterListHandler
**File:** `/Arrowgene.Ddon.GameServer/Handler/FriendGetRecentCharacterListHandler.cs`

```csharp
public class FriendGetRecentCharacterListHandler : PacketHandler<GameClient>
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(FriendGetRecentCharacterListHandler));
    
    public FriendGetRecentCharacterListHandler(DdonGameServer server) : base(server) { }
    
    public override PacketId Id => PacketId.C2S_FRIEND_GET_RECENT_CHARACTER_LIST_REQ;
    
    public override void Handle(GameClient client, IPacket packet)
    {
        client.Send(InGameDump.Dump_71);
    }
}
```

**Current Status:** The handler is STUB implementation - it simply sends a hardcoded packet dump response without any actual database query or dynamic data generation.

**Registration:** Handler is registered in `DdonGameServer.cs`:
```
AddHandler(new FriendGetRecentCharacterListHandler(this));
```

---

## 2. Packet Structures

### Packet IDs
Located in: `/Arrowgene.Ddon.Shared/Network/PacketId.cs`

#### Request Packet
- **ID:** `C2S_FRIEND_GET_RECENT_CHARACTER_LIST_REQ`
- **Packet IDs:** Group: 16, Handler: 6, SubId: 1 (16, 6, 1)
- **Type:** Client to Server Request
- **Direction:** Client initiates

#### Response Packet
- **ID:** `S2C_FRIEND_GET_RECENT_CHARACTER_LIST_RES`
- **Packet IDs:** Group: 16, Handler: 6, SubId: 2 (16, 6, 2)
- **Type:** Server to Client Response
- **Japanese Comment:** "最近遊んだプレイヤー取得に" (Recently Played Players Acquisition)
- **Direction:** Server responds

### Packet Definition Locations

**GameFullDump.cs:**
```csharp
// Request packet (C2S)
public static byte[] data_Dump_71 = new byte[] /* 16.6.1 */
{
    0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0
};
public static Packet Dump_71 = new Packet(new PacketId(16, 6, 1, "Dump_71"), data_Dump_71);
```

**InGameDump.cs:**
```csharp
// Response packet (S2C)
public static byte[] data_Dump_71 = new byte[] /* 16.6.2 */
{
    0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0,
    0x0, 0x0, 0x3, 0xF2, 0x41, 0x1, 0x0
};
public static Packet Dump_71 = new Packet(new PacketId(16, 6, 2, "Dump_71"), data_Dump_71);
```

**Note:** No dedicated packet structure classes (C2SFriendGetRecentCharacterListReq/S2CFriendGetRecentCharacterListRes) currently exist in `/Arrowgene.Ddon.Shared/Entity/PacketStructure/`.

---

## 3. Database Schema Analysis

### Current Database Support
- **SQLite:** Primary development database
- **PostgreSQL:** Supported for production

### Friend/Contact System Tables
The friend system uses a generic **ContactList** table rather than dedicated "recent players" tracking.

#### ContactList Methods in IDatabase
Located in: `/Arrowgene.Ddon.Database/IDatabase.cs`

```csharp
// ContactList
int InsertContact(uint requestingCharacterId, uint requestedCharacterId, 
                  ContactListStatus status, ContactListType type, 
                  bool requesterFavorite, bool requestedFavorite);

int UpdateContact(uint requestingCharacterId, uint requestedCharacterId, 
                  ContactListStatus status, ContactListType type, 
                  bool requesterFavorite, bool requestedFavorite);

List<ContactListEntity> SelectContactsByCharacterId(uint characterId);

ContactListEntity SelectContactsByCharacterId(uint characterId1, uint characterId2);

ContactListEntity SelectContactListById(uint id);

List<(ContactListEntity, CDataCharacterListElement)> SelectFullContactListByCharacterId(
    uint characterId, DbConnection? connectionIn = null);
```

#### ContactListType Enum
Used to categorize different contact list types:
- `FriendList` - Accepted friends
- `BlackList` - Blocked players
- `RecentPlayers` - (Potentially used for recent encounters, but no dedicated implementation found)

### Current Findings
**No dedicated database table or methods for tracking recently encountered players were found.**
- The handler currently returns empty/stub data
- No migrations or schema changes for "recent players" tracking exist
- The ContactList system could potentially be extended to support this feature

---

## 4. Friend System Architecture

### Related Handler Classes
Located in: `/Arrowgene.Ddon.GameServer/Handler/`

1. **FriendApplyFriendListHandler** - Apply for friendship
2. **FriendApproveFriendListHandler** - Approve friend requests
3. **FriendRemoveFriendHandler** - Remove friend
4. **FriendRegisterFavoriteFriendHandler** - Mark friend as favorite
5. **FriendCancelFriendApplicationHandler** - Cancel pending request
6. **FriendGetFriendListHandler** - Retrieve friend list
7. **FriendGetRecentCharacterListHandler** - **GET RECENT PLAYERS** (stub implementation)

### FriendGetFriendListHandler Pattern (Reference Implementation)
File: `/Arrowgene.Ddon.GameServer/Handler/FriendGetFriendListHandler.cs`

```csharp
public class FriendGetFriendListHandler : GameRequestPacketHandler<C2SFriendGetFriendListReq, S2CFriendGetFriendListRes>
{
    public override S2CFriendGetFriendListRes Handle(GameClient client, C2SFriendGetFriendListReq request)
    {
        S2CFriendGetFriendListRes res = new();
        
        // Database query to get all contacts
        List<(ContactListEntity, CDataCharacterListElement)> friends = 
            Database.SelectFullContactListByCharacterId(client.Character.CharacterId);
        
        // Filter by type and status
        foreach ((ContactListEntity contact, CDataCharacterListElement character) in friends)
        {
            if (contact.Type != ContactListType.FriendList) continue;
            
            if (contact.Status == ContactListStatus.Accepted)
            {
                res.FriendInfoList.Add(new CDataFriendInfo()
                {
                    CharacterListElement = character,
                    PendingStatus = 0,
                    IsFavorite = contact.IsFavoriteForCharacter(client.Character.CharacterId),
                    FriendNo = contact.Id,
                });
            }
        }
        return res;
    }
}
```

This is the **pattern expected** for RecentCharacterListHandler once implemented.

---

## 5. Contact/Friend Data Structures

### CDataFriendInfo
File: `/Arrowgene.Ddon.Shared/Entity/Structure/CDataFriendInfo.cs`

```csharp
public class CDataFriendInfo
{
    public CDataCharacterListElement CharacterListElement { get; set; }
    public byte PendingStatus { get; set; }
    public uint FriendNo { get; set; }
    public bool IsFavorite { get; set; }
}
```

### ContactListEntity
Database entity representing a contact list entry with:
- `RequesterCharacterId` - Character initiating contact
- `RequestedCharacterId` - Character receiving contact
- `Type` - ContactListType (Friend, Block, Recent, etc.)
- `Status` - ContactListStatus (Accepted, Pending, etc.)
- `IsFavorite` flags for both characters

### CDataCharacterListElement
Contains full character information:
- `OnlineStatus`
- `CharacterName` (FirstName, LastName)
- `Job` and `Level`
- `ClanName`
- `MatchingProfile`
- `Server` information

---

## 6. How Friend System Tracks Encounters

### Current Implementation Gaps

#### No Automatic Encounter Tracking
- **Party Join:** When players join a party (`PartyPartyJoinHandler`), no recent player record is created
- **Co-op Encounters:** No tracking when players meet in quests or dungeons
- **Server Move:** `ConnectionMoveInServerHandler` doesn't trigger encounter logging

#### Missing Integration Points
1. **Party System** - Could track members when party forms
2. **Quest Completion** - Could log co-op participants
3. **Dungeon Instances** - Could log dungeon group members
4. **PvP/Multiplayer Events** - Could log defeated/allied players

#### Related Manager Classes
Located in: `/Arrowgene.Ddon.GameServer/Characters/`

- `PartyManager` - Manages party groups and members
- `CharacterManager` - Manages character state
- `ContactListManager` - Utility methods for contact/friend conversions
  - `CharacterToFriend()` - Converts Character to CDataFriendInfo
  - `CharacterToListEml()` - Converts Character to CDataCharacterListElement

---

## 7. Implementation Roadmap

### To Implement Full Recent Players Feature:

#### Phase 1: Database Schema
1. Create a new table for encounter tracking:
   ```sql
   CREATE TABLE character_recent_encounters (
       id INTEGER PRIMARY KEY,
       character_id INTEGER NOT NULL,
       recent_character_id INTEGER NOT NULL,
       encounter_type VARCHAR(50),  -- 'party', 'quest', 'pvp', 'dungeon'
       encounter_timestamp DATETIME,
       location VARCHAR(255),
       FOREIGN KEY(character_id) REFERENCES character(character_id),
       FOREIGN KEY(recent_character_id) REFERENCES character(character_id)
   );
   ```
   OR modify ContactList to include:
   - `encounter_timestamp` field
   - Auto-create ContactListEntry when players meet

#### Phase 2: Integration Points
1. `PartyPartyJoinHandler` - Log party member encounters
2. `ConnectionMoveInServerHandler` - Track server transitions
3. Quest completion handlers - Log quest participants
4. Instance dungeon handlers - Log dungeon group members

#### Phase 3: Response Handler
1. Create proper packet structures:
   - `C2SFriendGetRecentCharacterListReq.cs`
   - `S2CFriendGetRecentCharacterListRes.cs`
2. Implement `FriendGetRecentCharacterListHandler.Handle()` with:
   - Query recent encounters from database
   - Apply sorting (most recent first)
   - Limit results (typically 20-30 entries)
   - Filter out self, blocked players, current friends

#### Phase 4: Testing
1. Verify encounter logging
2. Test response serialization
3. Client display validation

---

## 8. References and Related Code

### Handler Registry
File: `/Arrowgene.Ddon.GameServer/DdonGameServer.cs`
```csharp
AddHandler(new FriendGetRecentCharacterListHandler(this));
```

### Packet Definition
File: `/Arrowgene.Ddon.Shared/Network/PacketId.cs`
- Packet IDs registered in `AddGamePacketIds()` method
- Japanese name confirms feature: "最近遊んだプレイヤー取得に"

### Manager Pattern
Contact/Friend management uses `ContactListManager`:
- `/Arrowgene.Ddon.GameServer/Characters/ContactListManager.cs`
- Provides conversion utilities between Character and Contact/Friend structures

### Database Access
All database operations go through `IDatabase` interface:
- `/Arrowgene.Ddon.Database/IDatabase.cs`
- Implementations use `SelectFullContactListByCharacterId()` pattern

---

## 9. Key Findings Summary

1. **Handler Status:** STUB - Returns hardcoded empty response
2. **Database Support:** None currently implemented
3. **Packet Structure:** IDs defined but no serializable classes
4. **Encounter Tracking:** Not implemented at party/quest level
5. **Contact System:** Could be repurposed for recent players
6. **Response Format:** Currently empty (0x00 bytes in dump)
7. **Feature Intent:** Clear from Japanese packet comment - "Recently Played Players Acquisition"
8. **Integration Ready:** Handler hook in place, just needs implementation

---

## 10. TODO for Full Implementation

- [ ] Create database migration for encounter tracking
- [ ] Create packet structure classes (C2S/S2C variants)
- [ ] Implement encounter logging in Party join/leave handlers
- [ ] Implement encounter logging in quest completion handlers
- [ ] Implement encounter logging in dungeon instance handlers
- [ ] Modify FriendGetRecentCharacterListHandler to query database
- [ ] Add timestamp-based sorting (most recent first)
- [ ] Add limit to response (e.g., 20-30 most recent)
- [ ] Test end-to-end with multiplayer scenarios
- [ ] Add player filtering (remove self, blocked, existing friends)

