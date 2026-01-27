/**
 * @brief Nedo's Trail
 * @desc Track Prince Nedo, who is believed to have headed towards Firefall Mountain.
 *       Search for clues in the northern bandit community and assist bandits along the way.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.NedosTrail;
    public override ushort RecommendedLevel => 95;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.TheRoyalFamilyMausoleum;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint BanditAttacker = 1790;       // Bandit Attacker
        public const uint FlamingDemon = 1791;         // Flaming Demon
        public const uint TrailEnemy = 1792;           // Trail Enemy
    }

    private class QstLayoutFlag
    {
        // Lookout Castle (st0451)
        public const uint LookoutCastleNpcs0 = 6800;   // Meirova and crew at start

        // Northern Bandit Hideout (st0594)
        public const uint NorthernBanditNpcs0 = 6801;  // Cyril and bandits
        public const uint NorthernBanditNpcs1 = 6802;  // Bacias area master

        // Urteca Mountains (st0465)
        public const uint UrtecaNpcs0 = 6803;          // Bertha's subordinate
        public const uint UrtecaNpcs1 = 6804;          // Clue locations
        public const uint UrtecaNpcs2 = 6805;          // Assisting bandit group
        public const uint UrtecaNpcs3 = 6806;          // Fresh trail location
        public const uint UrtecaNpcs4 = 6807;          // Injured woman (Sly)
        public const uint UrtecaOMs = 6808;            // Barriers and portcrystal

        // Firefall Mountain Campsite (st0595)
        public const uint FirefallNpcs0 = 6809;        // Sly at campsite

        // Lookout Castle (st0451) - Return
        public const uint LookoutCastleNpcs1 = 6810;   // Meirova for report
    }

    private class MyQstFlag
    {
        public const uint FirstClueFound = 1;
        public const uint SecondClueFound = 2;
        public const uint BanditBattleComplete = 3;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(95));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.TheMissingPrince));
    }

    protected override void InitializeRewards()
    {
        AddPointReward(PointType.ExperiencePoints, 900000);
        AddWalletReward(WalletType.Gold, 100000);
        AddWalletReward(WalletType.RiftPoints, 10000);

        AddFixedItemReward(ItemId.RoyalCrestMedalUrtecaDistrict, 5);
        AddFixedItemReward(ItemId.UnappraisedCloudTrinketGeneral, 2);
        AddFixedItemReward(ItemId.ApUrtecaMountains, 500);
    }

    protected override void InitializeEnemyGroups()
    {
        // Group 0: Enemies attacking the bandits (rescue battle)
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.UrtecaMountains, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGoblinLeader, 95, 0)
                .SetNamedEnemyParams(NamedParamId.BanditAttacker),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGoblinFighter, 95, 1)
                .SetNamedEnemyParams(NamedParamId.BanditAttacker),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGoblinFighter, 95, 2)
                .SetNamedEnemyParams(NamedParamId.BanditAttacker),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGoblinSling, 95, 3)
                .SetNamedEnemyParams(NamedParamId.BanditAttacker),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGoblin, 95, 4)
                .SetNamedEnemyParams(NamedParamId.BanditAttacker),
            LibDdon.Enemy.CreateAuto(EnemyId.BlazeGoblin, 95, 5)
                .SetNamedEnemyParams(NamedParamId.BanditAttacker),
        });

        // Group 1: Enemy encountered on the fresh trail (Flaming enemies near Firefall)
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.UrtecaMountains, 2, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.FlameSkeletonCyclops, 95, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.FlamingDemon),
            LibDdon.Enemy.CreateAuto(EnemyId.FlameSkeleton, 93, 1)
                .SetNamedEnemyParams(NamedParamId.TrailEnemy),
            LibDdon.Enemy.CreateAuto(EnemyId.FlameSkeleton, 93, 2)
                .SetNamedEnemyParams(NamedParamId.TrailEnemy),
            LibDdon.Enemy.CreateAuto(EnemyId.FlameSkeletonBrute, 94, 3)
                .SetNamedEnemyParams(NamedParamId.TrailEnemy),
            LibDdon.Enemy.CreateAuto(EnemyId.FlameSkeletonBrute, 94, 4)
                .SetNamedEnemyParams(NamedParamId.TrailEnemy),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Accept quest from Joseph
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 23000)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // Step 2: Head to the Lookout Castle
        process0.AddIsStageNoBlock(QuestAnnounceType.Accept, Stage.LookoutCastle1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs0);

        // Step 3: Head to the northern bandit community in the Urteca Mountains
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.NorthernBanditHideout)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.NorthernBanditNpcs0);

        // Step 4: Speak with the northern bandit leader Cyril
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.NorthernBanditHideout, 0, 0, NpcId.Cyril, 23010);

        // Step 5: Speak with Bacias, the Area Master
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.NorthernBanditHideout, 0, 1, NpcId.Bacias, 23020)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.NorthernBanditNpcs1);

        // Step 6: Activate the portcrystal
        process0.AddCollectItemBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.NorthernBanditHideout, 0, 1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.UrtecaOMs);

        // Step 7: Head to confirm the search situation
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.UrtecaNpcs0);

        // Step 8: Rendezvous with Bertha's subordinate
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 0, 0, NpcId.BerthasBanditGroup0, 23030)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.UrtecaNpcs1);

        // Step 9: Search for clues to the whereabouts of the abducted prince (2 remaining)
        process0.AddCollectItemBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 1, 1)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.FirstClueFound);

        // Step 10: Search for clues to the whereabouts of the abducted prince (1 remaining)
        process0.AddCollectItemBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 2, 1)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.SecondClueFound)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.UrtecaNpcs2);

        // Cutscene: Bandits under attack
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.UrtecaMountains, 0, 0, QuestJumpType.None, Stage.Invalid);

        // Step 11: Eliminate the enemies and assist the bandits
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 0);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.BanditBattleComplete);

        // Step 12: Speak with the assisting bandit group member
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 0, 2, NpcId.BerthasBanditGroup1, 23040)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.UrtecaNpcs3);

        // Step 13: Head to the location of the fresh trail
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 3, 0, 0);

        // Step 14: Defeat the enemy encountered
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 1);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 1, resetGroup: false)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.UrtecaNpcs4);

        // Step 15: Speak with the injured woman
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 0, 3, NpcId.InjuredMan, 23050);

        // Step 16: Speak with Sly
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 0, 4, NpcId.Sly, 23060)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FirefallNpcs0);

        // Step 17: Return to the northern bandit community
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.NorthernBanditHideout);
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.Update, Stage.NorthernBanditHideout, 0, 2, NpcId.Cyril, 23070);

        // Step 18: Return to the Lookout Castle and report to Meirova
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs1);
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.Update, Stage.LookoutCastle1, 0, 0, NpcId.Meirova0, 23080);

        // Step 19: Return to Lestania and report to Joseph
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 23090)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
