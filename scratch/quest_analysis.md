# Quest System Analysis - Arrowgene.DragonsDogmaOnline

## Overview

The quest system in Arrowgene.DragonsDogmaOnline is a sophisticated implementation that supports multiple quest types, a state machine-based progression system, and flexible scripting interfaces. The system is designed to handle everything from simple tutorial quests to complex multi-phase extreme missions.

## Directory Structure

```
Arrowgene.Ddon.GameServer/
  Quests/
    Quest.cs                    - Base quest class with all core functionality
    GenericQuest.cs             - Generic quest implementation from assets
    QuestStateManager.cs        - State management for active quests
    QuestProcess.cs             - Process/block execution model
    QuestBlock.cs               - Individual quest step/block definition
    Extensions/
      QuestBlockExtension.cs    - Fluent API for building quest blocks
    LightQuests/
      LightQuestManager.cs      - Dynamic generation of board quests
      LightQuestQuest.cs        - Light quest implementation
    MainQuests/
      Mq030260_HopesBitterEnd.cs - Example hardcoded main quest (Season 3.0)
    ProgressWork/
      WorldQuestClearedProgressWork.cs - Deferred work for quest completion

  Characters/
    QuestManager.cs             - Global quest registry and utilities

  Scripting/Interfaces/
    IQuest.cs                   - Quest scripting interface
    ILightQuestRewardMixin.cs   - Mixin for light quest rewards

  Handler/
    Quest*.cs                   - 50+ quest-related packet handlers

Arrowgene.Ddon.Scripts/scripts/quests/
  msq/                          - Main story quests (by season)
  personal/                     - Personal/tutorial quests
  vocation/                     - Vocation-specific quests
  trials/                       - Area trial quests
  exm/                          - Extreme missions
  seasonal_events/              - Time-limited event quests
  world_manage/                 - World management quests
```

## Quest Types

The system supports multiple quest types defined in `QuestType.cs`:

| Type | Value | Description |
|------|-------|-------------|
| All | 0 | Query filter for all quests |
| Light | 1 | Board quests (Hunt/Delivery) |
| Set/World | 2 | World quests in specific areas |
| Main | 3 | Main story quests |
| Tutorial | 4 | Tutorial/personal quests |
| Limited | 5 | Limited-time quests |
| CycleContents | 6 | Cycling content |
| CycleContentsQuest | 7 | Cycling content quests |
| WorldManage | 8 | World management quests |
| TimeGain/ExtremeMission | 9 | Extreme missions |
| WildHunt | 15 | Wild hunt quests |

## Core Architecture

### 1. Quest Base Class (`Quest.cs`)

The abstract `Quest` class (~1400 lines) provides:

- **Quest Identity**: `QuestId`, `QuestScheduleId`, `QuestType`, `QuestAreaId`
- **Configuration**: Level requirements, item rank, rewards, order conditions
- **State Management**: Process states, enemy groups, locations
- **Reward System**: Items, wallet (gold/rift), experience points, area points
- **Serialization**: Methods to convert to various `CData*` structures for packets
- **State Machine**: `StateMachineExecute()` for progression handling

Key properties:
```csharp
public bool IsPersonal => QuestType == QuestType.Light || QuestType == QuestType.Tutorial;
public bool IsDistributionTimed => DistributionEnd.UtcTicks != 0;
```

### 2. Quest State Manager (`QuestStateManager.cs`)

Manages active quest state with two implementations:

- **SharedQuestStateManager**: For party-shared quests (Main, World, EXM)
- **SoloQuestStateManager**: For personal quests (Light, Tutorial)

Key state tracking:
```csharp
public class QuestState {
    public QuestId QuestId;
    public uint QuestScheduleId;
    public QuestType QuestType;
    public QuestProgressState State;  // Unknown, Accepted, InProgress, Checkpoint, Complete
    public uint Step;
    public Dictionary<ushort, QuestProcessState> ProcessState;
    public Dictionary<StageLayoutId, Dictionary<uint, List<InstancedEnemy>>> QuestEnemies;
    public Dictionary<ItemId, QuestDeliveryRecord> DeliveryRecords;
    public Dictionary<EnemyUIId, QuestEnemyHuntRecord> HuntRecords;
}
```

