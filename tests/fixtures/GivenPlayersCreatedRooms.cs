using System;
using System.Linq;
using LobbyEngine.Controllers.Statuses;
using LobbyEngine.Models;

namespace LobbyEngine.Fixtures
{
    public class GivenPlayersCreatedRooms
    {
        public string Username;
        public string RoomName;
        public string GameType;

        public void Execute()
        {
            Common.Context.CurrentUser = Common.Engine.Context.Storage.Get<User>(u => u.Username == Username).GetAwaiter().GetResult().Single();
            var status = Common.Engine.RoomController.CreateRoom(RoomName, GameType).GetAwaiter().GetResult();
            if (RoomCreateStatus.OK != status)
            {
                throw new Exception($"Room {RoomName} wasn't created successfully by {Username}: {status}");
            }
        }
    }
}