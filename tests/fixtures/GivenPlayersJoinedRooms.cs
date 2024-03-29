using System;
using System.Linq;
using LobbyEngine.Controllers.Statuses;
using LobbyEngine.Models;

namespace LobbyEngine.Fixtures
{
    public class GivenPlayersJoinedRooms
    {
        public string Username;
        public string RoomName;

        public void Execute()
        {
            Common.Context.CurrentUser = Common.Engine.Context.Storage.Get<User>(u => u.Username == Username).GetAwaiter().GetResult().Single();
            var status = Common.Engine.RoomController.JoinRoom(RoomName).GetAwaiter().GetResult();
            if (status != RoomJoinStatus.OK)
            {
                throw new Exception($"Room {RoomName} wasn't joined successfully by {Username}: {status}");
            }
        }
    }
}