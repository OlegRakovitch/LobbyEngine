using RattusEngine.Controllers;

namespace RattusEngine
{
    public class Application : IApplication
    {
        public IContext Context { get; private set; }
        public RoomController RoomController { get; private set; }
        public UserController UserController { get; private set; }
        public Application(IContext context)
        {
            Context = context;
            RoomController = new RoomController(context);
            UserController = new UserController(context);
        }
    }
}