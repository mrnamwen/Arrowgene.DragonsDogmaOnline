using Arrowgene.Ddon.Server;
using Arrowgene.Ddon.Server.Network;
using Arrowgene.Ddon.Shared.Entity.PacketStructure;
using Arrowgene.Ddon.Shared.Network;
using Arrowgene.Logging;

namespace Arrowgene.Ddon.GameServer.Handler
{
    public class GpGpCourseGetAvailableListHandler : PacketHandler<GameClient>
    {
        private static readonly ServerLogger Logger = LogProvider.Logger<ServerLogger>(typeof(GpGpCourseGetAvailableListHandler));

        private readonly DdonGameServer _server;

        public GpGpCourseGetAvailableListHandler(DdonGameServer server) : base(server)
        {
            _server = server;
        }

        public override PacketId Id => PacketId.C2S_GP_GP_COURSE_GET_AVAILABLE_LIST_REQ;

        public override void Handle(GameClient client, IPacket packet)
        {
            S2CGpGpCourseGetAvailableListRes response = new S2CGpGpCourseGetAvailableListRes();

            // Get the available (purchased but not activated) courses for this character
            response.AvailableCourses = _server.GpCourseManager.GetAvailableCoursesInfo(client.Character.CharacterId);

            Logger.Debug($"Returning {response.AvailableCourses.Count} available courses for character {client.Character.CharacterId}");

            client.Send(response);
        }
    }
}