### 3. Quest Process Model

Quests use a **Process/Block** model:

- **Process**: A sequence of blocks (numbered 0, 1, 2...)
- **Block**: Individual quest steps with check/result commands
- Multiple processes can run in parallel (e.g., main objective + side objectives)

```
Quest
  Process 0 (Main)
    Block 1: Talk to NPC -> Accept quest
    Block 2: Kill enemies -> Checkpoint
    Block 3: Return to NPC -> Complete
  Process 1 (Side objective)
    Block 1: Wait for flag
    Block 2: Kill bonus enemies
    Block 3: End process
```

### 4. Scripting Interface (`IQuest.cs`)

The abstract `IQuest` class provides a clean scripting API:

```csharp
public abstract class IQuest
{
    // Required overrides
    public abstract QuestType QuestType { get; }
    public abstract QuestId QuestId { get; }
    public abstract ushort RecommendedLevel { get; }
    public abstract byte MinimumItemRank { get; }
    public abstract bool IsDiscoverable { get; }

    // Lifecycle methods
    protected virtual void InitializeState() { }
    protected virtual void InitializeRewards() { }
    protected virtual void InitializeEnemyGroups() { }
    protected virtual void InitializeBlocks() { }

    // Helper methods
    public void AddFixedItemReward(ItemId itemId, ushort amount);
    public void AddPointReward(PointType pointType, uint amount);
    public void AddWalletReward(WalletType walletType, uint amount);
    public void AddEnemies(uint id, StageInfo stageInfo, uint groupId, ...);
    public QuestProcess AddNewProcess(ushort processNo);
}
```

## Quest Block Extensions

The `QuestBlockExtension.cs` provides a fluent API for building quest blocks:

```csharp
var process0 = AddNewProcess(0);
process0.AddNpcTalkAndOrderBlock(Stage.TheWhiteDragonTemple0, NpcId.Leo, 12345);
process0.AddTalkToNpcBlock(QuestAnnounceType.Accept, Stage.HidellPlains, NpcId.Guard, 12346);
process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, enemyGroupId: 1);
process0.AddDestroyGroupBlock(QuestAnnounceType.Update, enemyGroupId: 1);
process0.AddProcessEndBlock(isTerminal: true);
```

Available block types:
- `AddNpcTalkAndOrderBlock` - Talk to NPC to start quest
- `AddTalkToNpcBlock` - Talk to NPC objective
- `AddDiscoverGroupBlock` - Discover enemy group
- `AddDestroyGroupBlock` - Kill enemy group
- `AddDeliverItemsBlock` - Deliver items to NPC
- `AddSceHitInBlock` - Enter scene area trigger
- `AddOmInteractEventBlock` - Interact with object
- `AddPlayEventBlock` - Play cutscene
- `AddStageJumpBlock` - Teleport to stage
- `AddIsQuestClearBlock` - Check quest completion
- `AddKillTargetEnemiesBlock` - Kill specific enemy types
- `AddExtendTimeBlock` - Extend mission timer

## Main Story Quest Implementation

Main story quests are implemented as C# scripts in `scripts/quests/msq/`. Example from Season 3.0:

```csharp
public class ScriptedQuest : IQuest
{
    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => (QuestId)30010;  // MSQ ID
    public override ushort RecommendedLevel => 60;

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);
        process0.AddNpcTalkAndOrderBlock(Stage.WhiteDragonTemple, NpcId.Leo, msgId);
        process0.AddTalkToNpcBlock(QuestAnnounceType.Accept, Stage.Battlefield, NpcId.Knight, msgId);
        process0.AddPlayEventBlock(QuestAnnounceType.Update, Stage.Battlefield, eventId: 100, startPos: 0);
        // ... more blocks
    }
}
```

**Status**: Only partial implementation exists. Season 3.0+ has 7 quests scripted (`q00030010` - `q00030070`). Season 1.0 has minimal coverage.

## World Quest Implementation

World quests are area-specific quests organized by `QuestAreaId`:

```csharp
public override QuestType QuestType => QuestType.World;
public override QuestAreaId QuestAreaId => QuestAreaId.HidellPlains;
```

