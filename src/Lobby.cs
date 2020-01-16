using LobbyEngine.Controllers;

namespace LobbyEngine
{
    public class Lobby : ILobbyEngine
    {
        public IContext Context { get; private set; }
        public RoomController RoomController { get; private set; }
        public UserController UserController { get; private set; }
        public Lobby(IContext context, IGameStarter starter)
        {
            Context = context;
            RoomController = new RoomController(context, starter);
            UserController = new UserController(context);
        }
    }
}