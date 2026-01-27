/**
 * @brief Those Who Follow the Dragon
 * @desc Final quest of Season 3.3. Receive a revelation from the White Dragon.
 *       This is a cutscene-only epilogue quest with no combat.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.ThoseWhoFollowTheDragon;
    public override ushort RecommendedLevel => 100;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.BreakdownOfReason;

    private class QstLayoutFlag
    {
        // Audience Chamber - NPCs for revelation scene
        public const uint AudienceChamberNpcs = 8200;  // Joseph, Klaus, etc. for final scene
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(100));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.HopesBitterEnd));
    }

    protected override void InitializeRewards()
    {
        AddPointReward(PointType.ExperiencePoints, 900000);
        AddWalletReward(WalletType.Gold, 100000);
        AddWalletReward(WalletType.RiftPoints, 10000);

        AddFixedItemReward(ItemId.RoyalCrestMedalUrtecaDistrict, 5);
        AddFixedItemReward(ItemId.UnappraisedCloudTrinketGeneral, 2);
        AddFixedItemReward(ItemId.ApUrtecaMountains, 500);
        AddFixedItemReward(ItemId.BraceletOfTheFirstKing, 1);
    }

    protected override void InitializeEnemyGroups()
    {
        // No enemies - this is a cutscene-only quest
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Accept quest from The White Dragon
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.TheWhiteDragon, 26050)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason32);

        // Step 2: Watch the revelation cutscene
        process0.AddPlayEventBlock(QuestAnnounceType.Accept, Stage.AudienceChamber, 30, 0);

        // Step 3: Speak with The White Dragon after the revelation
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.TheWhiteDragon, 26051)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.AudienceChamberNpcs);

        // Step 4: Final cutscene - epilogue
        process0.AddPlayEventBlock(QuestAnnounceType.None, Stage.AudienceChamber, 31, 0);

        // Step 5: Report back to The White Dragon to complete the quest
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.TheWhiteDragon, 26052)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // Unlock tutorial for The Land of Despair: Section IV
        process0.AddIsStageNoBlock(QuestAnnounceType.None, Stage.AudienceChamber)
            .AddResultCmdTutorialDialog(TutorialId.TheLandofDespairSectionIV);

        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