The `QuestManager` tracks world quests by area:
```csharp
private static Dictionary<QuestAreaId, HashSet<QuestId>> gWorldQuests;
```

World quests support:
- Area-specific availability
- Discovery mechanics
- Shared party progression
- Variant implementations (same QuestId, different content)

**Status**: 485 quest JSON assets exist, with many world quests implemented via JSON configuration.

## Light Quest System (Board Quests)

The most sophisticated subsystem, handling dynamically generated board quests.

### LightQuestManager

The `LightQuestManager` provides:

1. **Quest Generation**: Procedurally creates Hunt and Delivery quests
2. **Area Summary**: Tracks enemies and items by area for generation
3. **Decay Handling**: Manages quest expiration (default 1 day)
4. **Database Integration**: Stores generated quests in `ddon_light_quest_records`

```csharp
public Quest GenerateQuestFromRecord(LightQuestRecord record)
{
    LightQuestInfo info = LightQuestId.FromQuestId(record.QuestId);
    return info.Type switch
    {
        LightQuestType.Hunt => new LightQuestHuntQuest(record).GenerateQuest(Server),
        LightQuestType.Delivery => new LightQuestDeliveryQuest(record).GenerateQuest(Server),
    };
}
```

### Hunt Quests

Generated from enemy spawn data across all areas:
- Parses `EnemySpawnAsset` for valid targets
- Excludes passive creatures (rabbits, pigs, etc.)
- Considers enemy difficulty for boss multipliers
- Rewards scaled by level and count

### Delivery Quests

Generated from gathering spots and enemy drops:
- Materials (metal, ore, cloth, etc.) from subcategories
- Excludes craftable items
- Weighted by drop rates

### Quest Schedule IDs

Light quests use a special ID scheme:
```csharp
public static bool IsLightQuestScheduleId(uint scheduleId)
{
    return QuestScheduleId.GetType(scheduleId) == QuestScheduleId.ScheduleIdType.Board;
}
```

## Extreme Mission Implementation

Extreme missions (EXM) are complex timed content with:

```csharp
protected override void InitializeState()
{
    MissionParams.Group = ExtremeMissionUtils.Group.Alan;
    MissionParams.MinimumMembers = 1;
    MissionParams.MaximumMembers = 8;
    MissionParams.PlaytimeInSeconds = 1800;  // 30 minutes
    MissionParams.MaxPawns = 7;
    MissionParams.LootDistribution = QuestLootDistribution.TimeBased;
}
```

Features:
- Multiple parallel enemy groups (main targets + assault waves)
- Time-based rewards and extensions
- Raid point scoring
- Boss mechanics with named parameters

Example: `q50300004.csx` (Battle for Gritten Fort) has 24 enemy groups across multiple phases.

## Tutorial Quest Implementation

Tutorial quests are personal (solo) quests:

```csharp
public override QuestType QuestType => QuestType.Tutorial;
public override StageInfo StageInfo => Stage.TheWhiteDragonTemple0;  // Required for tutorials
```

Features:
- Content release unlocks (`ContentsRelease`)
- Tutorial flag system
- Order conditions based on other quest completion

## Quest Handlers

Over 50 handlers in `Handler/` manage quest packets:

| Handler | Purpose |
|---------|---------|
| `QuestQuestOrderHandler` | Accept quest |
| `QuestQuestProgressHandler` | Advance quest state |
| `QuestCancelHandler` | Cancel active quest |
| `QuestDeliverItemHandler` | Submit delivery items |
| `QuestGetMainQuestListHandler` | Get main quest list |
| `QuestGetLightQuestListHandler` | Get board quests |
| `QuestSetPriorityQuestHandler` | Set tracked quests |
| `QuestPlayStartHandler` | Start mission content |
| `QuestPlayEndHandler` | Complete mission content |

## Check and Result Commands

The system uses a command-based approach for conditions and effects:

