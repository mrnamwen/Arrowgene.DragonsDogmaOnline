/**
 * @brief The Royal Family Mausoleum
 * @desc Season 3.3 quest. Follow Prince Nedo to the Royal Family Mausoleum
 *       in the Urteca Mountains and search for him among the royal tombs.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.TheRoyalFamilyMausoleum;
    public override ushort RecommendedLevel => 95;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.TheDreadfulPassage;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint TombGuardian = 917;            // Tomb (prefix)
        public const uint AncientUndead = 2521;          // Ancient Undead
        public const uint AncientGhost = 2520;           // Ancient Ghost
        public const uint MausoleumGuardian = 1146;      // Foundation Guardian (boss-type)
    }

    private class QstLayoutFlag
    {
        // Lookout Castle (st0451)
        public const uint LookoutCastleNpcs0 = 6800;     // Bertha and crew for initial briefing

        // Urteca Mountains (st0465)
        public const uint UrtecaMountainsNpcs = 6801;    // Nedo leading towards mausoleum

        // The Royal Family Mausoleum (st0582)
        public const uint MausoleumNpcs0 = 6802;         // Nedo at entrance
        public const uint MausoleumNpcs1 = 6803;         // Meirova at tomb center
        public const uint MausoleumOMs = 6804;           // Barriers, tomb decorations

        // Lookout Castle post-quest (st0451)
        public const uint LookoutCastleNpcs1 = 6805;     // Post-mission debrief NPCs
    }

    private class MyQstFlag
    {
        public const uint SpawnTombGuardians = 1;
        public const uint EndTombGuardians = 2;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(95));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.NedosTrail));
    }

    protected override void InitializeRewards()
    {
        AddPointReward(PointType.ExperiencePoints, 900000);
        AddWalletReward(WalletType.Gold, 100000);
        AddWalletReward(WalletType.RiftPoints, 10000);

        AddFixedItemReward(ItemId.RoyalCrestMedalUrtecaDistrict, 5);
        AddFixedItemReward(ItemId.UnappraisedCloudTrinketGeneral, 2);
        AddFixedItemReward(ItemId.ApUrtecaMountains, 500);
    }

    protected override void InitializeEnemyGroups()
    {
        // Group 0: First encounter on the way to mausoleum - undead ambush
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.UrtecaMountains, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonKnight, 95, 0)
                .SetNamedEnemyParams(NamedParamId.TombGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonWarrior, 95, 1)
                .SetNamedEnemyParams(NamedParamId.TombGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonWarrior, 95, 2)
                .SetNamedEnemyParams(NamedParamId.TombGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.GhostMail, 95, 3)
                .SetNamedEnemyParams(NamedParamId.AncientGhost),
            LibDdon.Enemy.CreateAuto(EnemyId.GhostMail, 95, 4)
                .SetNamedEnemyParams(NamedParamId.AncientGhost),
        });

        // Group 1: Second encounter inside mausoleum - stronger undead
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.TheRoyalFamilyMausoleum, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.Wight0, 95, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.AncientUndead)
                .SetHpRate(150)
                .SetAttackRate(120),
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonSorcerer0, 95, 1)
                .SetNamedEnemyParams(NamedParamId.TombGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonSorcerer0, 95, 2)
                .SetNamedEnemyParams(NamedParamId.TombGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonMage0, 95, 3)
                .SetNamedEnemyParams(NamedParamId.TombGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.UndeadMale, 93, 4)
                .SetNamedEnemyParams(NamedParamId.AncientUndead),
            LibDdon.Enemy.CreateAuto(EnemyId.UndeadMale, 93, 5)
                .SetNamedEnemyParams(NamedParamId.AncientUndead),
        });

        // Group 2: Boss encounter - Mausoleum Guardian (protecting the royal tombs)
        AddEnemies(EnemyGroupId.Encounter + 2, Stage.TheRoyalFamilyMausoleum, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonCyclops, 95, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.MausoleumGuardian)
                .SetHpRate(200)
                .SetAttackRate(130)
                .SetDefenceRate(120),
        });

        // Group 3: Add spawns during boss fight - tomb defenders
        AddEnemies(EnemyGroupId.Encounter + 3, Stage.TheRoyalFamilyMausoleum, 3, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonKnight, 93, 0)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(45, 60)
                .SetNamedEnemyParams(NamedParamId.TombGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonKnight, 93, 1)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(45, 60)
                .SetNamedEnemyParams(NamedParamId.TombGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.GhostMail, 93, 2)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(45, 60)
                .SetNamedEnemyParams(NamedParamId.AncientGhost),
            LibDdon.Enemy.CreateAuto(EnemyId.GhostMail, 93, 3)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(45, 60)
                .SetNamedEnemyParams(NamedParamId.AncientGhost),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Head to the Lookout Castle
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 23000);
        process0.AddIsStageNoBlock(QuestAnnounceType.Accept, Stage.LookoutCastle1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs0);

        // Step 2: Head to the Royal Family Mausoleum in the Urteca Mountains
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.UrtecaMountainsNpcs);

        // Step 3: Defeat the enemy encountered (first encounter on the way)
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 0);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false);

        // Step 4: Follow Nedo to the royal tombs
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheRoyalFamilyMausoleum)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MausoleumNpcs0);

        // Step 5: Search for Nedo at the Royal Family Mausoleum
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheRoyalFamilyMausoleum, 1, 0, 0);

        // Step 6: Defeat the enemy encountered (inside mausoleum)
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 1);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false);

        // Step 7: Head to the center of the tomb in search of Nedo
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheRoyalFamilyMausoleum, 2, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MausoleumOMs);

        // Pre-boss cutscene: Mausoleum guardian awakens
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.TheRoyalFamilyMausoleum, 0, 0, QuestJumpType.None, Stage.Invalid);

        // Boss fight: Defeat the Mausoleum Guardian
        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnTombGuardians)
            .AddCheckCmdEmHpLess(Stage.TheRoyalFamilyMausoleum, 2, 0, 50);

        // Post-boss cutscene: Find clues about Nedo's destination
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.TheRoyalFamilyMausoleum, 1, 0, QuestJumpType.None, Stage.Invalid)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndTombGuardians)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MausoleumNpcs1);

        // Step 8: Speak with Meirova
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheRoyalFamilyMausoleum, 0, 0, NpcId.Meirova0, 23010);

        // Step 9: Return to the Lookout Castle
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs1);

        // Speak with Bertha at Lookout Castle
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.Update, Stage.LookoutCastle1, 0, 0, NpcId.Bertha, 23020);

        // Step 10: Return to Lestania and report to the White Dragon
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 23030);

        process0.AddProcessEndBlock(true);

        // Process 1: Handle add spawns during boss fight
        var process1 = AddNewProcess(1);
        process1.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.SpawnTombGuardians);
        process1.AddSpawnGroupBlock(QuestAnnounceType.None, EnemyGroupId.Encounter + 3)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.EndTombGuardians);
        process1.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 2,
            EnemyGroupId.Encounter + 3
        ]);
        process1.AddProcessEndBlock(false);
    }
}

return new ScriptedQuest();
