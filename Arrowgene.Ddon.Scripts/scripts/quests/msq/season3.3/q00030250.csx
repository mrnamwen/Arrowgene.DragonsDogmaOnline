/**
 * @brief The Relics of the First King
 * @desc Season 3.3 quest to search for the Ring of the First King.
 *       The Arisen must investigate locations around Firefall Mountain,
 *       Megado Residential Level, and Eastern Urteca Cave.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.TheRelicsOfTheFirstKing;
    public override ushort RecommendedLevel => 97;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.HopesBitterEnd;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint CaveGuardian = 1850;        // Cave Guardian
        public const uint RelicProtector = 1851;      // Relic Protector
        public const uint GhostOfTheFirstKing = 2557; // The Ghost of the First King
    }

    private class QstLayoutFlag
    {
        // Firefall Mountain Campsite (st0595)
        public const uint FirefallMountainNpcs0 = 7700;     // Gillian and Cyril at entrance

        // Fortress City Megado Residential Level (st0486)
        public const uint MegadoResidentialNpcs = 7701;     // Gillian for information

        // Cave of Rest - Eastern Urteca (st0507)
        public const uint CaveOfRestOMs = 7702;             // Ring location markers
        public const uint CaveOfRestNpcs = 7703;            // NPCs in cave
    }

    private class MyQstFlag
    {
        public const uint SpawnCaveEnemies = 1;
        public const uint EndCaveEnemies = 2;
        public const uint RingObtained = 3;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(97));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.TheDreadfulPassage));
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
        // Group 0: Enemies encountered in the cave
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.CaveofRest, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedBehemoth, 97, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.CaveGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedWarg, 95, 1)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.RelicProtector),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedWarg, 95, 2)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.RelicProtector),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedWarg, 95, 3)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.RelicProtector),
        });

        // Group 1: Additional enemies guarding the ring location
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.CaveofRest, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 97, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.RelicProtector)
                .SetHpRate(200)
                .SetAttackRate(120)
                .SetDefenceRate(110),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedWarg, 95, 1)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.RelicProtector),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedWarg, 95, 2)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.RelicProtector),
        });

        // Group 2: Ghost of the First King (optional boss encounter)
        AddEnemies(EnemyGroupId.Encounter + 2, Stage.CaveofRest, 3, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SkullLord, 97, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.GhostOfTheFirstKing)
                .SetHpRate(180)
                .SetAttackRate(130)
                .SetDefenceRate(120)
                .SetMagicAttackRate(140),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Accept quest from Joseph - Head to the gate to enter Firefall Mountain
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 25000)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // Step 2: Head to the entrance of Firefall Mountain
        process0.AddIsStageNoBlock(QuestAnnounceType.Accept, Stage.FirefallMountainCampsite)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FirefallMountainNpcs0);

        // Step 3: Head to the Megado Residential Level to rendezvous with Gillian
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoResidentialLevel0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadoResidentialNpcs);

        // Step 4: Examine the mansion with the information obtained from Gillian
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoResidentialLevel0, 0, 0, NpcId.Gillian0, 25010);

        // Step 5: Search for the next location, relying on the Adventurer's message
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoResidentialLevel0, 1, 0, 0);

        // Step 6: Head to the cave in eastern Urteca Mountains
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.CaveofRest)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.CaveOfRestOMs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.CaveOfRestNpcs);

        // Step 7: Search for the Ring of The Relics of the First King
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 0);

        // Step 8: Defeat the enemy encountered
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false);

        // Step 9: Search for the Ring of The Relics of the First King (continue deeper)
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 1)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnCaveEnemies);

        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndCaveEnemies);

        // Cutscene: Discover the ring and encounter the Ghost of the First King
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.CaveofRest, 0, 0, QuestJumpType.None, Stage.Invalid)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.RingObtained);

        // Step 10: Report to Cyril at the entrance to Firefall Mountain
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FirefallMountainCampsite, 0, 0, NpcId.Cyril, 25020)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.CaveOfRestOMs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.CaveOfRestNpcs);

        // Step 11: Return to Lestania and report to Joseph
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 25030)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        process0.AddProcessEndBlock(true);

        // Process 1: Handle optional Ghost of the First King encounter
        var process1 = AddNewProcess(1);
        process1.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.RingObtained);
        process1.AddSpawnGroupBlock(QuestAnnounceType.None, EnemyGroupId.Encounter + 2)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.RingObtained);
        process1.AddDestroyGroupBlock(QuestAnnounceType.None, EnemyGroupId.Encounter + 2, resetGroup: false);
        process1.AddProcessEndBlock(false);
    }
}

return new ScriptedQuest();
