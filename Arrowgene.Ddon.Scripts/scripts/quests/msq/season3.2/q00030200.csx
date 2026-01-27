/**
 * @brief The Final Battle of the Royal Capital
 * @desc Final quest of Season 3.2. Invade the Megado with the Liberation Army
 *       and reclaim the stolen Dragon Force from the White Dragon Black Knight.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.TheFinalBattleOfTheRoyalCapital;
    public override ushort RecommendedLevel => 94;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.TheMissingPrince;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint MegadoGuardTroop = 1780;        // Megado Guard Troop
        public const uint BlackKnightGuard = 1781;        // Black Knight Guard
        public const uint SummonedDemon = 1782;           // Summoned Demon
        public const uint WhiteDragonBlackKnight = 1783;  // White Dragon Black Knight
    }

    private class QstLayoutFlag
    {
        // Lookout Castle (st0451)
        public const uint LookoutCastleNpcs0 = 6700;      // Gillian, Elliot, Meirova, Gurdolin, Lise

        // Fortress City Megado Residential Level (st0460)
        public const uint MegadoResidentialNpcs = 6701;   // Gillian, Meirova
        public const uint MegadoResidentialOMs = 6702;    // Barriers, blockades

        // Megado Corridor (st0462)
        public const uint MegadoCorridorNpcs0 = 6703;     // Meirova leading charge
        public const uint MegadoCorridorOMs = 6704;       // Barriers

        // Fortress City Megado Royal Palace Level (st0464)
        public const uint RoyalPalaceNpcs0 = 6705;        // Meirova at rendezvous point
        public const uint RoyalPalaceNpcs1 = 6706;        // NPCs during Black Knight encounter
        public const uint RoyalPalaceOMs = 6707;          // Boss arena barriers
    }

    private class MyQstFlag
    {
        public const uint SpawnDemonWave = 1;
        public const uint EndDemonWave = 2;
        public const uint SpawnBlackKnightAdds = 3;
        public const uint EndBlackKnightAdds = 4;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(94));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.ThePlightOfLookoutCastle));
    }

    protected override void InitializeRewards()
    {
        AddPointReward(PointType.ExperiencePoints, 800000);
        AddWalletReward(WalletType.Gold, 90000);
        AddWalletReward(WalletType.RiftPoints, 9000);

        AddFixedItemReward(ItemId.RoyalCrestMedalMegadosysDistrict, 5);
        AddFixedItemReward(ItemId.UnappraisedWaterTrinketGeneral, 2);
        AddFixedItemReward(ItemId.ApMegadosysPlateau, 500);
        // Unique quest completion reward: Bracelet of the First King
        AddFixedItemReward(ItemId.BraceletOfTheFirstKing, 1);
    }

    protected override void InitializeEnemyGroups()
    {
        // Megado Corridor - Guard troops blocking the path
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.MegadoCorridor0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardFighter, 94, 0)
                .SetNamedEnemyParams(NamedParamId.MegadoGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardHunter, 94, 1)
                .SetNamedEnemyParams(NamedParamId.MegadoGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardSeeker, 94, 2)
                .SetNamedEnemyParams(NamedParamId.MegadoGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardWarrior, 94, 3)
                .SetNamedEnemyParams(NamedParamId.MegadoGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardFighter, 94, 4)
                .SetNamedEnemyParams(NamedParamId.MegadoGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardHunter, 94, 5)
                .SetNamedEnemyParams(NamedParamId.MegadoGuardTroop),
        });

        // Megado Corridor - Second wave with heavy armor
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.MegadoCorridor0, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadyGorecyclopsLightArmor0, 94, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.BlackKnightGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardWarrior, 94, 1)
                .SetNamedEnemyParams(NamedParamId.MegadoGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardWarrior, 94, 2)
                .SetNamedEnemyParams(NamedParamId.MegadoGuardTroop),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardFighter, 94, 3)
                .SetNamedEnemyParams(NamedParamId.MegadoGuardTroop),
        });

        // Royal Palace Level - Summoned demons
        AddEnemies(EnemyGroupId.Encounter + 2, Stage.FortressCityMegadoRoyalPalaceLevel, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedDemon, 94, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.SummonedDemon),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 92, 1)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.SummonedDemon),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 92, 2)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.SummonedDemon),
        });

        // Royal Palace Level - Boss: White Dragon Black Knight (Holy attacks, absorbed dragon power)
        AddEnemies(EnemyGroupId.Encounter + 3, Stage.FortressCityMegadoRoyalPalaceLevel, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackKnightHolyIce0, 94, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.WhiteDragonBlackKnight)
                .SetHpRate(300)      // Very tanky - absorbed dragon power
                .SetAttackRate(150)
                .SetDefenceRate(130)
                .SetMagicAttackRate(180)  // Holy attacks are enhanced
                .SetMagicDefenceRate(130),
        });

        // Royal Palace Level - Add spawns during Black Knight fight
        AddEnemies(EnemyGroupId.Encounter + 4, Stage.FortressCityMegadoRoyalPalaceLevel, 3, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardFighter, 90, 0)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(60, 90)
                .SetNamedEnemyParams(NamedParamId.BlackKnightGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardHunter, 90, 1)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(60, 90)
                .SetNamedEnemyParams(NamedParamId.BlackKnightGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardSeeker, 90, 2)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(60, 90)
                .SetNamedEnemyParams(NamedParamId.BlackKnightGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.MegadoGuardWarrior, 90, 3)
                .SetEnemyTargetTypesId(1)
                .SetStartThinkTblNo(2)
                .SetIsManualSet(true)
                .SetRepopConditions(60, 90)
                .SetNamedEnemyParams(NamedParamId.BlackKnightGuard),
        });

        // Prevent enemies from spawning in certain areas during quest
        AddEnemies(EnemyGroupId.Encounter + 5, Stage.FortressCityMegadoResidentialLevel0, 0, QuestEnemyPlacementType.Manual, new()
        {
            /* prevent enemies from spawning */
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Accept quest from Joseph
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 25000)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // Step 2: Head to the Lookout Castle
        process0.AddPartyGatherBlock(QuestAnnounceType.Accept, Stage.LookoutCastle1, 0, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs0);

        // Step 3: Head to the Megado Residential Level
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoResidentialLevel0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadoResidentialNpcs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadoResidentialOMs);

        // Step 4: Speak with Gillian
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoResidentialLevel0, 0, 0, NpcId.Gillian0, 25010);

        // Step 5: Storm into the Megado Corridor with General Meirova
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadoCorridor0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadoCorridorNpcs0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadoCorridorOMs);

        // Fight through the corridor
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false);

        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false);

        // Step 6: Leave the Megado Corridor and head for the Royal Palace Level
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoRoyalPalaceLevel)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.RoyalPalaceNpcs0);

        // Step 7: Rendezvous with the leading Meirova and check the situation
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoRoyalPalaceLevel, 0, 0, NpcId.Meirova0, 25020);

        // Step 8: Head towards the Royal Palace and search for the Black Knight
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoRoyalPalaceLevel, 2, 0, 0);

        // Cutscene: Black Knight summons demons
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.FortressCityMegadoRoyalPalaceLevel, 0, 0, QuestJumpType.None, Stage.Invalid);

        // Step 9: Defeat the summoned demons
        process0.AddSpawnGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 2)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnDemonWave);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndDemonWave);

        // Cutscene: Black Knight reveals absorbed dragon power
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.FortressCityMegadoRoyalPalaceLevel, 1, 0, QuestJumpType.None, Stage.Invalid)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.RoyalPalaceNpcs1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.RoyalPalaceOMs);

        // Step 10: Defeat the Black Knight - Final Boss
        process0.AddSpawnGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 3)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnBlackKnightAdds)
            .AddCheckCmdEmHpLess(Stage.FortressCityMegadoRoyalPalaceLevel, 2, 0, 50);  // At 50% HP, spawn adds

        // Black Knight defeated event
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.FortressCityMegadoRoyalPalaceLevel, 2, 0, QuestJumpType.After, Stage.FortressCityMegadoRoyalPalaceLevel)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndBlackKnightAdds);

        // Step 11: Return to Lestania
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 25030)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.AudienceChamber.TheCrewEndSeason32);

        process0.AddProcessEndBlock(true);

        // Process 1: Handle add spawns during Black Knight fight
        var process1 = AddNewProcess(1);
        process1.AddMyQstFlagsBlock(QuestAnnounceType.None)
           .AddMyQstCheckFlag(MyQstFlag.SpawnBlackKnightAdds);
        process1.AddSpawnGroupBlock(QuestAnnounceType.None, EnemyGroupId.Encounter + 4)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.EndBlackKnightAdds);
        process1.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 3,
            EnemyGroupId.Encounter + 4
        ]);
        process1.AddProcessEndBlock(false);
    }
}

return new ScriptedQuest();
