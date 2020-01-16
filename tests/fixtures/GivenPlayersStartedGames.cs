using System;
using System.Linq;
using LobbyEngine.Controllers.Statuses;
using LobbyEngine.Models;

namespace LobbyEngine.Fixtures
{
    public class GivenPlayersStartedGames
    {
        public string Username;

        public void Execute()
        {
            Common.Context.CurrentUser = Common.Engine.Context.Storage.Get<User>(u => u.Username == Username).GetAwaiter().GetResult().Single();
            var status = Common.Engine.RoomController.StartGame().GetAwaiter().GetResult();
            if (status != GameStartStatus.OK)
            {
                throw new Exception($"Game wasn't started successfully by {Username}: {status}");
            }
        }
    }
}