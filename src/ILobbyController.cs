using RattusEngine.Controllers;

namespace RattusEngine
{
    public interface ILobbyController
    {
        IContext Context { get; }
        RoomController RoomController { get; }
        UserController UserController { get; }
    }
}