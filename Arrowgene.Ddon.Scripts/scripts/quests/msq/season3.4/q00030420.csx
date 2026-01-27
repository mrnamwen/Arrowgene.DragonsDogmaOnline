/**
 * @brief Spun Together Hope
 * @desc Season 3.4 MSQ: Save the Fire Dragon continent (Acre Selund) from the
 *       Vortex of Stagnation. Battle through Royal Capital Megado, destroy
 *       multiple Black Swords, and defeat demons in the Vortex of Stagnation.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.SpunTogetherHope;
    public override ushort RecommendedLevel => 100;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.TheWhiteDragonsArisen;

    private class EnemyGroupId
    {
        public const uint BlackSwordFeryana = 10;
        public const uint BlackSwordDacreim = 20;
        public const uint BlackSwordEvilDragonRoost = 30;
        public const uint MegadoDemonsEncounter = 40;
        public const uint VortexDemonsEncounter = 50;
    }

    private class NamedParamId
    {
        public const uint BlackSwordGuardian = 1960;      // Guardian of the Black Sword
        public const uint DemonOfTheVortex = 1961;        // Demon of the Vortex
        public const uint VortexAbomination = 1962;       // Vortex Abomination
    }

    private class QstLayoutFlag
    {
        // Lookout Castle (st0478)
        public const uint LookoutCastleNpcs0 = 8300;      // Quintus at Lookout Castle port
        public const uint LookoutCastleNpcs1 = 8301;      // NPCs during quest

        // Feryana Wilderness
        public const uint FeryanaWildernessNpcs = 8310;   // Mephite and party
        public const uint FeryanaBlackSword = 8311;       // Black Sword location

        // Dacreim Fortress
        public const uint DacreimFortressNpcs = 8320;     // NPCs at fortress
        public const uint DacreimBlackSword = 8321;       // Black Sword location

        // Evil Dragon's Roost
        public const uint EvilDragonRoostNpcs = 8330;     // Gillian and party
        public const uint EvilDragonRoostBlackSword = 8331; // Black Sword location

        // Royal Capital Megado
        public const uint MegadoCorridorNpcs = 8340;      // Former Arisen Corps
        public const uint MegadoRoyalPalaceNpcs = 8341;   // NPCs at palace gate

        // Vortex of Stagnation
        public const uint VortexNpcs0 = 8350;             // Initial setup
        public const uint VortexNpcs1 = 8351;             // After battle
    }

    private class MyQstFlag
    {
        public const uint SpawnBlackSwordFeryana = 1;
        public const uint DestroyedBlackSwordFeryana = 2;
        public const uint SpawnBlackSwordDacreim = 3;
        public const uint DestroyedBlackSwordDacreim = 4;
        public const uint SpawnBlackSwordEvilDragonRoost = 5;
        public const uint DestroyedBlackSwordEvilDragonRoost = 6;
        public const uint SpawnMegadoDemons = 7;
        public const uint DefeatedMegadoDemons = 8;
        public const uint SpawnVortexDemons = 9;
        public const uint DefeatedVortexDemons = 10;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(100));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.BreakdownOfReason));
    }

    protected override void InitializeRewards()
    {
        AddPointReward(PointType.ExperiencePoints, 1000000);
        AddWalletReward(WalletType.Gold, 100000);
        AddWalletReward(WalletType.RiftPoints, 10000);

        // Keystone to Ruin x10
        AddFixedItemReward(ItemId.KeystoneToRuin, 10);
    }

    protected override void InitializeEnemyGroups()
    {
        // ===== BLACK SWORD 1: Feryana Wilderness =====
        // Black Sword with guardian demons
        AddEnemies(EnemyGroupId.BlackSwordFeryana, Stage.FeryanaWilderness, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword1, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowGoblin, 100, 1)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowGoblinFighter, 100, 2)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowGoblinFighter, 100, 3)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowWolf, 100, 4)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
        });

        // ===== BLACK SWORD 2: Dacreim Fortress =====
        // Black Sword with Ancestor Orc guards
        AddEnemies(EnemyGroupId.BlackSwordDacreim, Stage.DacreimFortress0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword1, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrc, 100, 1)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.AncestorOrcFighter, 100, 2)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.LegionFighter, 100, 3)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.Legion, 100, 4)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
        });

        // ===== BLACK SWORD 3: Evil Dragon's Roost =====
        // Black Sword with powerful fire demons
        AddEnemies(EnemyGroupId.BlackSwordEvilDragonRoost, Stage.EvilDragonsRoost0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword1, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGrigori, 100, 1)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGoblinLeader, 100, 2)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGoblinFighter, 100, 3)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeWolf, 100, 4)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
        });

        // ===== MEGADO DEMONS ENCOUNTER =====
        // Demons encountered in Royal Capital Megado
        AddEnemies(EnemyGroupId.MegadoDemonsEncounter, Stage.MegadoCorridor0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.Eliminator, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.DemonOfTheVortex)
                .SetHpRate(200),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkCorpsePunisher, 100, 1)
                .SetNamedEnemyParams(NamedParamId.DemonOfTheVortex),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkCorpseTorturer, 100, 2)
                .SetNamedEnemyParams(NamedParamId.DemonOfTheVortex),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkSkeleton, 100, 3)
                .SetNamedEnemyParams(NamedParamId.DemonOfTheVortex),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkSkeletonBrute, 100, 4)
                .SetNamedEnemyParams(NamedParamId.DemonOfTheVortex),
        });

        // ===== VORTEX OF STAGNATION DEMONS =====
        // Final battle inside the Vortex of Stagnation
        AddEnemies(EnemyGroupId.VortexDemonsEncounter, Stage.VortexofStagnation0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.Abaddon0, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.VortexAbomination)
                .SetHpRate(300)
                .SetAttackRate(150)
                .SetDefenceRate(130),
            LibDdon.Enemy.CreateAuto(EnemyId.Ghost, 100, 1)
                .SetNamedEnemyParams(NamedParamId.VortexAbomination),
            LibDdon.Enemy.CreateAuto(EnemyId.MiseryGhost, 100, 2)
                .SetNamedEnemyParams(NamedParamId.VortexAbomination),
            LibDdon.Enemy.CreateAuto(EnemyId.GrudgeGhost, 100, 3)
                .SetNamedEnemyParams(NamedParamId.VortexAbomination),
            LibDdon.Enemy.CreateAuto(EnemyId.RageGhost, 100, 4)
                .SetNamedEnemyParams(NamedParamId.VortexAbomination),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // ========================================
        // STEP 1: Head to Acre Selund
        // Accept quest from White Dragon, travel to Acre Selund
        // ========================================
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.TheWhiteDragon, 30420)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // ========================================
        // STEP 2: Head to the Royal Palace layer where Prince Nedo awaits
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.Accept, Stage.FortressCityMegadoRoyalPalaceLevel);

        // ========================================
        // STEP 3: Speak with Nedo
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoRoyalPalaceLevel, NpcId.Nedo0, 30421);

        // ========================================
        // STEP 4: Head towards Quintus at Lookout Castle port
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle1, 0, 0, NpcId.Quintus, 30422)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs0);

        // ========================================
        // STEP 5: Head towards Mephite in Feryana Wilderness - Search for Black Sword
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FeryanaWilderness)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FeryanaWildernessNpcs);

        // ========================================
        // STEP 6: Proceed ahead and search for the Black Sword
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.Update, Stage.FeryanaWilderness, 0, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FeryanaBlackSword);

        // ========================================
        // STEP 7: Defeat the enemy and destroy the Black Sword (Feryana)
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.BlackSwordFeryana)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnBlackSwordFeryana);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.BlackSwordFeryana, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.DestroyedBlackSwordFeryana);

        // ========================================
        // STEP 8: Speak with Bertha
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FeryanaWilderness, NpcId.Bertha, 30423);

        // ========================================
        // STEP 9: Head towards Dacreim Fortress - Search for Black Sword
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.DacreimFortress0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DacreimFortressNpcs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DacreimBlackSword);

        // ========================================
        // STEP 10: Defeat the enemy and destroy the Black Sword (Dacreim)
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.BlackSwordDacreim)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnBlackSwordDacreim);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.BlackSwordDacreim, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.DestroyedBlackSwordDacreim);

        // ========================================
        // STEP 11: Speak with Meirova
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.DacreimFortress0, NpcId.Meirova0, 30424);

        // ========================================
        // STEP 12: Head towards Evil Dragon's Roost - Search for Black Sword
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EvilDragonsRoost0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EvilDragonRoostNpcs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EvilDragonRoostBlackSword);

        // ========================================
        // STEP 13: Defeat the enemy and destroy the Black Sword (Evil Dragon's Roost)
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.BlackSwordEvilDragonRoost)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnBlackSwordEvilDragonRoost);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.BlackSwordEvilDragonRoost, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.DestroyedBlackSwordEvilDragonRoost);

        // ========================================
        // STEP 14: Speak with Gillian
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EvilDragonsRoost0, NpcId.Gillian0, 30425);

        // ========================================
        // STEP 15: Head to the Royal Capital Megado being engulfed by vortex
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoResidentialLevel0);

        // ========================================
        // STEP 16: Return to Lestania - Head to Travers at White Dragon Temple
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, NpcId.Travers0, 30426);

        // ========================================
        // STEP 17: Clear "The Great Dragon Crystal War: Captured Palace"
        // (Accept challenge from Travers - this would typically trigger a separate instance)
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, NpcId.Travers0, 30427);

        // ========================================
        // STEP 18: Report to Travers
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, NpcId.Travers0, 30428);

        // ========================================
        // STEP 19: Head towards location of former Arisen Corps at Megado Corridor
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadoCorridor0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadoCorridorNpcs);

        // ========================================
        // STEP 20: Head towards the gate in Megado Royal Palace layer
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadoCorridor0, 0, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MegadoRoyalPalaceNpcs);

        // ========================================
        // STEP 21: Defeat the encountered demons
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.MegadoDemonsEncounter)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnMegadoDemons);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.MegadoDemonsEncounter, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.DefeatedMegadoDemons);

        // ========================================
        // STEP 22: Speak with Gurdolin
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadoCorridor0, NpcId.Gurdolin3, 30429);

        // ========================================
        // STEP 23: Plunge into the Vortex of Stagnation
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexNpcs0);

        // ========================================
        // STEP 24: Defeat the demons of the Vortex of Stagnation
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.VortexDemonsEncounter)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnVortexDemons);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.VortexDemonsEncounter, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.DefeatedVortexDemons)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexNpcs1);

        // ========================================
        // STEP 25: Speak with Nedo
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation0, 0, 0, NpcId.Nedo0, 30430);

        // ========================================
        // STEP 26: Return to Lestania and report to Joseph
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 30431)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // Unlock Dragon Abilities: Fire Dragon tutorial
        process0.AddIsStageNoBlock(QuestAnnounceType.None, Stage.AudienceChamber)
            .AddResultCmdTutorialDialog(TutorialId.DragonAbilitiesFireDragon);

        process0.AddProcessEndBlock(true);

        // ========================================
        // Process 1: Black Sword Feryana cleanup
        // ========================================
        var process1 = AddNewProcess(1);
        process1.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.DestroyedBlackSwordFeryana);
        process1.AddRawBlock(QuestAnnounceType.None)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.SpawnBlackSwordDacreim);
        process1.AddProcessEndBlock(false);

        // ========================================
        // Process 2: Black Sword Dacreim cleanup
        // ========================================
        var process2 = AddNewProcess(2);
        process2.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.DestroyedBlackSwordDacreim);
        process2.AddRawBlock(QuestAnnounceType.None)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.SpawnBlackSwordEvilDragonRoost);
        process2.AddProcessEndBlock(false);

        // ========================================
        // Process 3: Black Sword Evil Dragon's Roost cleanup
        // ========================================
        var process3 = AddNewProcess(3);
        process3.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.DestroyedBlackSwordEvilDragonRoost);
        process3.AddRawBlock(QuestAnnounceType.None)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.SpawnMegadoDemons);
        process3.AddProcessEndBlock(false);

        // ========================================
        // Process 4: Vortex battle management
        // ========================================
        var process4 = AddNewProcess(4);
        process4.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.SpawnVortexDemons);
        process4.AddRawBlock(QuestAnnounceType.None)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.DefeatedVortexDemons);
        process4.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.VortexDemonsEncounter
        ]);
        process4.AddProcessEndBlock(false);
    }
}

return new ScriptedQuest();
