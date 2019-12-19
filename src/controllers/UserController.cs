namespace RattusEngine
{
    public class UserController
    {
        IContext context;

        public UserController(IContext context)
        {
            this.context = context;
        }

        public bool CreateUser(string username)
        {
            return context.Storage.Save(new User() { Username = username });
        }
    }
}