public class NpcExtendedFacility : INpcExtendedFacility
{
    public NpcExtendedFacility()
    {
        NpcId = NpcId.Seneka0;
    }

    public override void GetExtendedOptions(DdonGameServer server, GameClient client, S2CNpcGetNpcExtendedFacilityRes result)
    {
        result.ExtendedMenuItemList.Add(new CDataNpcExtendedFacilityMenuItem()
        {
            FunctionClass = NpcFunction.LargeDeliveryEvent,
            FunctionSelect = NpcFunction.LargeDeliveryEvent
        });
    }
}

return new NpcExtendedFacility();
