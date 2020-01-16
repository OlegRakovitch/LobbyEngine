using LobbyEngine.Controllers;

namespace LobbyEngine
{
    public interface ILobbyEngine
    {
        IContext Context { get; }
        RoomController RoomController { get; }
        UserController UserController { get; }
    }
}