### Check Commands (Conditions)
```csharp
QuestManager.CheckCommand.TalkNpc(stageNo, npcId)
QuestManager.CheckCommand.DieEnemy(stageNo, groupNo, setNo)
QuestManager.CheckCommand.SceHitIn(stageNo, sceNo)
QuestManager.CheckCommand.HaveItem(itemId, itemNum)
QuestManager.CheckCommand.DeliverItem(itemId, itemNum, npcId)
QuestManager.CheckCommand.EmDieLight(enemyId, level, count)
QuestManager.CheckCommand.IsMainQuestClear(questId)
QuestManager.CheckCommand.EventEnd(stageNo, eventNo)
```

### Result Commands (Effects)
```csharp
QuestManager.ResultCommand.SetAnnounce(type, param)
QuestManager.ResultCommand.QstLayoutFlagOn(flagNo)
QuestManager.ResultCommand.HandItem(itemId, amount)
QuestManager.ResultCommand.EventExec(stageNo, eventNo)
```

## Missing Features and Quest Types

### Partially Implemented

1. **Main Story Quests**: Only Season 3.0 has scripted coverage; earlier seasons rely on JSON
2. **Wild Hunt Quests**: Framework exists (`QuestType.WildHunt = 15`) but no implementations found
3. **Clan Quests**: Referenced in code but minimal implementation

### Not Implemented / TODO Items

From code comments and structure:

1. **First-clear rewards**: `// TODO: Support first clear only rewards`
2. **Assist rewards**: `// TODO: Support adding reward for helping other players`
3. **Clear count rewards**: `// TODO: Support rewards based on clear count`
4. **All enemies killed rewards**: `// TODO: Support rewards based on all enemies killed`
5. **Clan Quest rotation**: Light quest style DB management for clan quests
6. **Cycle Contents**: Framework exists but no active content

### Known Bad Quest Schedule IDs

The system tracks known invalid IDs that shouldn't trigger errors:
```csharp
private static readonly HashSet<uint> KnownBadQuestScheduleIds = new HashSet<uint>()
{
    25077, 43645, 43646, 47734, 47735, 47736, 47737, 47738, 47739,
    49692, 77644, 151381, 208640, 233576, 259411, 259412, 287378, 315624
};
```

## Data Flow

### Quest Acceptance
```
Client -> C2SQuestQuestOrderReq -> QuestQuestOrderHandler
  -> QuestManager.GetQuestByScheduleId()
  -> QuestStateManager.AddNewQuest()
  -> Database.InsertQuestProgress()
  -> S2CQuestQuestOrderRes -> Client
```

### Quest Progression
```
Client -> C2SQuestQuestProgressReq -> QuestQuestProgressHandler
  -> quest.StateMachineExecute()
  -> QuestStateManager.UpdateProcessState()
  -> Database.UpdateQuestProgress()
  -> S2CQuestQuestProgressRes -> Client (+ NTC to party)
```

### Quest Completion
```
StateMachineExecute returns Complete state
  -> QuestStateManager.DistributeQuestRewards()
  -> QuestStateManager.CompleteQuestProgress()
  -> Database.InsertCompletedQuest()
  -> S2CQuestCompleteNtc -> Client/Party
```

## Configuration

### Quest Assets
- Location: `Arrowgene.Ddon.Shared/Files/Assets/quests/`
- Format: JSON files named `q<questid>.json`
- Count: 485 quest definitions

### Light Quest Configuration
- Generator assets define board quest parameters
- Mixin scripts calculate rewards
- Database stores active rotations

## Recommendations for Further Development

1. **Complete MSQ Coverage**: Script remaining main story quests for seasons 1.0-2.x
2. **Wild Hunt Implementation**: Add wild hunt quest content
3. **Reward Enhancements**: Implement first-clear and clear-count reward systems
4. **Event System**: Flesh out seasonal event quest framework
5. **Documentation**: Add inline documentation to core quest classes
6. **Testing**: Create unit tests for quest state machine logic

## File Reference

| File | Lines | Purpose |
|------|-------|---------|
| `Quest.cs` | ~1400 | Base quest implementation |
| `QuestStateManager.cs` | ~1234 | State management |
| `QuestManager.cs` | ~3000+ | Global registry and utilities |
| `IQuest.cs` | ~350 | Scripting interface |
| `QuestBlockExtension.cs` | ~590 | Block building API |
| `LightQuestManager.cs` | ~643 | Board quest generation |
| `GenericQuest.cs` | ~800 | JSON-based quest loading |
