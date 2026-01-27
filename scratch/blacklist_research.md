# Blacklist System Research - DDON

## Key Finding: Database Infrastructure Already Exists!

The blacklist system can use the **existing** `ddon_contact_list` table and database methods.

### Database Schema
```sql
CREATE TABLE "ddon_contact_list" (
    "id" INTEGER PRIMARY KEY AUTOINCREMENT,
    "requester_character_id" INTEGER NOT NULL,
    "requested_character_id" INTEGER NOT NULL,
    "status" SMALLINT NOT NULL,
    "type" SMALLINT NOT NULL,  -- 0=FriendList, 1=BlackList
    "requester_favorite" BOOLEAN NOT NULL,
    "requested_favorite" BOOLEAN NOT NULL
);
```

### Existing Model Support
```csharp
public enum ContactListType : byte
{
    FriendList = 0,
    BlackList = 1  // <-- Already defined!
}

public enum ContactListStatus : byte
{
    PendingApproval = 0,
    Accepted = 1,
    Blacklist = 2  // <-- Status for blacklist entries
}
```

### Existing Database Methods (IDatabase.cs)
- `InsertContact(...)` - Can create blacklist entries
- `SelectContactsByCharacterId(...)` - Can query blacklist
- `SelectFullContactListByCharacterId(...)` - Returns contact with character info
- `DeleteContactById(...)` - Can remove blacklist entries

### Current Implementation Status

**Implemented:**
- `BlackListGetBlackListHandler` - Returns empty list (basic implementation)
- Packet structures created: `C2SBlackListGetBlackListReq`, `S2CBlackListGetBlackListRes`

**Not Implemented (Future Work):**
- `BlackListAddBlackListHandler` - Would use `InsertContact` with `ContactListType.BlackList`
- `BlackListRemoveBlackListHandler` - Would use `DeleteContactById`
- Full database-backed blacklist retrieval

### Future Implementation Guide

To implement full blacklist functionality:

```csharp
// BlackListGetBlackListHandler - Full implementation
public override S2CBlackListGetBlackListRes Handle(GameClient client, C2SBlackListGetBlackListReq request)
{
    var res = new S2CBlackListGetBlackListRes();
    var contacts = Database.SelectFullContactListByCharacterId(client.Character.CharacterId);

    foreach ((var contact, var character) in contacts)
    {
        if (contact.Type != ContactListType.BlackList) continue;
        if (contact.RequesterCharacterId != client.Character.CharacterId) continue;

        res.BlackList.Add(ContactListManager.CharacterToCommunityInfo(character));
    }
    return res;
}
```

### Packet IDs (Already Defined)
- `C2S_BLACK_LIST_GET_BLACK_LIST_REQ` (17, 0, 1)
- `S2C_BLACK_LIST_GET_BLACK_LIST_RES` (17, 0, 2)
- `C2S_BLACK_LIST_ADD_BLACK_LIST_REQ` (17, 1, 1)
- `S2C_BLACK_LIST_ADD_BLACK_LIST_RES` (17, 1, 2)
- `C2S_BLACK_LIST_REMOVE_BLACK_LIST_REQ` (17, 2, 1)
- `S2C_BLACK_LIST_REMOVE_BLACK_LIST_RES` (17, 2, 2)

### Template Handlers
Reference these for implementation patterns:
- `FriendGetFriendListHandler.cs` - List retrieval pattern
- `FriendApplyFriendListHandler.cs` - Add contact pattern
- `FriendRemoveFriendHandler.cs` - Remove contact pattern
