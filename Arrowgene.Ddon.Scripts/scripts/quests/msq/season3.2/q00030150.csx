/**
 * @brief Rally the Troops
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.RallyTheTroops;
    public override ushort RecommendedLevel => 90;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.AttackOnTheRoyalCapital;

    private class EnemyGroupId
    {
        public const uint Encounter0 = 10;
        public const uint Encounter1 = 11;
        public const uint Encounter2 = 12;
    }

    private class NamedParamId
    {
        public const uint AncestorOrcRaid0 = 1780; // Ancestor Orc Raid
        public const uint AncestorOrcRaid1 = 1781; // Ancestor Orc Raid
        public const uint AncestorOrcRaid2 = 1782; // Ancestor Orc Raid
    }

    private class QstLayoutFlag
    {
        // Lookout Castle (st0451)
        public const uint LookoutCastleGillian = 7000; // Gillian

        // Eli Guard Tower (st0636)
        public const uint EliGuardTowerYuri = 7001; // Yuri
        public const uint EliGuardTowerGillian = 7002; // Gillian

        // Megadosys Plateau (st0133)
        public const uint MegadosysKirsty = 7003; // Kirsty
        public const uint MegadosysSoldier = 7004; // Assisting Soldier
        public const uint MegadosysNpc = 7005; // Assisting Person
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(90));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.TheRoadToTheRoyalCapital));
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
        // First encounter - searching for escaped people
        AddEnemies(EnemyGroupId.Encounter0, Stage.MegadosysPlateau, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrc, 90, 0)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid0),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrc, 90, 1)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid0),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrcFighter, 90, 2)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid0),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrcFighter, 90, 3)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid0),
            LibDdon.Enemy.CreateAuto(EnemyId.CaptainAncestorOrc, 90, 4, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid0),
        });

        // Second encounter - remaining people search
        AddEnemies(EnemyGroupId.Encounter1, Stage.MegadosysPlateau, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrc, 90, 0)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid1),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrc, 90, 1)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid1),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrc, 90, 2)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid1),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrcFighter, 90, 3)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid1),
            LibDdon.Enemy.CreateAuto(EnemyId.CaptainAncestorOrc, 90, 4, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid1),
        });

        // Third encounter - final remaining people search
        AddEnemies(EnemyGroupId.Encounter2, Stage.MegadosysPlateau, 3, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrc, 90, 0)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid2),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrcFighter, 90, 1)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid2),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrcFighter, 90, 2)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid2),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrcFighter, 90, 3)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid2),
            LibDdon.Enemy.CreateAuto(EnemyId.CaptainAncestorOrc, 90, 4, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.AncestorOrcRaid2),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Head to the Lookout Castle
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 22000)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.MephiteTravelersInn.Nayajiku)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // Step 2: Speak with Gillian
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.Accept, Stage.LookoutCastle1, 0, 0, NpcId.Gillian0, 22001)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleGillian);

        // Step 3: Obtain information from Yuri at Eli Guard Tower
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 0, 0, NpcId.Yuri, 22002)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EliGuardTowerYuri);

        // Step 4: Search for the people who escaped from Megado
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter0);

        // Step 5: Defeat the encountered enemy
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter0, resetGroup: false);

        // Step 6: Speak with Kirsty
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadosysPlateau, 0, 0, NpcId.Kirsty0, 22003)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadosysKirsty);

        // Step 7: Search for the remaining people
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter1);

        // Step 8: Defeat the encountered enemy
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter1, resetGroup: false);

        // Step 9: Speak with the assisting soldier
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadosysPlateau, 0, 1, NpcId.LiberationArmySoldier0, 22004)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadosysSoldier);

        // Step 10: Search for the remaining people
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter2);

        // Step 11: Defeat the encountered enemy
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter2, resetGroup: false);

        // Step 12: Speak with the assisting person
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadosysPlateau, 0, 2, NpcId.LiberationArmySoldier1, 22005)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadosysNpc);

        // Step 13: Head to Gillian waiting in Eli Guard Tower
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 0, 1, NpcId.Gillian0, 22006)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EliGuardTowerGillian);

        // Step 14: Return to the Lookout Castle and report to Meirova
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle1, NpcId.Meirova0, 22007)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.MephiteTravelersInn.Nayajiku);

        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
