using System.Threading.Tasks;
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

        public async Task<UserRegisterStatus> Register(string username)
        {
            await context.Storage.Save(new User() { Username = username });
            return UserRegisterStatus.OK;
        }
    }
}