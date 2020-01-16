using System.Linq;
using LobbyEngine.Controllers;
using LobbyEngine.Models;
using Xunit;

namespace LobbyEngine.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public async void UserIsSavedAfterBeingRegistered()
        {
            var context = new ModifiableContext()
            {
                Storage = new MemoryStorage()
            };
            var userController = new UserController(context);
            Assert.Equal(Controllers.Statuses.UserRegisterStatus.OK, await userController.Register("user"));
            var user = (await context.Storage.Get<User>()).Single();
            Assert.Equal("user", user.Username);
        }
    }
}