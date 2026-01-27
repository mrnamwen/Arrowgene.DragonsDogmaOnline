#nullable enable
using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler;

public class GpCourseGetValidListHandler : GameRequestPacketHandler<C2SGpCourseGetValidListReq, S2CGpCourseGetValidListRes>
{
    private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(GpCourseGetValidListHandler));

    public GpCourseGetValidListHandler(DdonGameServer server) : base(server)
    {
    }

    public override S2CGpCourseGetValidListRes Handle(GameClient client, C2SGpCourseGetValidListReq request)
    {
        var res = new S2CGpCourseGetValidListRes();

        // Get all valid (currently active) courses for this character
        // This combines server-wide courses and character's personal active courses
        res.Items = Server.GpCourseManager.GetValidCoursesForCharacter(client.Character.CharacterId);

        Logger.Debug($"Returning {res.Items.Count} valid courses for character {client.Character.CharacterId}");

        return res;
    }
}
