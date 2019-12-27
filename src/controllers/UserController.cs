using RattusEngine.Controllers.Statuses;
using RattusEngine.Models;

namespace RattusEngine.Controllers
{
    public class UserController
    {
        IContext context;

        public UserController(IContext context)
        {
            this.context = context;
        }

        public UserRegisterStatus Register(string username)
        {
            context.Storage.Save(new User() { Username = username });
            return UserRegisterStatus.OK;
        }
    }
}