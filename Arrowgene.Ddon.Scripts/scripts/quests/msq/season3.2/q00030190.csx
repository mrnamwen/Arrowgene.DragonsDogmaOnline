/**
 * @brief The Plight of Lookout Castle
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.ThePlightOfLookoutCastle;
    public override ushort RecommendedLevel => 92;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.TheFinalBattleOfTheRoyalCapital;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint LookoutCastleGuardTroop = 1775; // Lookout Castle Guard Troop
        public const uint WarMaster = 1776; // War Master (Boss)
        public const uint WarMasterFollower = 1777; // War Master Follower
    }

    private class QstLayoutFlag
    {
        // Fortress City Megado (st0486)
        public const uint MegadoNpcs0 = 6700; // NPCs for Black Knight search

        // Lookout Castle (st0450)
        public const uint LookoutCastleNpcs0 = 6701; // Nedo and crew
        public const uint LookoutCastleResidents = 6702; // Trapped residents
        public const uint LookoutCastleOms = 6703; // Blockades

        // Lookout Castle (st0451)
        public const uint LookoutCastleNpcs1 = 6704; // Post-battle NPCs with Meirova
    }

    private class MyQstFlag
    {
        public const uint StartWarMasterAdds = 1;
        public const uint EndWarMasterAdds = 2;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(92));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.ABriefDragonForce));
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
        // Group 0: First encounter - rescue the residents
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.LookoutCastle0, 3, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SquadLeaderDwarfOrc, 92, 0)
                .SetNamedEnemyParams(NamedParamId.LookoutCastleGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 92, 1)
                .SetNamedEnemyParams(NamedParamId.LookoutCastleGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 92, 2)
                .SetNamedEnemyParams(NamedParamId.LookoutCastleGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.BluntSoldierDwarfOrc, 92, 3)
                .SetNamedEnemyParams(NamedParamId.LookoutCastleGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadyGrimwargLightArmor, 92, 4)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.LookoutCastleGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadyGrimwargLightArmor, 92, 5)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.LookoutCastleGuardTroop),
        });

        // Group 1: Middle level combat
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.LookoutCastle0, 4, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadyGorecyclopsLightArmor0, 92, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.LookoutCastleGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.BluntSoldierDwarfOrc, 92, 1)
                .SetNamedEnemyParams(NamedParamId.LookoutCastleGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.BluntSoldierDwarfOrc, 92, 2)
                .SetNamedEnemyParams(NamedParamId.LookoutCastleGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 92, 3)
                .SetNamedEnemyParams(NamedParamId.LookoutCastleGuardTroop),
        });

        // Group 2: War Master boss fight
        AddEnemies(EnemyGroupId.Encounter + 2, Stage.LookoutCastle0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.WarMaster0, 92, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.WarMaster),
        });

        // Group 3: War Master adds (spawned during the fight)
        AddEnemies(EnemyGroupId.Encounter + 3, Stage.LookoutCastle0, 6, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 88, 1)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(50, 60)
                .SetNamedEnemyParams(NamedParamId.WarMasterFollower),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 88, 2)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(50, 60)
                .SetNamedEnemyParams(NamedParamId.WarMasterFollower),
            LibDdon.Enemy.CreateAuto(EnemyId.BluntSoldierDwarfOrc, 88, 3)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(50, 60)
                .SetNamedEnemyParams(NamedParamId.WarMasterFollower),
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadyGrimwargLightArmor, 88, 4)
                .SetInfectionType(1)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(50, 60)
                .SetNamedEnemyParams(NamedParamId.WarMasterFollower),
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadyGrimwargLightArmor, 88, 5)
                .SetInfectionType(1)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(50, 60)
                .SetNamedEnemyParams(NamedParamId.WarMasterFollower),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Head to Megado and search for the Black Knight
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 22000);
        process0.AddIsStageNoBlock(QuestAnnounceType.Accept, Stage.FortressCityMegadoResidentialLevel0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadoNpcs0);

        // Step 2: Head to the Lookout Castle quickly
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleResidents);

        // Step 3: Speak with Nedo
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle0, 0, 0, NpcId.Nedo0, 22010);

        // Step 4: Rescue the residents who failed to escape
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 0);

        // Step 5: Eliminate the enemy
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false);

        // Step 6: Confirm the safety of the residents you helped
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle0, 0, 2, NpcId.Quintus, 22020);

        // Step 7: Head to the middle level of the castle
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 1);

        // Step 8: Eliminate the enemy
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false);

        // Step 9: Confirm the war situation from a Liberation Army soldier
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle0, 0, 3, NpcId.Gurdolin3, 22030);

        // Step 10: Head for the castle gates
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle0, 50, 18200, -11750);

        // Pre-boss cutscene
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.LookoutCastle0, 0, 0, QuestJumpType.None, Stage.Invalid);

        // Step 11: Defeat the War Master
        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.StartWarMasterAdds)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleOms)
            .AddCheckCmdEmHpLess(Stage.LookoutCastle0, 1, 0, 65);

        // Post-boss cutscene with transition to LookoutCastle1
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.LookoutCastle0, 5, 13, QuestJumpType.After, Stage.LookoutCastle1)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndWarMasterAdds);

        // Step 12: Speak with Meirova
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle1, 0, 3, NpcId.Meirova0, 22040)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs1)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.LookoutCastle.NedosFurniture);

        // Return to Joseph to complete
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 22050);
        process0.AddProcessEndBlock(true);

        // Process 1: Handle War Master adds spawning during the boss fight
        var process1 = AddNewProcess(1);
        process1.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.StartWarMasterAdds);
        process1.AddSpawnGroupBlock(QuestAnnounceType.None, EnemyGroupId.Encounter + 3)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.EndWarMasterAdds);
        process1.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 2,
            EnemyGroupId.Encounter + 3
        ]);
        process1.AddProcessEndBlock(false);
    }
}

return new ScriptedQuest();
