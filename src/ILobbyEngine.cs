using RattusEngine.Controllers;

namespace RattusEngine
{
    public interface ILobbyEngine
    {
        IContext Context { get; }
        RoomController RoomController { get; }
        UserController UserController { get; }
    }
}