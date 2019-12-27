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
            Common.Context.CurrentUser = Common.App.Context.Storage.Get<User>().Single(u => u.Username == Username);
            var status = Common.App.RoomController.StartGame();
            if (status != GameStartStatus.OK)
            {
                throw new Exception($"Game wasn't started successfully by {Username}: {status}");
            }
        }
    }
}