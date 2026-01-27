/**
 * @brief Attack on the Royal Capital
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.AttackOnTheRoyalCapital;
    public override ushort RecommendedLevel => 90;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.AnOmenOfDestruction;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint MegadoRaidForce0 = 1776; // Megado Raid Force
        public const uint MegadoRaidForce1 = 1777; // Megado Raid Force
        public const uint MegadoRaidCommander = 1778; // Megado Raid Commander
    }

    private class QstLayoutFlag
    {
        // Eli Guard Tower (st0584)
        public const uint EliGuardTowerNpcs0 = 6700; // Nedo, Fabio, etc.

        // Fortress City Megado Residential Level (st0486/487)
        public const uint ResidentialNpcs0 = 6701; // Bertha's group
        public const uint ResidentialBlockade = 6702;
    }

    private class MyQstFlag
    {
        public const uint StartAdds = 1;
        public const uint EndAdds = 2;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(90));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.RallyTheTroops));
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
        // Initial enemies in the residential level
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.FortressCityMegadoResidentialLevel0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SquadLeaderDwarfOrc, 90, 0)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidForce0),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 90, 1)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidForce0),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 90, 2)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidForce0),
            LibDdon.Enemy.CreateAuto(EnemyId.BluntSoldierDwarfOrc, 90, 3)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidForce0),
            LibDdon.Enemy.CreateAuto(EnemyId.BluntSoldierDwarfOrc, 90, 4)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidForce0),
            LibDdon.Enemy.CreateAuto(EnemyId.HeavySoldierDwarfOrc, 90, 5)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidForce0),
        });

        // Boss encounter: War-Armored Gore Manticore (Light type)
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.FortressCityMegadoResidentialLevel0, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadyGoremanticoreLightArmor, 90, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidCommander),
        });

        // Infinite Dwarf Orc spawns (ignored for quest completion)
        AddEnemies(EnemyGroupId.Encounter + 2, Stage.FortressCityMegadoResidentialLevel0, 3, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 90, 0)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(50, 60)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidForce1),
            LibDdon.Enemy.CreateAuto(EnemyId.SwordSoldierDwarfOrc, 90, 1)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(50, 60)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidForce1),
            LibDdon.Enemy.CreateAuto(EnemyId.BluntSoldierDwarfOrc, 90, 2)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(50, 60)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidForce1),
            LibDdon.Enemy.CreateAuto(EnemyId.BluntSoldierDwarfOrc, 90, 3)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(50, 60)
                .SetNamedEnemyParams(NamedParamId.MegadoRaidForce1),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);
        // Step 1: Head to Eli Guard Tower in the Megado territory
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 21900)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.MephiteTravelersInn.Nayajiku)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);
        process0.AddIsStageNoBlock(QuestAnnounceType.Accept, Stage.EliGuardTower)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EliGuardTowerNpcs0);
        // Step 2: Speak with Nedo
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 0, 0, NpcId.Nedo0, 21901);
        // Step 3: Head to the approach to the Megado residential level
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoResidentialLevel0);
        // Step 4: Meet with Bertha waiting at the raid entrance
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoResidentialLevel0, 0, 0, NpcId.Bertha, 21902)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.ResidentialNpcs0);
        // Step 5: Raid the Megado residential level
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.FortressCityMegadoResidentialLevel0, 0, 0, QuestJumpType.None, Stage.Invalid);
        // Step 6: Sweep away the enemy to gain control of the residential level
        process0.AddDestroyGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 0);
        // Step 7: Rendezvous with the comrades and check situation
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoResidentialLevel0, 0, 0, 0);
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.FortressCityMegadoResidentialLevel0, 5, 0, QuestJumpType.None, Stage.Invalid);
        // Step 8: Defeat the enemy to allow comrades to retreat
        process0.AddSpawnGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 1)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.StartAdds)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.ResidentialBlockade);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndAdds);
        // Step 9: Retreat to Eli Guard Tower through the well
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.FortressCityMegadoResidentialLevel0, 10, 0, QuestJumpType.After, Stage.EliGuardTower);
        // Step 10: Speak with Fabio
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EliGuardTower, 0, 0, NpcId.Fabio0, 21903)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.ResidentialBlockade);
        // Step 11: Return quickly to the White Dragon Temple
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber);
        // Step 12: Speak with the White Dragon
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.TheWhiteDragon, 21904)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.MephiteTravelersInn.Nayajiku);
        process0.AddProcessEndBlock(true);

        // Process 1: Handle infinite Dwarf Orc spawns during boss fight
        var process1 = AddNewProcess(1);
        process1.AddMyQstFlagsBlock(QuestAnnounceType.None)
           .AddMyQstCheckFlag(MyQstFlag.StartAdds);
        process1.AddSpawnGroupBlock(QuestAnnounceType.None, EnemyGroupId.Encounter + 2)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.EndAdds);
        process1.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 1,
            EnemyGroupId.Encounter + 2
        ]);
        process1.AddProcessEndBlock(false);
    }
}

return new ScriptedQuest();
