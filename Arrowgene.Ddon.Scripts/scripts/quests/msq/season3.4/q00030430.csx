/**
 * @brief The White Dragon's Arisen
 * @desc Season 3.4 MSQ. Deal with the vortex of stagnation that appeared in
 *       Lestania by finding Fabio. Work with Knights including Ringdeel, Elliot,
 *       Heinz, Gerd, and Vanessa to destroy Black Swords at multiple locations.
 *       Culminates in a battle against the illusion of the Black Knight in the
 *       Vortex of Stagnation.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.TheWhiteDragonsArisen;
    public override ushort RecommendedLevel => 100;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.TheFateOfAll;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint VortexGuardian = 1960;          // Vortex Guardian <name>
        public const uint BlockingThePath = 1961;         // <name> Blocking the Path
        public const uint IllusionBlackKnight = 1962;     // Illusion of the Black Knight
    }

    private class QstLayoutFlag
    {
        // Dowe Valley - Jingen (Fabio's location)
        public const uint DoweValleyNpcs0 = 8700;         // Fabio at Jingen location

        // Erte Deenan (Black Sword location 1)
        public const uint ErteDeenanNpcs = 8701;          // NPCs at Erte Deenan
        public const uint ErteDeenanOMs = 8702;           // Black Sword OM

        // Gardnox Fortress (Black Sword location 2)
        public const uint GardnoxFortressNpcs0 = 8710;    // Elliot and allies
        public const uint GardnoxFortressNpcs1 = 8711;    // Heinz and Knights
        public const uint GardnoxFortressOMs = 8712;      // Black Sword OM

        // Temple of Purification (Gerd's location)
        public const uint TempleOfPurificationNpcs = 8720; // Gerd as decoy
        public const uint TempleOfPurificationOMs = 8721;  // Vortex barriers

        // Dreed Castle (Vanessa's location)
        public const uint DreedCastleNpcs0 = 8730;        // Vanessa and White Wings
        public const uint DreedCastleNpcs1 = 8731;        // Scherzo
        public const uint DreedCastleOMs = 8732;          // Gate OM

        // White Dragon Temple (Travers)
        public const uint WhiteDragonTempleNpcs = 8740;   // Travers

        // Vortex of Stagnation (Final battle)
        public const uint VortexOfStagnationNpcs0 = 8750; // Pre-battle setup
        public const uint VortexOfStagnationNpcs1 = 8751; // Battle setup
        public const uint VortexOfStagnationNpcs2 = 8752; // Post-battle NPCs
        public const uint VortexOfStagnationOMs = 8753;   // Arena barriers
    }

    private class MyQstFlag
    {
        public const uint SpawnErteDeenanEncounter = 1;
        public const uint EndErteDeenanEncounter = 2;
        public const uint SpawnGardnoxEncounter = 3;
        public const uint EndGardnoxEncounter = 4;
        public const uint SpawnTempleEncounter = 5;
        public const uint EndTempleEncounter = 6;
        public const uint SpawnDreedCastleEncounter = 7;
        public const uint EndDreedCastleEncounter = 8;
        public const uint SpawnBlackKnightIllusion = 9;
        public const uint BlackKnightIllusionComplete = 10;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(100));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.SpunTogetherHope));
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
        // ===== ENCOUNTER 1: Erte Deenan - Enemies guarding Black Sword =====
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.ErteDeenan, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowGoblin, 100, 0, isBoss: false)
                .SetNamedEnemyParams(NamedParamId.VortexGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowGoblinFighter, 100, 1)
                .SetNamedEnemyParams(NamedParamId.VortexGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowSlingGoblin, 100, 2)
                .SetNamedEnemyParams(NamedParamId.VortexGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowWolf, 100, 3)
                .SetNamedEnemyParams(NamedParamId.VortexGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowWolf, 100, 4)
                .SetNamedEnemyParams(NamedParamId.VortexGuardian),
        });

        // Black Sword at Erte Deenan
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.ErteDeenan, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword0, 100, 0, isBoss: true)
                .SetHpRate(50)
                .SetAttackRate(100)
                .SetDefenceRate(100),
        });

        // ===== ENCOUNTER 2: Gardnox Fortress - Enemies guarding Black Sword =====
        AddEnemies(EnemyGroupId.Encounter + 2, Stage.GardnoxFortress0, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.OrcSoldier0, 100, 0, isBoss: false)
                .SetNamedEnemyParams(NamedParamId.BlockingThePath),
            LibDdon.Enemy.CreateAuto(EnemyId.OrcAimer, 100, 1)
                .SetNamedEnemyParams(NamedParamId.BlockingThePath),
            LibDdon.Enemy.CreateAuto(EnemyId.OrcBanger, 100, 2)
                .SetNamedEnemyParams(NamedParamId.BlockingThePath),
            LibDdon.Enemy.CreateAuto(EnemyId.CaptainOrc0, 100, 3, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.BlockingThePath)
                .SetHpRate(150),
        });

        // Black Sword at Gardnox Fortress
        AddEnemies(EnemyGroupId.Encounter + 3, Stage.GardnoxFortress0, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword0, 100, 0, isBoss: true)
                .SetHpRate(50)
                .SetAttackRate(100)
                .SetDefenceRate(100),
        });

        // ===== ENCOUNTER 3: Temple of Purification - Illusion and Black Sword =====
        AddEnemies(EnemyGroupId.Encounter + 4, Stage.TempleofPurification, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackKnightPhantomClear, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.IllusionBlackKnight)
                .SetHpRate(200)
                .SetAttackRate(120)
                .SetDefenceRate(110),
        });

        // Black Sword at Temple of Purification
        AddEnemies(EnemyGroupId.Encounter + 5, Stage.TempleofPurification, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackSword0, 100, 0, isBoss: true)
                .SetHpRate(50)
                .SetAttackRate(100)
                .SetDefenceRate(100),
        });

        // ===== ENCOUNTER 4: Dreed Castle - Demons at the gate =====
        AddEnemies(EnemyGroupId.Encounter + 6, Stage.DreedCastle, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowGoblinLeader, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.VortexGuardian)
                .SetHpRate(150),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowGoblin, 100, 1)
                .SetNamedEnemyParams(NamedParamId.VortexGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowGoblinFighter, 100, 2)
                .SetNamedEnemyParams(NamedParamId.VortexGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.ShadowGrigori, 100, 3)
                .SetNamedEnemyParams(NamedParamId.VortexGuardian),
        });

        // ===== BOSS ENCOUNTER: Vortex of Stagnation - Black Knight Illusion =====
        AddEnemies(EnemyGroupId.Encounter + 7, Stage.VortexofStagnation1, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlackKnightPhantomOpaque, 100, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.IllusionBlackKnight)
                .SetHpRate(400)
                .SetAttackRate(150)
                .SetDefenceRate(130)
                .SetMagicAttackRate(150)
                .SetMagicDefenceRate(130),
        });

        // Prevent normal spawns during quest
        AddEnemies(EnemyGroupId.Encounter + 8, Stage.VortexofStagnation1, 0, QuestEnemyPlacementType.Manual, new()
        {
            /* prevent enemies from spawning */
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // ========================================
        // STEP 1: Head to Fabio's location in Jingen in the Dowe Valley
        // ========================================
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.TheWhiteDragon, 26500)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        process0.AddPartyGatherBlock(QuestAnnounceType.Accept, Stage.Lestania, 1, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DoweValleyNpcs0);

        // ========================================
        // STEP 2: Head to Erte Deenan and search for the Black Sword
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.Lestania, 0, 0, NpcId.Fabio0, 26501);

        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.ErteDeenan)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.ErteDeenanNpcs);

        // ========================================
        // STEP 3: Defeat the enemy and destroy the Black Sword
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnErteDeenanEncounter);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false);

        // Destroy the Black Sword
        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.ErteDeenanOMs);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndErteDeenanEncounter);

        // ========================================
        // STEP 4: Speak with Ringdeel
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.ErteDeenan, 0, 0, NpcId.Ringdeel0, 26502);

        // ========================================
        // STEP 5: Head to Gardnox Fortress and search for your comrades
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.GardnoxFortress0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.GardnoxFortressNpcs0);

        // ========================================
        // STEP 6: Rendezvous with Elliot
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.GardnoxFortress0, 0, 0, NpcId.Elliot0, 26503);

        // ========================================
        // STEP 7: Search for the Black Sword and the Knights led by Heinz
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.GardnoxFortress0, 1, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.GardnoxFortressNpcs1);

        // ========================================
        // STEP 8: Defeat the enemy and destroy the Black Sword
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnGardnoxEncounter);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 2, resetGroup: false);

        // Destroy the Black Sword
        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 3)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.GardnoxFortressOMs);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 3, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndGardnoxEncounter);

        // ========================================
        // STEP 9: Speak with Heinz
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.GardnoxFortress0, 0, 0, NpcId.Heinz2, 26504);

        // ========================================
        // STEP 10: Head to the Temple of Purification, where Gerd is acting as a decoy
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TempleofPurification)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.TempleOfPurificationNpcs);

        // ========================================
        // STEP 11: Head to the Vortex of Stagnation
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TempleofPurification, 1, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.TempleOfPurificationOMs);

        // ========================================
        // STEP 12: Defeat the illusion of the Black Knight and destroy the Black Sword
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 4)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnTempleEncounter);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 4, resetGroup: false);

        // Destroy the Black Sword
        process0.AddSpawnGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 5);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 5, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndTempleEncounter);

        // ========================================
        // STEP 13: Speak with Gerd
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TempleofPurification, 0, 0, NpcId.Gerd1, 26505);

        // ========================================
        // STEP 14: Head towards Dreed Castle, where Vanessa leads the White Wings Arisen Corps
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.DreedCastle)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DreedCastleNpcs0);

        // ========================================
        // STEP 15: Head to Travers in the White Dragon Temple
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.DreedCastle, 0, 0, NpcId.Vanessa0, 26506);

        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.WhiteDragonTempleNpcs);

        // ========================================
        // STEP 16: Clear "The Great Dragon Crystal War: Castle with No Lord" by accepting the challenge from Travers
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, 0, 0, NpcId.Travers0, 26507);

        // Note: The actual Grand Mission would be triggered here via NPC interaction
        // For now, we proceed after speaking with Travers

        // ========================================
        // STEP 17: Report to Travers
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, 0, 0, NpcId.Travers0, 26508);

        // ========================================
        // STEP 18: Head towards Scherzo waiting at Dreed Castle
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.DreedCastle)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DreedCastleNpcs1);

        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.DreedCastle, 0, 0, NpcId.Scherzo, 26509);

        // ========================================
        // STEP 19: Head towards the location of the gate that appeared in Dreed Castle
        // ========================================
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.DreedCastle, 1, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DreedCastleOMs);

        // ========================================
        // STEP 20: Defeat the encountered demons
        // ========================================
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 6)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnDreedCastleEncounter);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 6, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EndDreedCastleEncounter);

        // ========================================
        // STEP 21: Speak with Vanessa
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.DreedCastle, 0, 0, NpcId.Vanessa0, 26510);

        // ========================================
        // STEP 22: Plunge into the Vortex of Stagnation
        // ========================================
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexOfStagnationNpcs0);

        // Cutscene before final battle
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.VortexofStagnation1, 0, 0, QuestJumpType.After, Stage.VortexofStagnation1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexOfStagnationNpcs1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexOfStagnationOMs);

        // ========================================
        // STEP 23: Defeat the Black Knight
        // ========================================
        process0.AddSpawnGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 7)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SpawnBlackKnightIllusion);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 7, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.BlackKnightIllusionComplete);

        // Victory cutscene
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.VortexofStagnation1, 5, 0, QuestJumpType.After, Stage.VortexofStagnation1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Clear, QstLayoutFlag.VortexOfStagnationOMs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.VortexOfStagnationNpcs2);

        // ========================================
        // STEP 24: Speak with Vanessa
        // ========================================
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.VortexofStagnation1, 0, 0, NpcId.Vanessa0, 26511);

        // ========================================
        // STEP 25: Report to Joseph at the White Dragon Temple
        // ========================================
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TheWhiteDragonTemple0, NpcId.Joseph, 26512)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        process0.AddProcessEndBlock(true);

        // ========================================
        // Process 1: Handle Black Knight Illusion mechanics
        // ========================================
        var process1 = AddNewProcess(1);
        process1.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.SpawnBlackKnightIllusion);
        process1.AddRawBlock(QuestAnnounceType.None)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.BlackKnightIllusionComplete);
        process1.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 7
        ]);
        process1.AddProcessEndBlock(false);

        // ========================================
        // Process 2: Cleanup for Black Swords
        // ========================================
        var process2 = AddNewProcess(2);
        process2.AddMyQstFlagsBlock(QuestAnnounceType.None)
            .AddMyQstCheckFlag(MyQstFlag.EndErteDeenanEncounter);
        process2.AddRawBlock(QuestAnnounceType.None)
            .AddCheckCmdMyQstFlagOn(MyQstFlag.EndTempleEncounter);
        process2.AddRemoveGroupBlock(QuestAnnounceType.None, [
            EnemyGroupId.Encounter + 1,
            EnemyGroupId.Encounter + 3,
            EnemyGroupId.Encounter + 5
        ]);
        process2.AddProcessEndBlock(false);
    }
}

return new ScriptedQuest();
