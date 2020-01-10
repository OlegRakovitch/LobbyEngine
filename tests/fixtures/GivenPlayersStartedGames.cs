using System;
using System.Linq;
using RattusEngine.Controllers.Statuses;
using RattusEngine.Models;

namespace RattusEngine.Fixtures
{
    public class GivenPlayersStartedGames
    {
        public string Username;

        public void Execute()
        {
            Common.Context.CurrentUser = Common.App.Context.Storage.Get<User>(u => u.Username == Username).GetAwaiter().GetResult().Single();
            var status = Common.App.RoomController.StartGame().GetAwaiter().GetResult();
            if (status != GameStartStatus.OK)
            {
                throw new Exception($"Game wasn't started successfully by {Username}: {status}");
            }
        }
    }
}