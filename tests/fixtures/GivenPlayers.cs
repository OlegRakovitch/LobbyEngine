using System;
using LobbyEngine.Controllers.Statuses;

namespace LobbyEngine.Fixtures
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