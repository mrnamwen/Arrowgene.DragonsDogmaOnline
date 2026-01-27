/**
 * @brief The Missing Prince
 * @desc First quest of Season 3.3. Prince Nedo has gone missing after the
 *       recapture of Megado. Search for him at the Megado Detached Palace.
 */

#load "libs.csx"

public class ScriptedQuest : IQuest
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(ScriptedQuest));

    public override QuestType QuestType => QuestType.Main;
    public override QuestId QuestId => QuestId.TheMissingPrince;
    public override ushort RecommendedLevel => 95;
    public override byte MinimumItemRank => 0;
    public override bool IsDiscoverable => true;
    public override StageInfo StageInfo => Stage.AudienceChamber;
    public override QuestId NextQuestId => QuestId.NedosTrail;

    private class EnemyGroupId
    {
        public const uint Encounter = 10;
    }

    private class NamedParamId
    {
        public const uint DetachedPalaceGuard = 1790;       // Detached Palace Guard
        public const uint AncientGuardian = 1791;           // Ancient Guardian
    }

    private class QstLayoutFlag
    {
        // Lookout Castle (st0451)
        public const uint LookoutCastleNpcs = 6800;         // Meirova, Gillian, Gurdolin, Elliot

        // Megado Royal Palace Level (st0464)
        public const uint RoyalPalaceNpcs = 6801;           // Guide NPC for rendezvous

        // Megado Detached Palace (st0465)
        public const uint DetachedPalaceNpcs0 = 6802;       // Meirova at entrance
        public const uint DetachedPalaceNpcs1 = 6803;       // NPCs at lower level
        public const uint DetachedPalaceOMs = 6804;         // Altar and interactables
        public const uint Portcrystal = 6805;               // Portcrystal activation

        // Lookout Castle after return (st0451)
        public const uint LookoutCastleBertha = 6806;       // Bertha for report
    }

    private class MyQstFlag
    {
        public const uint EnemyEncounter = 1;
        public const uint AltarExamined = 2;
    }

    protected override void InitializeState()
    {
        AddQuestOrderCondition(QuestOrderCondition.MinimumLevel(95));
        AddQuestOrderCondition(QuestOrderCondition.MainQuestCompleted(QuestId.TheFinalBattleOfTheRoyalCapital));
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
        // Megado Detached Palace - Enemies encountered while searching for Nedo
        AddEnemies(EnemyGroupId.Encounter + 0, Stage.MegadoDetachedPalace, 1, QuestEnemyPlacementType.Manual, new()
        {
            LibDdon.Enemy.CreateAuto(EnemyId.GhostMail, 95, 0)
                .SetNamedEnemyParams(NamedParamId.DetachedPalaceGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.GhostMail, 95, 1)
                .SetNamedEnemyParams(NamedParamId.DetachedPalaceGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkSkeleton, 95, 2)
                .SetNamedEnemyParams(NamedParamId.DetachedPalaceGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkSkeleton, 95, 3)
                .SetNamedEnemyParams(NamedParamId.DetachedPalaceGuard),
            LibDdon.Enemy.CreateAuto(EnemyId.DarkSkeletonBrute, 95, 4, isBoss: true)
                .SetNamedEnemyParams(NamedParamId.AncientGuardian),
            LibDdon.Enemy.CreateAuto(EnemyId.GrudgeGhost, 95, 5)
                .SetNamedEnemyParams(NamedParamId.DetachedPalaceGuard),
        });

        // Prevent normal spawns in the dungeon during quest
        AddEnemies(EnemyGroupId.Encounter + 1, Stage.MegadoDetachedPalace, 0, QuestEnemyPlacementType.Manual, new()
        {
            /* prevent enemies from spawning */
        });
    }

    protected override void InitializeBlocks()
    {
        var process0 = AddNewProcess(0);

        // Step 1: Accept quest from Joseph - Consult about Prince Nedo who disappeared after recapture of Megado
        process0.AddNpcTalkAndOrderBlock(Stage.AudienceChamber, NpcId.Joseph, 26000)
            .AddQuestFlag(QuestFlagAction.Clear, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        // Step 2: Head to the Lookout Castle
        process0.AddPartyGatherBlock(QuestAnnounceType.Accept, Stage.LookoutCastle1, 0, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleNpcs);

        // Step 3: Speak with Meirova at Lookout Castle
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle1, 0, 0, NpcId.Meirova0, 26001);

        // Step 4: Rendezvous with the guide at the Megado Royal Palace Level
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.FortressCityMegadoRoyalPalaceLevel, 0, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.RoyalPalaceNpcs);

        // Step 5: Head to the Megado Detached Palace
        process0.AddIsStageNoBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadoDetachedPalace)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DetachedPalaceNpcs0);

        // Step 6: Head to the lowest level of the detached palace and search for Nedo
        process0.AddPartyGatherBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadoDetachedPalace, 1, 0, 0)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DetachedPalaceNpcs1);

        // Step 7: Defeat the enemy encountered
        process0.AddDiscoverGroupBlock(QuestAnnounceType.Update, EnemyGroupId.Encounter + 0);
        process0.AddDestroyGroupBlock(QuestAnnounceType.CheckpointAndUpdate, EnemyGroupId.Encounter + 0, resetGroup: false)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.EnemyEncounter);

        // Step 8: Examine the vicinity of the altar
        process0.AddOmInteractEventBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadoDetachedPalace, 2, 0, OmQuestType.MyQuest, OmInteractType.Release)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.DetachedPalaceOMs)
            .AddQuestFlag(QuestFlagType.MyQst, QuestFlagAction.Set, MyQstFlag.AltarExamined);

        // Step 9: Speak with Meirova
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.MegadoDetachedPalace, 0, 0, NpcId.Meirova0, 26002);

        // Step 10: Activate the portcrystal located in the place after leaving the detached palace
        process0.AddOmInteractEventBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.UrtecaMountains, 0, 0, OmQuestType.MyQuest, OmInteractType.Release)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.Portcrystal);

        // Step 11: Return to the Lookout Castle and speak with Bertha
        process0.AddNewTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.LookoutCastle1, 0, 0, NpcId.Bertha, 26003)
            .AddQuestFlag(QuestFlagType.QstLayout, QuestFlagAction.Set, QstLayoutFlag.LookoutCastleBertha);

        // Step 12: Return to Lestania and report to Joseph
        process0.AddTalkToNpcBlock(QuestAnnounceType.CheckpointAndUpdate, Stage.AudienceChamber, NpcId.Joseph, 26004)
            .AddQuestFlag(QuestFlagAction.Set, QuestFlags.AudienceChamber.TheCrewEndSeason34);

        process0.AddProcessEndBlock(true);
    }
}

return new ScriptedQuest();
