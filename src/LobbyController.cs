using RattusEngine.Controllers;

namespace RattusEngine
{
    public class LobbyController : ILobbyController
    {
        public IContext Context { get; private set; }
        public RoomController RoomController { get; private set; }
        public UserController UserController { get; private set; }
        public LobbyController(IContext context, IGameStarter starter)
        {
            Context = context;
            RoomController = new RoomController(context, starter);
            UserController = new UserController(context);
        }
    }
}