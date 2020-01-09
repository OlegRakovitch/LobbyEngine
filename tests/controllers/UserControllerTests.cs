using System.Linq;
using RattusEngine.Controllers;
using RattusEngine.Models;
using Xunit;

namespace RattusEngine.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public void UserIsSavedAfterBeingRegistered()
        {
            var context = new ModifiableContext()
            {
                Storage = new MemoryStorage()
            };
            var userController = new UserController(context);
            Assert.Equal(Controllers.Statuses.UserRegisterStatus.OK, userController.Register("user"));
            var user = context.Storage.Get<User>().Single();
            Assert.Equal("user", user.Username);
        }
    }
}