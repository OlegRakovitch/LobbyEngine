using RattusEngine.Controllers;

namespace RattusEngine
{
    public interface IApplication
    {
        IContext Context { get; }
        RoomController RoomController { get; }
        UserController UserController { get; }
    }
}