/**
 * @brief Breakdown of Reason
 * @desc First quest of the FINAL season (Season 3.4). Glimpse with the White Dragon
 *       the unsettling events unfolding in the world of order. Travel to Phindym continent
 *       and destroy multiple Black Swords while confronting demons threatening the world.
 *       This is the longest quest with 30 steps.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.BreakdownOfReason;
    public override ushort RecommendedLevel => 100;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.SpunTogetherHope;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint BlackSwordGuardian = 2000;      // Black Sword Guardian <name>
        public const uint BlackSword = 2001;              // The Black Sword
        public const uint VortexDemon = 2002;             // Demon of the Vortex
        public const uint DarkCrystalGuard = 2003;        // Dark Crystal Guard
        public const uint StagnationDemon = 2004;         // Demon of Stagnation
    }

    private class QstLayoutFlag
    {
        // Audience Chamber (st0201) - Initial and final scenes
        public const uint AudienceChamberNpcs0 = 8700;    // Joseph, Klaus for revelation
        public const uint AudienceChamberNpcs1 = 8701;    // NPCs after dragon vision
        public const uint AudienceChamberNpcs2 = 8702;    // NPCs for final report

        // Elan Water Grove (st0121) - Rendezvous with Mordred
        public const uint ElanWaterGroveNpcs = 8710;      // Mordred at meeting point
        public const uint ElanWaterGroveOMs = 8711;       // First Black Sword location

        // Kingal Canyon (st0122) - Second Black Sword location
        public const uint KingalCanyonNpcs = 8720;        // NPCs searching for Black Sword
        public const uint KingalCanyonOMs = 8721;         // Second Black Sword markers

        // Hollow of Beginnings (st0880/881) - Information gathering
        public const uint HollowOfBeginningsNpcs = 8730;  // Spirit Dragon NPCs
        public const uint HollowOfBeginningsOMs = 8731;   // Investigation markers

        // Spirit Dragon's Roost (st0435/436) - Third Black Sword
        public const uint SpiritDragonsRoostNpcs = 8740;  // Adair Donnchadh
        public const uint SpiritDragonsRoostOMs = 8741;   // Third Black Sword location

        // Shadolean Great Temple (st0432/433/439) - Being engulfed by whirlpool
        public const uint ShadoleanGreatTempleNpcs0 = 8750; // Initial temple state
        public const uint ShadoleanGreatTempleNpcs1 = 8751; // Gearoid's envoy Musel
        public const uint ShadoleanGreatTempleOMs = 8752;   // Temple barriers

        // Darkness Shrouded Shadolean Great Temple (st3110/3111) - Dark energy
        public const uint DarkTempleNpcs = 8760;          // NPCs in darkness
        public const uint DarkTempleOMs = 8761;           // Dark energy markers

        // Vortex of Stagnation (st0430/431) - Final demon battle
        public const uint VortexNpcs = 8770;              // Battle setup NPCs
        public const uint VortexOMs = 8771;               // Battle arena
    }

    private class MyQstFlag
    {
        // Elan Water Grove - First Black Sword
        public const uint SpawnFirstBlackSwordGuards = 1;
        public const uint EndFirstBlackSwordGuards = 2;
        public const uint SpawnFirstBlackSword = 3;
        public const uint EndFirstBlackSword = 4;

        // Kingal Canyon - Second Black Sword
        public const uint SpawnSecondBlackSwordGuards = 5;
        public const uint EndSecondBlackSwordGuards = 6;
        public const uint SpawnSecondBlackSword = 7;
        public const uint EndSecondBlackSword = 8;

        // Spirit Dragon's Roost - Third Black Sword
        public const uint SpawnThirdBlackSwordGuards = 9;
        public const uint EndThirdBlackSwordGuards = 10;
        public const uint SpawnThirdBlackSword = 11;
        public const uint EndThirdBlackSword = 12;

        // Darkness Shrouded Temple - Demon encounters
        public const uint SpawnDarkTempleDemons = 13;
        public const uint EndDarkTempleDemons = 14;

        // Vortex of Stagnation - Final demons
        public const uint SpawnVortexDemons = 15;
        public const uint EndVortexDemons = 16;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(100));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.ThoseWhoFollowTheDragon));
    }

    protected override void InitializeRewards()
    {
        AddPointReward(PointType.ExperiencePoints, 1000000);
        AddWalletReward(WalletType.Gold, 100000);
        AddWalletReward(WalletType.RiftPoints, 10000);

        // Keystone to Ruin x1 - Key item for Season 3.4 content
        AddFixedItemReward(ItemId.KeystoneToRuin, 1);
    }

    protected override void InitializeEnemyGroups()
    {
        // ===== ELAN WATER GROVE - First Black Sword Encounter =====
        // Black Sword Guardians guarding the sword
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.ElanWaterGrove, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 100, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian)
                .SetHpRate(150)
                .SetAttackRate(120),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedWarg, 100, 1)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedWarg, 100, 2)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedPixie, 100, 3)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedPixie, 100, 4)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
        });

        // First Black Sword
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.ElanWaterGrove, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword0, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlackSword)
                .SetHpRate(200)
                .SetDefenceRate(150),
        });

        // ===== KINGAL CANYON - Second Black Sword Encounter =====
        // Dark Crystal Guards
        AddEnemies(EnemyGroupId.Encounter + 2, Stage.KingalCanyon, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGriffin, 100, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.DarkCrystalGuard)
                .SetHpRate(180)
                .SetAttackRate(130),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 100, 1)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.DarkCrystalGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 100, 2)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.DarkCrystalGuard),
        });

        // Second Black Sword
        AddEnemies(EnemyGroupId.Encounter + 3, Stage.KingalCanyon, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword0, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlackSword)
                .SetHpRate(220)
                .SetDefenceRate(160),
        });

        // ===== SPIRIT DRAGON'S ROOST - Third Black Sword Encounter =====
        // Powerful guardians near the Spirit Dragon's domain
        AddEnemies(EnemyGroupId.Encounter + 4, Stage.SpiritDragonsRoost0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedDemon, 100, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.DarkCrystalGuard)
                .SetHpRate(200)
                .SetAttackRate(140)
                .SetMagicAttackRate(150),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 100, 1)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.DarkCrystalGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 100, 2)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.DarkCrystalGuard),
        });

        // Third Black Sword
        AddEnemies(EnemyGroupId.Encounter + 5, Stage.SpiritDragonsRoost0, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword1, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlackSword)
                .SetHpRate(250)
                .SetDefenceRate(170)
                .SetMagicDefenceRate(150),
        });

        // ===== DARKNESS SHROUDED SHADOLEAN GREAT TEMPLE - Demon Encounter =====
        AddEnemies(EnemyGroupId.Encounter + 6, Stage.DarknessShroudedShadoleanGreatTemple1, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedDemon, 100, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.VortexDemon)
                .SetHpRate(250)
                .SetAttackRate(150)
                .SetMagicAttackRate(170),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGriffin, 100, 1)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.VortexDemon),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGriffin, 100, 2)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.VortexDemon),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 100, 3)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.VortexDemon),
        });

        // ===== VORTEX OF STAGNATION - Final Demon Battle =====
        AddEnemies(EnemyGroupId.Encounter + 7, Stage.VortexofStagnation1, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedDemon, 100, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.StagnationDemon)
                .SetHpRate(350)
                .SetAttackRate(160)
                .SetDefenceRate(140)
                .SetMagicAttackRate(200)
                .SetMagicDefenceRate(150),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedDemon, 100, 1)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.StagnationDemon)
                .SetHpRate(250),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 100, 2)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.StagnationDemon),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 100, 3)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.StagnationDemon),
        });

        // Prevent spawns in quest areas
        AddEnemies(EnemyGroupId.Encounter + 8, Stage.ElanWaterGrove, 0, QuestEnemyPlacementType.Manual, new()
        {
            /* prevent enemies from spawning */
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // ========================================
        // STEP 1: Seek out the White Dragon
        // ========================================
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.TheWhiteDragon, 26100)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // ========================================
        // STEP 2: With the White Dragon, glimpse through the dragon's eyes the destruction of order
        // ========================================
        process0.AddPlayEventBlock(QuestAnnounceType.Accept, Stage.AudienceChamber, 40, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.AudienceChamberNpcs0);

        // Unlock tutorials for Season 3.4 content
        process0.AddIsStageNoBlock(QuestAnnounceType.None, Stage.AudienceChamber)
            .AddResultCmdTutorialDialog(TutorialId.DarkDragonCrystalDestruction);
        process0.AddIsStageNoBlock(QuestAnnounceType.None, Stage.AudienceChamber)
            .AddResultCmdTutorialDialog(TutorialId.MissionsinDungeons);
        process0.AddIsStageNoBlock(QuestAnnounceType.None, Stage.AudienceChamber)
            .AddResultCmdTutorialDialog(TutorialId.ChainRewards);
        process0.AddIsStageNoBlock(QuestAnnounceType.None, Stage.AudienceChamber)
            .AddResultCmdTutorialDialog(TutorialId.DragonAbilities);

        // ========================================
        // STEP 3: Speak with Joseph
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 26101)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.AudienceChamberNpcs1);

        // ========================================
        // STEP 4: Head to Phindym with the Arisen Corps
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.ElanWaterGrove)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.ElanWaterGroveNpcs);

        // ========================================
        // STEP 5: Speak with Gearoid's envoy Musel
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.ShadoleanGreatTemple0, 0, 0, NpcId.Musel0, 26102)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.ShadoleanGreatTempleNpcs1);

        // ========================================
        // STEP 6: Gearoid awaits at Shadolean Great Temple
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.ShadoleanGreatTemple1, 0, 0, NpcId.Gearoid0, 26103)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.ShadoleanGreatTempleNpcs0);

        // Cutscene: Gearoid explains the Black Sword threat
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.ShadoleanGreatTemple1, 0, 0);

        // ========================================
        // STEP 7: Rendezvous with Mordred in the Elan Water Grove
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.ElanWaterGrove, 0, 0, NpcId.Mordred0, 26104)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.ElanWaterGroveOMs);

        // ========================================
        // STEP 8: Head to the location of the Black Sword
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.ElanWaterGrove, 1, 0, 0);

        // ========================================
        // STEP 9: Defeat the enemy and destroy the Black Sword (First)
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnFirstBlackSwordGuards);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndFirstBlackSwordGuards);

        // Destroy the First Black Sword
        process0.AddSpawnGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 1)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnFirstBlackSword);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndFirstBlackSword);

        // ========================================
        // STEP 10: Report to Mordred
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.ElanWaterGrove, 0, 1, NpcId.Mordred0, 26105);

        // ========================================
        // STEP 11: Head to Kingal Canyon and search for the Black Sword
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.KingalCanyon)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.KingalCanyonNpcs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.KingalCanyonOMs);

        // ========================================
        // STEP 12: Defeat the enemy and destroy the Black Sword (Second)
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnSecondBlackSwordGuards);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndSecondBlackSwordGuards);

        // Destroy the Second Black Sword
        process0.AddSpawnGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 3)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnSecondBlackSword);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 3, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndSecondBlackSword);

        // ========================================
        // STEP 13: Head towards the Hollow of Beginnings to obtain information about the new Black Sword
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.HollowofBeginnings0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.HollowOfBeginningsNpcs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.HollowOfBeginningsOMs);

        // Cutscene: Learning about the third Black Sword location
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.HollowofBeginnings0, 0, 0);

        // ========================================
        // STEP 14: Head towards the Spirit Dragon's Roost and search for the Black Sword
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.SpiritDragonsRoost0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.SpiritDragonsRoostNpcs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.SpiritDragonsRoostOMs);

        // ========================================
        // STEP 15: Defeat the enemy and destroy the Black Sword (Third)
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 4)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnThirdBlackSwordGuards);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 4, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndThirdBlackSwordGuards);

        // Destroy the Third Black Sword
        process0.AddSpawnGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 5)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnThirdBlackSword);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 5, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndThirdBlackSword);

        // ========================================
        // STEP 16: Speak with Adair Donnchadh
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.SpiritDragonsRoost0, 0, 0, NpcId.AdairDonnchadh0, 26106);

        // Cutscene: Adair reveals the temple is being engulfed
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.SpiritDragonsRoost0, 1, 0);

        // ========================================
        // STEP 17: Head to the Shadolean Great Temple that is being engulfed by a whirlpool
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.ShadoleanGreatTemple2)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.ShadoleanGreatTempleOMs);

        // Cutscene: The temple being consumed by darkness
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.ShadoleanGreatTemple2, 0, 0, QuestJumpType.After, Stage.AudienceChamber);

        // ========================================
        // STEP 18: Return to Lestania and speak with the White Dragon
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.TheWhiteDragon, 26107);

        // ========================================
        // STEP 19: Speak with Joseph
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 26108);

        // ========================================
        // STEP 20: Seek out Travers
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, 0, 0, NpcId.Travers0, 26109);

        // ========================================
        // STEP 21: Clear "The Great Dragon Crystal War: The Resisting Land" by accepting the challenge from Travers
        // Note: This would normally be a separate instance/mission check. For scripting purposes, we use a checkpoint.
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, 0, 1, NpcId.Travers0, 26110);

        // ========================================
        // STEP 22: Report to Travers
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, 0, 2, NpcId.Travers0, 26111);

        // ========================================
        // STEP 23: Seek out the White Dragon
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.TheWhiteDragon, 26112);

        // Cutscene: White Dragon grants power for the final confrontation
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.AudienceChamber, 41, 0);

        // ========================================
        // STEP 24: Head to the Shadolean Great Temple where the Arisen Corps awaits
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.ShadoleanGreatTemple2)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.ShadoleanGreatTempleOMs);

        // ========================================
        // STEP 25: Head to the location where the dark energy within the Great Temple has shown signs of change
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.DarknessShroudedShadoleanGreatTemple1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DarkTempleNpcs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DarkTempleOMs);

        // ========================================
        // STEP 26: Defeat the encountered demons
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 6)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnDarkTempleDemons);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 6, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndDarkTempleDemons);

        // Cutscene: The path to the Vortex opens
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.DarknessShroudedShadoleanGreatTemple1, 0, 0);

        // ========================================
        // STEP 27: Defeat the demons of the Vortex of Stagnation
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexNpcs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexOMs);

        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 7)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnVortexDemons);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 7, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndVortexDemons);

        // Victory cutscene
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.VortexofStagnation1, 0, 0, QuestJumpType.After, Stage.HollowofBeginnings1);

        // ========================================
        // STEP 28: Return to the Hollow of Beginnings
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.HollowofBeginnings1, 0, 0, 0);

        // Cutscene: Revelation about the true threat
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.HollowofBeginnings1, 0, 0, QuestJumpType.After, Stage.AudienceChamber);

        // ========================================
        // STEP 29-30: Return to Lestania and report to Joseph
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 26113)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.AudienceChamberNpcs2);

        // Final cutscene: Season 3.4 setup complete
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.AudienceChamber, 42, 0)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        process0.AddProcessEndBlock(true);

        // ========================================
        // Process 1: Handle Black Sword enemy cleanup
        // ========================================
        var process1 = AddNewProcess(1);
        process1.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.EndFirstBlackSword);
        process1.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 0,
            EnemyGroupId.Encounter + 1
        ]);
        process1.AddProcessEndBlock(false);

        // ========================================
        // Process 2: Handle second Black Sword cleanup
        // ========================================
        var process2 = AddNewProcess(2);
        process2.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.EndSecondBlackSword);
        process2.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 2,
            EnemyGroupId.Encounter + 3
        ]);
        process2.AddProcessEndBlock(false);

        // ========================================
        // Process 3: Handle third Black Sword cleanup
        // ========================================
        var process3 = AddNewProcess(3);
        process3.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.EndThirdBlackSword);
        process3.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 4,
            EnemyGroupId.Encounter + 5
        ]);
        process3.AddProcessEndBlock(false);

        // ========================================
        // Process 4: Handle Dark Temple and Vortex cleanup
        // ========================================
        var process4 = AddNewProcess(4);
        process4.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.EndVortexDemons);
        process4.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 6,
            EnemyGroupId.Encounter + 7
        ]);
        process4.AddProcessEndBlock(false);
    }
}

return new ScriptedQuest();
