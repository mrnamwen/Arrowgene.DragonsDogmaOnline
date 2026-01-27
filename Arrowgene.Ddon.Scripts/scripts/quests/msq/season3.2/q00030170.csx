/**
 * @brief An Omen of Destruction
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.AnOmenOfDestruction;
    public override ushort RecommendedLevel => 92;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.ABriefDragonForce;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint DragonForceDisruptor0 = 1780; // Dragon Force Disruptor
        public const uint DragonForceDisruptor1 = 1781; // Dragon Force Disruptor
        public const uint DragonForceDisruptor2 = 1782; // Dragon Force Disruptor
        public const uint CursedDragon = 1783; // Cursed Dragon
    }

    private class QstLayoutFlag
    {
        // Temple of Purification (st0408)
        public const uint TempleNpcs0 = 6700; // NPCs at temple entrance
        public const uint TempleNpcs1 = 6701; // NPCs after first fight
        public const uint TempleNpcs2 = 6702; // NPCs after second fight
        public const uint TempleNpcs3 = 6703; // NPCs after third fight

        // Vortex of Stagnation (st0430)
        public const uint VortexNpcs = 6710; // NPCs in vortex area

        // Mayleaf's Bedroom (st0213)
        public const uint MayleafsBedroom = 6720; // Mayleaf in bedroom
    }

    private class MyQstFlag
    {
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(92));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.AttackOnTheRoyalCapital));
    }

    protected override void InitializeRewards()
    {
        AddPointReward(PointType.ExperiencePoints, 800000);
        AddWalletReward(WalletType.Gold, 90000);
        AddWalletReward(WalletType.RiftPoints, 9000);

        AddFixedItemReward(ItemId.RoyalCrestMedalMegadosysDistrict, 5);
        AddFixedItemReward(ItemId.UnappraisedWaterTrinketGeneral, 2);
        AddFixedItemReward(ItemId.ApMegadosysPlateau, 500);
    }

    protected override void InitializeEnemyGroups()
    {
        // First demon group (Dragon Force disruptor 1)
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.TempleofPurification, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.Wight0, 92, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor0),
            LibDdon.Enemy.CreateAuto(EnemyId.GhostMail, 92, 1)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor0),
            LibDdon.Enemy.CreateAuto(EnemyId.GhostMail, 92, 2)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor0),
            LibDdon.Enemy.CreateAuto(EnemyId.Ghost, 92, 3)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor0),
            LibDdon.Enemy.CreateAuto(EnemyId.Ghost, 92, 4)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor0),
        });

        // Second demon group (Dragon Force disruptor 2)
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.TempleofPurification, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.Wight0, 92, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor1),
            LibDdon.Enemy.CreateAuto(EnemyId.MiseryGhost, 92, 1)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor1),
            LibDdon.Enemy.CreateAuto(EnemyId.MiseryGhost, 92, 2)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor1),
            LibDdon.Enemy.CreateAuto(EnemyId.GhostMail, 92, 3)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor1),
            LibDdon.Enemy.CreateAuto(EnemyId.GhostMail, 92, 4)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor1),
        });

        // Third demon group (Dragon Force disruptor 3)
        AddEnemies(EnemyGroupId.Encounter + 2, Stage.TempleofPurification, 3, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.Wight0, 92, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor2),
            LibDdon.Enemy.CreateAuto(EnemyId.GrudgeGhost, 92, 1)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor2),
            LibDdon.Enemy.CreateAuto(EnemyId.GrudgeGhost, 92, 2)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor2),
            LibDdon.Enemy.CreateAuto(EnemyId.MiseryGhost, 92, 3)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor2),
            LibDdon.Enemy.CreateAuto(EnemyId.MiseryGhost, 92, 4)
                .SetNamedEnemyParams(NamedParamId.DragonForceDisruptor2),
        });

        // Final boss - Cursed Dragon in Vortex of Stagnation
        // Boss has back wounds that must be destroyed, then chest core
        AddEnemies(EnemyGroupId.Encounter + 3, Stage.VortexofStagnation0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.CursedDragon, 92, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.CursedDragon),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Speak with Joseph
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 22000)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.MephiteTravelersInn.Nayajiku)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // Step 2: Head to the Temple of Purification
        process0.AddPartyGatherBlock(QuestAnnounceType.Accept, Stage.TempleofPurification, 0, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.TempleNpcs0);

        // Step 3: Search for a pseudo-dragon
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TempleofPurification);

        // Step 4: Examine the cause of Dragon Force imbalance
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 0);

        // Step 5: Defeat the demons disrupting the Dragon Force (3 remaining)
        process0.AddDestroyGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 0, resetGroup: false);

        // Step 6: Examine the location where the enemy was defeated
        process0.AddRawBlock(QuestAnnounceType.CheckpointAndUpdate)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.TempleNpcs0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.TempleNpcs1);

        // Step 7: Defeat the demons disrupting the Dragon Force (2 remaining)
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 1);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false);

        // Step 8: Examine the location where the enemy was defeated
        process0.AddRawBlock(QuestAnnounceType.CheckpointAndUpdate)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.TempleNpcs1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.TempleNpcs2);

        // Step 9: Defeat the demons disrupting the Dragon Force (1 remaining)
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 2);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2, resetGroup: false);

        // Step 10: Examine the location where the enemy was defeated
        process0.AddPlayEventBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TempleofPurification, 0, 0, QuestJumpType.None, Stage.Invalid)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.TempleNpcs2)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.TempleNpcs3);

        // Step 11: Ride on the revived Dragon Force and head to the Vortex of Stagnation
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexNpcs);

        // Step 12: Proceed further and search for the pseudo-dragon
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 3);

        // Step 13: Defeat the encountered demons (Cursed Dragon boss - destroy back wounds then chest core)
        process0.AddDestroyGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 3, resetGroup: false);

        // Step 14: Examine the location where the enemy was defeated
        process0.AddPlayEventBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation0, 5, 0, QuestJumpType.None, Stage.Invalid);

        // Step 15: Head to Mayleaf's room in the White Dragon Temple
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MayleafsBedroom, 0, 0, NpcId.Mayleaf0, 22010)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MayleafsBedroom);

        // Step 16: Report to Joseph in the Audience Chamber
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 22020)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.MephiteTravelersInn.Nayajiku);

        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
