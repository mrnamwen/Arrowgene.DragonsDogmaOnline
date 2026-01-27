/**
 * @brief Hope's Bitter End
 * @desc CLIMACTIC BOSS QUEST of Season 3.3. Head to Firefall Mountain to help
 *       Prince Nedo confront the Evil Dragon. Fight through fire elementals and
 *       war-ready monsters on the Sacred Flame Path before the epic two-phase
 *       battle against the Evil Dragon at its roost.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.HopesBitterEnd;
    public override ushort RecommendedLevel => 100;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.ThoseWhoFollowTheDragon;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint SacredFlameGuardian = 1950;     // Sacred Flame Guardian <name>
        public const uint BlockingThePath = 1951;         // <name> Blocking the Path
        public const uint TheEvilDragon = 1952;           // The Evil Dragon
    }

    private class QstLayoutFlag
    {
        // Lookout Castle (st0478)
        public const uint LookoutCastleNpcs0 = 7861;      // Meirova, Gillian, Bertha at Lookout Castle
        public const uint LookoutCastleNpcs1 = 7862;      // NPCs during event

        // Firefall Mountain Campsite (st0595)
        public const uint CampsiteNpcs = 7901;            // Cyril at campsite entrance

        // Sacred Flame Path (st0490)
        public const uint SacredFlamePathNpcs = 8059;     // NPCs along the path
        public const uint SacredFlamePathOMs0 = 8060;     // First barrier/blockade
        public const uint SacredFlamePathOMs1 = 8061;     // Second barrier
        public const uint SacredFlamePathOMs2 = 8062;     // Third barrier

        // Sacred Flame Path Upper Level (st0491)
        public const uint UpperLevelNpcs = 8065;          // NPCs at upper level

        // Evil Dragon's Roost (st0588)
        public const uint EvilDragonsRoostNpcs0 = 7867;   // Battle setup
        public const uint EvilDragonsRoostNpcs1 = 7868;   // Phase 1 setup
        public const uint EvilDragonsRoostNpcs2 = 7872;   // Post-battle NPCs (Nedo, party)
        public const uint EvilDragonsRoostOMs = 7903;     // Boss arena barriers
    }

    private class MyQstFlag
    {
        public const uint SpawnFirstEncounter = 1;
        public const uint EndFirstEncounter = 2;
        public const uint SpawnSecondEncounter = 3;
        public const uint EndSecondEncounter = 4;
        public const uint SpawnThirdEncounter = 5;
        public const uint EndThirdEncounter = 6;
        public const uint SpawnEvilDragonPhase1 = 7;
        public const uint EvilDragonPhase1Complete = 8;
        public const uint SpawnEvilDragonPhase2 = 9;
        public const uint EvilDragonPhase2Complete = 10;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(100));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.TheRelicsOfTheFirstKing));
    }

    protected override void InitializeRewards()
    {
        AddPointReward(PointType.ExperiencePoints, 900000);
        AddWalletReward(WalletType.Gold, 100000);
        AddWalletReward(WalletType.RiftPoints, 10000);

        // Royal Crest Medal (Urteca District) x5
        AddFixedItemReward(ItemId.RoyalCrestMedalUrtecaDistrict, 5);
        // Unappraised Cloud Trinket (General) x2
        AddFixedItemReward(ItemId.UnappraisedCloudTrinketGeneral, 2);
        // AP (Urteca Mountains) x500
        AddFixedItemReward(ItemId.ApUrtecaMountains, 500);
    }

    protected override void InitializeEnemyGroups()
    {
        // ===== ENCOUNTER 1: Sacred Flame Path Entrance =====
        // War-Ready Grimwargs (Senko Grimwag), Blaze Harpy, Blaze Grigori
        // Location: Near entrance at X:53, Y:90
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.SacredFlamePath0, 17, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadyGrimwargLightArmor, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.SacredFlameGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadyGrimwargLightArmor, 100, 1)
                .SetNamedEnemyParams(NamedParamId.SacredFlameGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeHarpy, 100, 2)
                .SetNamedEnemyParams(NamedParamId.SacredFlameGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGrigori, 100, 3)
                .SetNamedEnemyParams(NamedParamId.SacredFlameGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGrigori, 100, 4)
                .SetNamedEnemyParams(NamedParamId.SacredFlameGuardian),
        });

        // ===== ENCOUNTER 2: War-Ready Goremanticore =====
        // Armored Gormanticore blocking the way at X:37, Y:75
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.SacredFlamePath0, 18, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadyGoremanticoreLightArmor, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlockingThePath)
                .SetHpRate(200)
                .SetAttackRate(130)
                .SetDefenceRate(120)
                .SetMagicAttackRate(130)
                .SetMagicDefenceRate(120),
        });

        // ===== ENCOUNTER 3: Final Path Enemies =====
        // War-Ready Saurian (Armored Lizardman), Dwarf Orcs blocking the way
        // Location: Square before the door at X:51, Y:42
        AddEnemies(EnemyGroupId.Encounter + 2, Stage.SacredFlamePath0, 3, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.WarReadySaurianLightArmor, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlockingThePath),
            LibDdon.Enemy.CreateAuto(EnemyId.RangedSoldierDwarfOrc, 100, 1)
                .SetNamedEnemyParams(NamedParamId.BlockingThePath),
            LibDdon.Enemy.CreateAuto(EnemyId.RangedSoldierDwarfOrc, 100, 2)
                .SetNamedEnemyParams(NamedParamId.BlockingThePath),
            LibDdon.Enemy.CreateAuto(EnemyId.SquadLeaderDwarfOrc, 100, 3)
                .SetNamedEnemyParams(NamedParamId.BlockingThePath),
            LibDdon.Enemy.CreateAuto(EnemyId.HeavySoldierDwarfOrc, 100, 4)
                .SetNamedEnemyParams(NamedParamId.BlockingThePath),
        });

        // ===== BOSS ENCOUNTER: Evil Dragon Phase 1 =====
        // First form - similar to Phoenix, fire-based attacks
        AddEnemies(EnemyGroupId.Encounter + 3, Stage.EvilDragonsRoost1, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.TheEvilDragon0, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.TheEvilDragon)
                .SetHpRate(400)       // Phase 1: Very tanky
                .SetAttackRate(150)
                .SetDefenceRate(130)
                .SetMagicAttackRate(180)  // Heavy fire magic
                .SetMagicDefenceRate(130),
        });

        // ===== BOSS ENCOUNTER: Evil Dragon Phase 2 =====
        // Revived form - even more powerful after regaining power
        AddEnemies(EnemyGroupId.Encounter + 4, Stage.EvilDragonsRoost1, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.TheEvilDragon1, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.TheEvilDragon)
                .SetHpRate(500)       // Phase 2: Even tankier
                .SetAttackRate(180)
                .SetDefenceRate(150)
                .SetMagicAttackRate(200)  // Devastating fire attacks
                .SetMagicDefenceRate(150),
        });

        // Prevent normal spawns during quest
        AddEnemies(EnemyGroupId.Encounter + 5, Stage.SacredFlamePath0, 0, QuestEnemyPlacementType.Manual, new()
        {
            /* prevent enemies from spawning */
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // ========================================
        // STEP 1: Accept quest from The White Dragon
        // ========================================
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.TheWhiteDragon, 22449)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // ========================================
        // STEP 2: Head to the Lookout Castle
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.Accept, Stage.LookoutCastle1, 15, 18280, -14593)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs0);

        // Cutscene at Lookout Castle
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.LookoutCastle1, 20, 0, QuestJumpType.After, Stage.LookoutCastle1);

        // ========================================
        // STEP 3: Speak with Meirova
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle1, 0, 1, NpcId.Meirova0, 22490);

        // ========================================
        // STEP 4: Rendezvous with Cyril at Firefall Mountain Camp
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FirefallMountainCampsite, 0, 0, NpcId.Cyril, 26027)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.CampsiteNpcs);

        // ========================================
        // STEP 5: Enter the Sacred Flame Path
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.SacredFlamePath0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.SacredFlamePathNpcs);

        // ========================================
        // STEP 6: ENCOUNTER 1 - Defeat Grimwargs, Blaze Harpy, Blaze Grigori
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnFirstEncounter);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndFirstEncounter);

        // ========================================
        // STEP 7: Head for the Evil Dragon's Roost
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.SacredFlamePath0, 1, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.SacredFlamePathOMs0);

        // ========================================
        // STEP 8: ENCOUNTER 2 - Defeat War-Ready Goremanticore
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnSecondEncounter);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndSecondEncounter);

        // ========================================
        // STEP 9: Continue to the Evil Dragon's Roost
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.SacredFlamePath0, 2, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.SacredFlamePathOMs1);

        // ========================================
        // STEP 10: ENCOUNTER 3 - Defeat Lizardmen and Dwarf Orcs
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnThirdEncounter);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndThirdEncounter);

        // ========================================
        // STEP 11: Enter the Sacred Flame Path Upper Level
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.SacredFlamePathUpperLevel)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.UpperLevelNpcs);

        // Proceed to the door leading to Evil Dragon's Roost
        process0.AddPartyGatherBlock(QuestAnnounceType.Update, Stage.SacredFlamePathUpperLevel, 21030, 2934, -24797);

        // ========================================
        // STEP 12: Enter Evil Dragon's Roost - Cutscene and BOSS PHASE 1
        // ========================================
        // Cutscene: Arrival at Evil Dragon's Roost, confrontation begins
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.EvilDragonsRoost1, 0, 0, QuestJumpType.After, Stage.EvilDragonsRoost1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EvilDragonsRoostNpcs0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EvilDragonsRoostOMs);

        // BOSS PHASE 1: Defeat the Evil Dragon (first form)
        process0.AddSpawnGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 3)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnEvilDragonPhase1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EvilDragonsRoostNpcs1);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 3, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EvilDragonPhase1Complete);

        // ========================================
        // STEP 13: Cutscene - Evil Dragon revives with renewed power
        // ========================================
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.EvilDragonsRoost1, 5, 0, QuestJumpType.None, Stage.Invalid)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.EvilDragonsRoostNpcs1);

        // BOSS PHASE 2: Defeat the Evil Dragon who has regained its power
        process0.AddSpawnGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 4)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnEvilDragonPhase2);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 4, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EvilDragonPhase2Complete);

        // Victory cutscene
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.EvilDragonsRoost1, 10, 0, QuestJumpType.After, Stage.EvilDragonsRoost1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.EvilDragonsRoostOMs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.EvilDragonsRoostNpcs2);

        // ========================================
        // STEP 14: Speak with Nedo
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.EvilDragonsRoost1, 1, 0, NpcId.Nedo0, 26035);

        // ========================================
        // STEP 15: Return to Lestania and report to the White Dragon
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.TheWhiteDragon, 22599)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        process0.AddProcessEndBlock(true);

        // ========================================
        // Process 1: Handle Evil Dragon Phase 1 mechanics
        // Triggers phase transition when HP reaches a threshold
        // ========================================
        var process1 = AddNewProcess(1);
        process1.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.SpawnEvilDragonPhase1);
        // Monitor HP for phase transition (at 1% HP, trigger transition instead of death)
        process1.AddNoOpBlock(QuestAnnounceType.None)
            .AddCheckCmdEmHpLess(Stage.EvilDragonsRoost1, 1, 0, 1);
        process1.AddProcessEndBlock(false);

        // ========================================
        // Process 2: Handle Evil Dragon Phase 2 mechanics
        // Buff management during the second phase
        // ========================================
        var process2 = AddNewProcess(2);
        process2.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.SpawnEvilDragonPhase2);
        process2.AddNoOpBlock(QuestAnnounceType.None)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.EvilDragonPhase2Complete);
        process2.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 3,
            EnemyGroupId.Encounter + 4
        ]);
        process2.AddProcessEndBlock(false);

        // ========================================
        // Process 3: NPC escort dialogue during Sacred Flame Path
        // ========================================
        var process3 = AddNewProcess(3);
        process3.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.EndFirstEncounter);
        process3.AddNoOpBlock(QuestAnnounceType.None)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.EndThirdEncounter);
        process3.AddProcessEndBlock(false);
    }
}

return new ScriptedQuest();
