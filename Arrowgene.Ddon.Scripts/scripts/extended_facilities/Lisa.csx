public class NpcExtendedFacility : INpcExtendedFacility
{
    public NpcExtendedFacility()
    {
        NpcId = NpcId.Lisa;
    }

    public override void GetExtendedOptions(DdonGameServer server, GameClient client, S2CNpcGetNpcExtendedFacilityRes result)
    {
        // Add Reward Mission menu option
        result.ExtendedMenuItemList.Add(new CDataNpcExtendedFacilityMenuItem()
        {
            FunctionClass = NpcFunction.RewardMission,
            FunctionSelect = NpcFunction.RewardMission
        });

        // Add Reward Medal Exchange menu option
        result.ExtendedMenuItemList.Add(new CDataNpcExtendedFacilityMenuItem()
        {
            FunctionClass = NpcFunction.RewardMedalExchange,
            FunctionSelect = NpcFunction.RewardMedalExchange
        });
    }
}

return new NpcExtendedFacility();
