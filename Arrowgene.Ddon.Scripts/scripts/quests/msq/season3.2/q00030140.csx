/**
 * @brief The Road to the Royal Capital
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.TheRoadToTheRoyalCapital;
    public override ushort RecommendedLevel => 89;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.RallyTheTroops;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint DemonArmyTroop = 1776; // Demon Army Troop
    }

    private class QstLayoutFlag
    {
        // Lookout Castle (st0451)
        public const uint LookoutCastleNpcs0 = 5610; // Meirova, Gillian, Nedo

        // Eli Guard Tower (st0636)
        public const uint EliGuardTowerNpcs0 = 5611; // Gillian
        public const uint EliGuardTowerNpcs1 = 5612; // Nedo, Meirova
        public const uint EliGuardTowerAreaMaster = 5613; // Area Master
    }

    private class MyQstFlag
    {
        public const uint SecondWave = 1;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(89));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.TheBattleOfLookoutCastle));
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
        // First wave at Eli Guard Tower - Dwarf Orcs (Ice/Dark weakness)
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.EliGuardTower, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SquadLeaderDwarfOrc, 89, 0)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 89, 1)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 89, 2)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.BluntSoldierDwarfOrc, 89, 3)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.BluntSoldierDwarfOrc, 89, 4)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.RangedSoldierDwarfOrc, 89, 5)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
        });

        // Second wave - remaining troops in the tower
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.EliGuardTower, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SquadLeaderDwarfOrc, 89, 0)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 89, 1)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 89, 2)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.HeavySoldierDwarfOrc, 89, 3)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.RangedSoldierDwarfOrc, 89, 4)
                .SetNamedEnemyParams(NamedParamId.DemonArmyTroop),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);
        // Step 1: Speak with Meirova at Lookout Castle
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 21900)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.Accept, Stage.LookoutCastle1, 0, 0, NpcId.Meirova0, 21901)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs0);

        // Step 2: Rendezvous with Gillian in the Enslaved Beasts' pen
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 0, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EliGuardTowerNpcs0);

        // Step 3: Speak with Gillian
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 0, 0, NpcId.Gillian0, 21902);

        // Step 4: Navigate through the cave and head towards Eli Guard Tower
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower);

        // Step 5: Defeat the Demon Army occupying Eli Guard Tower
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 0);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SecondWave);

        // Step 6: Defeat the remaining troops in the tower
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 1);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false);

        // Step 7: Rendezvous with Gillian
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 0, 100, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.EliGuardTowerNpcs0);

        // Step 8: Activate the portcrystal
        process0.AddRawBlock(QuestAnnounceType.CheckpointAndUpdate)
            .AddCheckCmdIsReleaseWarpPointAnyone(73); // Eli Guard Tower portcrystal

        // Step 9: Speak with Gillian
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 0, 1, NpcId.Gillian0, 21903);

        // Step 10: Welcome the arrival of Nedo and Meirova
        process0.AddPlayEventBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EliGuardTowerNpcs1);

        // Step 11: Speak with Nedo
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 0, 2, NpcId.Nedo0, 21904);

        // Step 12: Speak with the Area Master in the basement of the guard tower
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 1, 0, NpcId.Mustafa, 21905)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EliGuardTowerAreaMaster);

        // Step 13: Return to Lestania
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 21906)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.LookoutCastleNpcs0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.EliGuardTowerNpcs1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.EliGuardTowerAreaMaster);
        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
