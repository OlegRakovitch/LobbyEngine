using System;
using RattusEngine.Controllers.Statuses;

namespace RattusEngine.Fixtures
{
    public class GivenPlayers
    {
        public string Username;

        public void Execute()
        {
            var status = Common.Engine.UserController.Register(Username).GetAwaiter().GetResult();
            if (status != UserRegisterStatus.OK)
            {
                throw new Exception($"User {Username} wasn't registered: {status}");
            }
        }
    }
}