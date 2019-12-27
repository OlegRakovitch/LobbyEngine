using System;
using RattusEngine.Controllers.Statuses;

namespace RattusEngine.Fixtures
{
    public class GivenPlayers
    {
        public string Username;

        public void Execute()
        {
            var status = Common.App.UserController.Register(Username);
            if (status != UserRegisterStatus.OK)
            {
                throw new Exception($"User {Username} wasn't registered: {status}");
            }
        }
    }
}