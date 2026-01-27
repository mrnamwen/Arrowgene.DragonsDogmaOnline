/**
 * @brief A Brief Dragon Force
 * @desc Story: Black Knight wounded White Dragon, Arisen's power weakens.
 *       Receive a premonition of the awakening of Spirit Dragon Cecily and head towards Phindym.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.ABriefDragonForce;
    public override ushort RecommendedLevel => 92;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.ThePlightOfLookoutCastle;

    private class EnemyGroupId
    {
        public const uint TarieSmallTowerEncounter = 10;
        public const uint SageTowerRuinsEncounter = 20;
    }

    private class NamedParamId
    {
        public const uint AncientGuardian = 1800; // Ancient Guardian
    }

    private class QstLayoutFlag
    {
        // Hollow of Beginnings
        public const uint HollowOfBeginningsNpcs0 = 6600; // Adair, Gearoid
        public const uint HollowOfBeginningsNpcs1 = 6601; // Gearoid

        // Bloodbane Isle
        public const uint BloodbaneIsleBertrand = 6610;
    }

    private class MyQstFlag
    {
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(92));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.AnOmenOfDestruction));
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
        // Tarie Small Tower encounter - searching for ancient Dragon Force
        AddEnemies(EnemyGroupId.TarieSmallTowerEncounter, Stage.TarieSmallTower, 0, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.Golem, 92, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.AncientGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonMage, 92, 1)
                .SetNamedEnemyParams(NamedParamId.AncientGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.SkeletonMage, 92, 2)
                .SetNamedEnemyParams(NamedParamId.AncientGuardian),
        });

        // Sage Tower Ruins encounter - searching for Dragon Force on Bloodbane Isle
        AddEnemies(EnemyGroupId.SageTowerRuinsEncounter, Stage.SageTowerRuins, 0, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.Lich, 92, 0, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.AncientGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.UndeadMale, 92, 1)
                .SetNamedEnemyParams(NamedParamId.AncientGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.UndeadMale, 92, 2)
                .SetNamedEnemyParams(NamedParamId.AncientGuardian),
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Head to the Hollow of Beginnings on the continent of Phindym
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 25000)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);
        process0.AddIsStageNoBlock(QuestAnnounceType.Accept, Stage.HollowofBeginnings0);

        // Step 2: Speak with Adair Donnchadh
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.HollowofBeginnings0, 0, 0, NpcId.AdairDonnchadh0, 25001)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.HollowOfBeginningsNpcs0);

        // Step 3: Heading to the Tarie Small Tower in search of ancient Dragon Force
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TarieSmallTower);

        // Step 4: Search for Dragon Force and explore the interior
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.TarieSmallTowerEncounter);

        // Step 5: Defeat the encountered enemy
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.TarieSmallTowerEncounter, resetGroup: false);

        // Step 6: Proceed through the door and search for Dragon Force
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.TarieSmallTower, 0, 0, 500);
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.TarieSmallTower, 0, 0);

        // Step 7: Return to the Hollow of Beginnings and report to Gearoid
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.HollowofBeginnings0, 0, 0, NpcId.Gearoid0, 25002)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.HollowOfBeginningsNpcs1);

        // Step 8: Head to Bloodbane Isle and speak with Bertrand
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.BloodbaneIsle0, 0, 0, NpcId.Bertrand, 25003)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.BloodbaneIsleBertrand);

        // Step 9: Head to the Sage Tower Ruins on the island
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.SageTowerRuins);

        // Step 10: Search for Dragon Force and explore the interior
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.SageTowerRuinsEncounter);

        // Step 11: Defeat the encountered enemy
        process0.AddDestroyGroupBlock(QuestAnnounceType.Update, EnemyGroupId.SageTowerRuinsEncounter, resetGroup: false);

        // Step 12: Proceed through the door and search for Dragon Force
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.SageTowerRuins, 0, 0, 500);
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.SageTowerRuins, 0, 0);

        // Step 13: Return to the Hollow of Beginnings and report to Gearoid
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.HollowofBeginnings0, 0, 0, NpcId.Gearoid0, 25004);

        // Step 14: Observe the state of the Spirit Dragon
        process0.AddPlayEventBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.HollowofBeginnings0, 0, 0);

        // Step 15: Speak with Gearoid
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.HollowofBeginnings0, 0, 0, NpcId.Gearoid0, 25005);

        // Step 16: Return to Lestania
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 25006);

        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
