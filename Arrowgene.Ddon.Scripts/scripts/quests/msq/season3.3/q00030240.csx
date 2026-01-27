/**
 * @brief The Dreadful Passage
 * @desc Follow the Prince to the mouth of Firefall Mountain
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.TheDreadfulPassage;
    public override ushort RecommendedLevel => 97;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.TheRelicsOfTheFirstKing;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint MountainGuard = 1790;        // Mountain Guard
        public const uint FirefallBeast = 1791;        // Firefall Beast
    }

    private class QstLayoutFlag
    {
        // Lookout Castle (st0451)
        public const uint LookoutCastleNpcs0 = 8200;   // Gillian, Gurdolin at Lookout Castle

        // Urteca Mountains (st0465)
        public const uint UrtecaNpcs0 = 8201;          // Gillian at mountain foothills
        public const uint UrtecaNpcs1 = 8202;          // Gillian at Firefall entrance

        // Firefall Mountain Campsite (st0595)
        public const uint FirefallCampsiteNpcs = 8203; // Gurdolin at campsite
        public const uint FirefallCampsiteOMs = 8204;  // Portcrystal activation
    }

    private class MyQstFlag
    {
        public const uint EnemyEncounterComplete = 1;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(97));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.TheRoyalFamilyMausoleum));
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
        // Group 0: Enemies blocking path on mountain foothills (Step 3)
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.UrtecaMountains, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedGorecyclops, 97, 0, isBoss: true)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.MountainGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedWarg, 95, 1)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.MountainGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedWarg, 95, 2)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.MountainGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.SeverelyInfectedWarg, 95, 3)
                .SetInfectionType(1)
                .SetNamedEnemyParams(NamedParamId.MountainGuard),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Head to the Lookout Castle
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 24000)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        process0.AddIsStageNoBlock(QuestAnnounceType.Accept, Stage.LookoutCastle1)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs0);

        // Step 2: Head towards the foothills of the northern part of the Urteca Mountains, towards Firefall Mountain
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.UrtecaNpcs0);

        // Step 3: Strike down the enemy that stands in your way
        process0.AddDiscoverGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 0);
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EnemyEncounterComplete);

        // Step 4: Speak with Gillian
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 0, 0, NpcId.Gillian0, 24010);

        // Step 5: Head towards the entrance of Firefall Mountain, beyond the foot of the mountain
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 1, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.UrtecaNpcs1);

        // Step 6: Speak with Gillian
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 0, 1, NpcId.Gillian0, 24020);

        // Step 7: Activate the portcrystal
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FirefallMountainCampsite)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FirefallCampsiteNpcs)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.FirefallCampsiteOMs);

        process0.AddOmInteractBlock(QuestAnnounceType.Update, Stage.FirefallMountainCampsite, 1);

        // Step 8: Speak with Gurdolin
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FirefallMountainCampsite, 0, 0, NpcId.Gurdolin0, 24030);

        // Step 9: Return to Lestania and report to Joseph
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 24040)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
