#nullable enable
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler;

public class GpCourseUseFromAvailableHandler : GameRequestPacketHandler<C2SGpCourseUseFromAvailableReq, S2CGpCourseUseFromAvailableRes>
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(GpCourseUseFromAvailableHandler));

    public GpCourseUseFromAvailableHandler(DdonGameServer server) : base(server)
    {
    }

    public override S2CGpCourseUseFromAvailableRes Handle(GameClient client, C2SGpCourseUseFromAvailableReq request)
    {
        var res = new S2CGpCourseUseFromAvailableRes();

        // Activate the course from the available list
        ulong endTime = Server.GpCourseManager.ActivateCourse(client, request.AvailableId);

        if (endTime == 0)
        {
            Logger.Error($"Failed to activate course with available ID {request.AvailableId} for character {client.Character.CharacterId}");
            res.Error = 1; // Indicate failure
        }
        else
        {
            res.FinishDateTime = endTime;
            Logger.Info($"Character {client.Character.CharacterId} activated course (available ID: {request.AvailableId}), expires at {endTime}");
        }

        return res;
    }
}
