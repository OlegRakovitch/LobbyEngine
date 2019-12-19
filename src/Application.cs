using System;

namespace RattusEngine
{
    public class Application
    {
        public readonly IContext Context;
        public readonly RoomController RoomController;
        public readonly UserController UserController;
        public Application(IContext context)
        {
            Context = context;
            RoomController = new RoomController(context);
            UserController = new UserController(context);
        }
    }
}