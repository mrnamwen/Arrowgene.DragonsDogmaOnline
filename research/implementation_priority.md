# Implementation Priority List

## Handlers Still Using Packet Dumps (32 total)

### Tier 1: Simple Solo Features (Quick Wins)

| Handler | Description | Complexity |
|---------|-------------|------------|
| `BlackListGetBlackListHandler` | Player blacklist - can return empty list initially | Low |
| `ConnectionGetLoginAnnouncementHandler` | Login announcements/MOTD - useful for server messages | Low |
| `OrbDevoteGetOrbGainExtendParamHandler` | Orb system parameters | Low |
| `GpGetUpdateAppCourseBonusFlagHandler` | GP course bonus flags | Low |

### Tier 2: System Mail (Related Group)

| Handler | Description | Complexity |
|---------|-------------|------------|
| `MailSystemMailGetListHeadHandler` | System mail list header | Medium |
| `MailSystemMailGetListDataHandler` | System mail list data | Medium |
| `MailSystemMailGetListFootHandler` | System mail list footer | Medium |
| `MailMailGetListFootHandler` | Personal mail footer | Medium |

### Tier 3: Quest Reward Box

| Handler | Description | Complexity |
|---------|-------------|------------|
| `QuestGetRewardBoxListHandler` | Quest reward box list | Medium |
| `QuestGetRewardBoxItemHandler` | Claim items from reward box | Medium |

### Tier 4: Pawn System

| Handler | Description | Complexity |
|---------|-------------|------------|
| `PawnGetNoraPawnListHandler` | NPC "Nora" pawns available for hire | Medium |

### Tier 5: Quest Lists (Complex - Quest System Integration)

| Handler | Description | Complexity |
|---------|-------------|------------|
| `QuestGetSetQuestListHandler` | Set quest list | High |
| `QuestGetWorldManageQuestListHandler` | World manage quest list | High |
| `QuestGetAdventureGuideQuestListHandler` | Adventure guide quests | High |
| `QuestGetCycleContentsNewsListHandler` | Cycle contents news | High |
| `QuestGetCycleContentsStateListHandler` | Cycle contents state | High |
| `QuestGetMainQuestListHandler` | Main quest list | High |
| `QuestGetPackageQuestListHandler` | Package quest list | High |
| `QuestGetPriorityQuestHandler` | Priority quest | High |
| `QuestGetQuestCompletedListHandler` | Completed quests list | High |

### Tier 6: Social/Friends

| Handler | Description | Complexity |
|---------|-------------|------------|
| `FriendGetRecentCharacterListHandler` | Recent characters list | Medium |

### Tier 7: Server/Connection

| Handler | Description | Complexity |
|---------|-------------|------------|
| `ServerGetGameSettingHandler` | Game settings | Medium |
| `ConnectionMoveInServerHandler` | Server connection/channel move | High |

### Skip: Requires Real Party/Multiplayer

| Handler | Description | Reason |
|---------|-------------|--------|
| `PartyMemberSetValueHandler` | Party member values | Requires party |
| `EntryBoardEntryBoardItemCreateHandler` | Entry board create | Requires matchmaking |
| `EntryBoardEntryBoardItemExtendTimeoutHandler` | Entry board timeout | Requires matchmaking |
| `EntryBoardEntryBoardItemForceStartHandler` | Entry board force start | Requires matchmaking |
| `EntryBoardEntryBoardItemInfoMyselfHandler` | Entry board self info | Requires matchmaking |
| `EntryBoardEntryBoardListHandler` | Entry board list | Requires matchmaking |
| `GroupChatGroupChatGetMemberListHandler` | Group chat members | Requires multiplayer |
| `BattleContentPartyMemberInfoUpdateHandler` | Battle content party | Requires party |
| `BattleContentInfoListHandler` | Battle content list | Requires party content |

---

## Recently Completed

- [x] Ultimate Synthesis (stat lottery for equipment enhancement)
- [x] Daily Missions (daily/weekly missions with rewards)
- [x] Pawn Expeditions
- [x] Treasure Points
- [x] Quest Bonuses (Area, Level, Party, Quest Party)
- [x] Mandragora system
- [x] Personal Mail (send/receive/delete)
- [x] Equip Presets
- [x] Quick Party system

---

## Recommended Implementation Order

1. **Login Announcements** - Simple, immediately useful for server operators
2. **Blacklist** - Simple, database-backed
3. **System Mail** - Group of 3-4 handlers, useful feature
4. **Quest Reward Box** - 2 handlers, ties into quest completion
5. **Nora Pawns** - Enhances pawn rental system
6. **Quest Lists** - Complex, requires deep quest system knowledge

---

## Notes

- All handlers listed are in `Arrowgene.Ddon.GameServer/Handler/`
- Packet dumps are in `Arrowgene.Ddon.GameServer/Dump/GameFullDump.cs`
- Follow existing patterns: Asset + AssetDeserializer + JSON + Handler update
- Use `GameRequestPacketHandler<TReq, TRes>` pattern for request/response handlers
