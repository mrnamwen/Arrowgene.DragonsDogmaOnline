/**
 * @brief The Fate of All
 * @desc THE FINAL QUEST OF DRAGON'S DOGMA ONLINE. The ultimate confrontation
 *       with the Black Dragon awaits in the Vortex of Stagnation. Fight through
 *       LEO (who surrenders the Water Dragon's power) before the climactic
 *       two-phase battle against the Black Dragon itself. This is THE END.
 *
 *       Phase 1: Attack the Black Dragon's chest, it enters rage mode at 50% HP
 *       Phase 2: Darkness phase with shield mechanic and void crystal
 *       Dragon Abilities auto-activate during the final battle
 *
 *       NPCs: Gurdolin, Lise, Elliot, Mysial, Joseph, Klaus, Theodor, Travers, Beatrix
 *       This quest has NO successor - it is the conclusion of the game's story.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.TheFateOfAll;
    public override ushort RecommendedLevel => 100;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.TheWhiteDragonTemple0;
    public override QuestId NextQuestId => QuestId.None;  // THE END - No more quests

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint VortexDemon = 1960;            // Vortex of Stagnation Demon
        public const uint BlackSwordGuardian = 1961;     // Black Sword Guardian
        public const uint DarkLeo = 1962;                // Dark Leo - Former Commander
        public const uint TheBlackDragon = 1963;         // The Black Dragon - FINAL BOSS
        public const uint VoidCrystal = 1964;            // Void Crystal (Phase 2)
    }

    private class QstLayoutFlag
    {
        // White Dragon Temple (st0200)
        public const uint WhiteDragonTempleNpcs0 = 8700;  // Joseph, Klaus at Temple start
        public const uint WhiteDragonTempleNpcs1 = 8701;  // NPCs during preparation

        // Zandora Wastelands (st0100)
        public const uint ZandoraWastelandsNpcs = 8710;   // Theodor and expedition party

        // Mergoda Ruins Royal Palace Level (st0418)
        public const uint MergodaRuinsNpcs0 = 8720;       // Beatrix and search party
        public const uint MergodaRuinsOMs0 = 8721;        // First Black Sword barrier
        public const uint MergodaRuinsOMs1 = 8722;        // Second Black Sword barrier

        // Mergoda Security District (st0411)
        public const uint MergodaSecurityNpcs0 = 8730;    // Lise, Gurdolin, Elliot waiting
        public const uint MergodaSecurityNpcs1 = 8731;    // After rendezvous
        public const uint MergodaSecurityOMs = 8732;      // Gate to Vortex

        // The Great Dragon Crystal War Instance
        public const uint TraversNpcs = 8740;             // Travers at White Dragon Temple

        // Vortex of Stagnation (st0430/st0431)
        public const uint VortexNpcs0 = 8750;             // Mysial and party at entry
        public const uint VortexNpcs1 = 8751;             // During demon encounter
        public const uint VortexOMs0 = 8752;              // Vortex barriers

        // Black Dragon's Domain - Final Arena
        public const uint FinalArenaNpcs0 = 8760;         // Leo confrontation setup
        public const uint FinalArenaNpcs1 = 8761;         // After Leo defeated
        public const uint FinalArenaNpcs2 = 8762;         // Black Dragon battle setup
        public const uint FinalArenaNpcs3 = 8763;         // Post-victory scene
        public const uint FinalArenaOMs0 = 8764;          // Leo arena barriers
        public const uint FinalArenaOMs1 = 8765;          // Black Dragon arena barriers
        public const uint FinalArenaOMs2 = 8766;          // Void Crystal (Phase 2)
    }

    private class MyQstFlag
    {
        // Black Sword encounters
        public const uint SpawnBlackSwordEnemy1 = 1;
        public const uint EndBlackSwordEnemy1 = 2;
        public const uint SpawnBlackSwordEnemy2 = 3;
        public const uint EndBlackSwordEnemy2 = 4;

        // Vortex demon encounter
        public const uint SpawnVortexDemons = 5;
        public const uint EndVortexDemons = 6;

        // LEO PRE-BOSS
        public const uint SpawnLeo = 7;
        public const uint LeoDefeated = 8;

        // BLACK DRAGON FINAL BOSS
        public const uint SpawnBlackDragonPhase1 = 9;
        public const uint BlackDragonPhase1Complete = 10;
        public const uint SpawnBlackDragonPhase2 = 11;
        public const uint BlackDragonPhase2Complete = 12;  // THE END

        // Dragon Abilities activation
        public const uint DragonAbilitiesActive = 13;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(100));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.TheWhiteDragonsArisen));
    }

    protected override void InitializeRewards()
    {
        // ULTIMATE REWARDS FOR COMPLETING THE GAME
        AddPointReward(PointType.ExperiencePoints, 1000000);  // 1 MILLION XP
        AddWalletReward(WalletType.Gold, 100000);
        AddWalletReward(WalletType.RiftPoints, 10000);

        // Keystone to Ruin x10
        AddFixedItemReward(ItemId.KeystoneToRuin, 10);
        // Ring of Order x1 - UNIQUE FINAL REWARD
        AddFixedItemReward(ItemId.RingOfOrder, 1);
    }

    protected override void InitializeEnemyGroups()
    {
        // ===== ENCOUNTER 1: Black Sword Guardian at Mergoda Ruins =====
        // Demons guarding the first Black Sword
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.MergodaRuinsRoyalPalaceLevel0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.DarkCorpsePunisher, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian)
                .SetHpRate(150)
                .SetAttackRate(130),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkCorpseTorturer, 100, 1)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkCorpseTorturer, 100, 2)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkSkeleton, 100, 3)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkSkeleton, 100, 4)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
        });

        // Black Sword 1
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.MergodaRuinsRoyalPalaceLevel0, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword0, 100, 0)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
        });

        // ===== ENCOUNTER 2: Black Sword Guardian at Second Location =====
        AddEnemies(EnemyGroupId.Encounter + 2, Stage.MergodaRuinsRoyalPalaceLevel0, 3, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.DarkCorpsePunisher, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian)
                .SetHpRate(150)
                .SetAttackRate(130),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkSkeletonBrute, 100, 1)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkSkeletonBrute, 100, 2)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SkullLord, 100, 3)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
        });

        // Black Sword 2
        AddEnemies(EnemyGroupId.Encounter + 3, Stage.MergodaRuinsRoyalPalaceLevel0, 4, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword0, 100, 0)
                .SetNamedEnemyParams(NamedParamId.BlackSwordGuardian),
        });

        // ===== ENCOUNTER 3: Vortex Demons at Mergoda Security District =====
        AddEnemies(EnemyGroupId.Encounter + 4, Stage.MergodaSecurityDistrict0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.Eliminator, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.VortexDemon)
                .SetHpRate(200)
                .SetAttackRate(140),
            LibDdon.Enemy.CreateAuto(EnemyId.EliminatorSlay, 100, 1)
                .SetNamedEnemyParams(NamedParamId.VortexDemon),
            LibDdon.Enemy.CreateAuto(EnemyId.Sludgeman, 100, 2)
                .SetNamedEnemyParams(NamedParamId.VortexDemon),
            LibDdon.Enemy.CreateAuto(EnemyId.Sludgeman, 100, 3)
                .SetNamedEnemyParams(NamedParamId.VortexDemon),
        });

        // ===== PRE-BOSS: DARK LEO =====
        // Leo, corrupted by the Water Dragon's power, fights with ice attacks
        // Upon defeat, surrenders the Water Dragon's power to the Arisen
        AddEnemies(EnemyGroupId.Encounter + 5, Stage.VortexofStagnation0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.Leo, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.DarkLeo)
                .SetHpRate(350)       // Tough but not impossible
                .SetAttackRate(160)
                .SetDefenceRate(130)
                .SetMagicAttackRate(180)  // Strong ice magic
                .SetMagicDefenceRate(140),
        });

        // ===== FINAL BOSS: THE BLACK DRAGON - PHASE 1 =====
        // First form - Attack the chest, enters rage mode at 50% HP
        AddEnemies(EnemyGroupId.Encounter + 6, Stage.VortexofStagnation1, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackDragon1stForm1, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.TheBlackDragon)
                .SetHpRate(500)       // MASSIVE HP pool
                .SetAttackRate(180)
                .SetDefenceRate(150)
                .SetMagicAttackRate(200)
                .SetMagicDefenceRate(150),
        });

        // ===== FINAL BOSS: THE BLACK DRAGON - PHASE 2 =====
        // Second form - Darkness phase, shield mechanic, void crystal
        // Dragon Abilities auto-activate to help the Arisen
        AddEnemies(EnemyGroupId.Encounter + 7, Stage.VortexofStagnation1, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackDragon2ndForm, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.TheBlackDragon)
                .SetHpRate(600)       // EVEN MORE HP - THE ULTIMATE CHALLENGE
                .SetAttackRate(200)
                .SetDefenceRate(160)
                .SetMagicAttackRate(220)  // Devastating dark attacks
                .SetMagicDefenceRate(160),
        });

        // Void Crystal - must be destroyed during Phase 2 darkness mechanic
        AddEnemies(EnemyGroupId.Encounter + 8, Stage.VortexofStagnation1, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.DragonCrystalOfDestruction, 100, 0)
                .SetNamedEnemyParams(NamedParamId.VoidCrystal)
                .SetHpRate(100),
        });

        // Prevent normal spawns
        AddEnemies(EnemyGroupId.Encounter + 9, Stage.VortexofStagnation0, 0, QuestEnemyPlacementType.Manual, new()
        {
            /* prevent enemies from spawning */
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // ========================================
        // STEP 1: Speak with Joseph at the White Dragon Temple
        // "The Vortex of Stagnation has appeared. We must act now."
        // ========================================
        process0.AddNpcTalkAndOrderBlock(Stage.TheWhiteDragonTemple0, NpcId.Joseph, 27000)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.WhiteDragonTempleNpcs0);

        // ========================================
        // STEP 2: Speak with Klaus for tactical briefing
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.Accept, Stage.TheWhiteDragonTemple0, 0, 1, NpcId.Klaus0, 27001);

        // ========================================
        // STEP 3: Head to Zandora Wastelands where the Vortex can be observed
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.Lestania, 0, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.ZandoraWastelandsNpcs);

        // ========================================
        // STEP 4: Head to Theodor in the Mergoda Ruins
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MergodaRuinsRoyalPalaceLevel0, 0, 0, NpcId.Theodor, 27002)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MergodaRuinsNpcs0);

        // ========================================
        // STEP 5: Search for the Black Sword in Mergoda Ruins Royal Palace
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MergodaRuinsRoyalPalaceLevel0, 1, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MergodaRuinsOMs0);

        // ========================================
        // STEP 6: ENCOUNTER 1 - Defeat enemy and destroy the Black Sword
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnBlackSwordEnemy1);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false);
        // Destroy Black Sword
        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndBlackSwordEnemy1);

        // ========================================
        // STEP 7: Speak with Beatrix
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MergodaRuinsRoyalPalaceLevel0, 1, 1, NpcId.Beatrix, 27003);

        // ========================================
        // STEP 8: Head towards next location and search for Black Sword
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MergodaRuinsRoyalPalaceLevel0, 2, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MergodaRuinsOMs1);

        // ========================================
        // STEP 9: ENCOUNTER 2 - Defeat enemy and destroy the Black Sword
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnBlackSwordEnemy2);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2, resetGroup: false);
        // Destroy Black Sword
        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 3);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 3, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndBlackSwordEnemy2);

        // ========================================
        // STEP 10: Speak with Theodor
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MergodaRuinsRoyalPalaceLevel0, 2, 1, NpcId.Theodor, 27004);

        // ========================================
        // STEP 11: Head to Lise and others at Mergoda Security District
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MergodaSecurityDistrict0, 0, 0, NpcId.Lise0, 27005)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MergodaSecurityNpcs0);

        // ========================================
        // STEP 12: Head to Travers in the White Dragon Temple
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, 0, 2, NpcId.Travers0, 27006)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.TraversNpcs);

        // ========================================
        // STEP 13: Clear "The Great Dragon Crystal War: The Sacrificed Capital"
        // (Instance content - simplified for quest flow)
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0);

        // ========================================
        // STEP 14: Report to Travers
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, NpcId.Travers0, 27007);

        // ========================================
        // STEP 15: Rendezvous with Lise and others at Mergoda Security District
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MergodaSecurityDistrict0, 0, 1, NpcId.Lise0, 27008)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MergodaSecurityNpcs1);

        // ========================================
        // STEP 16: Head towards the gate at Mergoda Security District
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MergodaSecurityDistrict0, 1, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.MergodaSecurityOMs);

        // ========================================
        // STEP 17: ENCOUNTER 3 - Defeat the encountered demons
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 4)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnVortexDemons);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 4, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndVortexDemons);

        // ========================================
        // STEP 18: Speak with Mysial
        // "The Vortex awaits. Your destiny lies within."
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MergodaSecurityDistrict0, 1, 1, NpcId.Mysial0, 27009)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexNpcs0);

        // ========================================
        // STEP 19: Plunge into the Vortex of Stagnation
        // Cutscene: Entering the Vortex, the realm between worlds
        // ========================================
        process0.AddPlayEventBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation0, 0, 0, QuestJumpType.After, Stage.VortexofStagnation0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexOMs0);

        // ========================================
        // STEP 20: Report to Travers (brief checkpoint)
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation0);

        // ========================================
        // STEP 21: Seek out the Black Dragon
        // Navigate through the Vortex, encounter LEO
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation0, 1, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FinalArenaNpcs0);

        // Cutscene: Leo confrontation
        // "Arisen... I cannot let you pass. The Water Dragon's power compels me..."
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.VortexofStagnation0, 5, 0, QuestJumpType.None, Stage.Invalid)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FinalArenaOMs0);

        // ===== PRE-BOSS BATTLE: LEO =====
        // Leo, corrupted by the Water Dragon's power, ice-based attacks
        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 5)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnLeo);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 5, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.LeoDefeated);

        // Cutscene: Leo defeated, surrenders Water Dragon power
        // "You have... freed me. Take this power. Defeat the Black Dragon!"
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.VortexofStagnation0, 10, 0, QuestJumpType.After, Stage.VortexofStagnation1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.FinalArenaOMs0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FinalArenaNpcs1);

        // ========================================
        // STEP 22: FINAL BATTLE - Defeat the Black Dragon
        // THE CLIMAX OF THE ENTIRE GAME
        // ========================================

        // Cutscene: Black Dragon appears
        // The ground shakes. Darkness coalesces. The Black Dragon reveals itself.
        process0.AddPlayEventBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation1, 0, 0, QuestJumpType.None, Stage.Invalid)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FinalArenaNpcs2)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FinalArenaOMs1);

        // ===== FINAL BOSS PHASE 1: THE BLACK DRAGON =====
        // Attack the chest, enters rage mode at 50% HP
        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 6)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnBlackDragonPhase1)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.DragonAbilitiesActive);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 6, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.BlackDragonPhase1Complete);

        // ===== PHASE TRANSITION CUTSCENE =====
        // The Black Dragon roars in agony, darkness erupts from its core
        // "You think you have won? I AM ETERNAL!"
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.VortexofStagnation1, 5, 0, QuestJumpType.None, Stage.Invalid)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.FinalArenaNpcs2)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FinalArenaOMs2);

        // ===== FINAL BOSS PHASE 2: THE BLACK DRAGON - DARKNESS FORM =====
        // Shield mechanic active, Void Crystal must be destroyed
        // Dragon Abilities surge - all powers of the White Dragon, Spirit Dragons,
        // and Water Dragon combine to aid the Arisen
        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 7)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnBlackDragonPhase2);
        // Spawn Void Crystal for shield mechanic
        process0.AddSpawnGroupBlock(QuestAnnounceType.None, EnemyGroupId.Encounter + 8);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 7, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.BlackDragonPhase2Complete);

        // ===== VICTORY CUTSCENE - THE END =====
        // The Black Dragon falls. Light returns to the world.
        // All allies appear - Gurdolin, Lise, Elliot, Mysial, Joseph, Klaus, and more
        // The Arisen stands victorious. Lestania is saved.
        // "You have done it, Arisen. The fate of all... was in your hands."
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.VortexofStagnation1, 20, 0, QuestJumpType.After, Stage.TheWhiteDragonTemple0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.FinalArenaOMs1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.FinalArenaOMs2)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FinalArenaNpcs3);

        // ===== EPILOGUE - Return to White Dragon Temple =====
        // Unlock final tutorial: "The Land of Despair: Section V" - journey complete
        process0.AddIsStageNoBlock(QuestAnnounceType.None, Stage.TheWhiteDragonTemple0)
            .AddResultCmdTutorialDialog(TutorialId.TheLandofDespairSectionV);

        // Final conversation with The White Dragon
        // "Arisen... you have saved us all. Your legend will echo through eternity."
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.TheWhiteDragon, 27099);

        // ===== THE END =====
        process0.AddProcessEndBlock(true);

        // ========================================
        // Process 1: Black Dragon Phase 1 mechanics
        // Monitors HP for rage mode trigger at 50%
        // ========================================
        var process1 = AddNewProcess(1);
        process1.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.SpawnBlackDragonPhase1);
        // Monitor HP for phase transition (at 1% HP, trigger transition instead of death)
        process1.AddRawBlock(QuestAnnounceType.None)
            .AddCheckCmdEmHpLess(Stage.VortexofStagnation1, 1, 0, 1);
        process1.AddProcessEndBlock(false);

        // ========================================
        // Process 2: Black Dragon Phase 2 mechanics
        // Void Crystal shield mechanic management
        // ========================================
        var process2 = AddNewProcess(2);
        process2.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.SpawnBlackDragonPhase2);
        process2.AddRawBlock(QuestAnnounceType.None)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.BlackDragonPhase2Complete);
        process2.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 6,
            EnemyGroupId.Encounter + 7,
            EnemyGroupId.Encounter + 8
        ]);
        process2.AddProcessEndBlock(false);

        // ========================================
        // Process 3: Dragon Abilities activation during final battle
        // All Dragon powers (White Dragon, Spirit Dragons, Water Dragon)
        // automatically assist the Arisen
        // ========================================
        var process3 = AddNewProcess(3);
        process3.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.DragonAbilitiesActive);
        // Dragon Abilities remain active until battle complete
        process3.AddRawBlock(QuestAnnounceType.None)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.BlackDragonPhase2Complete);
        process3.AddProcessEndBlock(false);

        // ========================================
        // Process 4: Leo encounter cleanup
        // ========================================
        var process4 = AddNewProcess(4);
        process4.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.LeoDefeated);
        process4.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 5
        ]);
        process4.AddProcessEndBlock(false);
    }
}

return new ScriptedQuest();
