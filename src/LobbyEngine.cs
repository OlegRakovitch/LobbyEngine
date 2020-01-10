using RattusEngine.Controllers;

namespace RattusEngine
{
    public class LobbyEngine : ILobbyEngine
    {
        public IContext Context { get; private set; }
        public RoomController RoomController { get; private set; }
        public UserController UserController { get; private set; }
        public LobbyEngine(IContext context, IGameStarter starter)
        {
            Context = context;
            RoomController = new RoomController(context, starter);
            UserController = new UserController(context);
        }
    }
}