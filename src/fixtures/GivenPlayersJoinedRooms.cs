using System;
using System.Linq;
using RattusEngine.Controllers.Statuses;
using RattusEngine.Models;

namespace RattusEngine.Fixtures
{
    public class GivenPlayersJoinedRooms
    {
        public string Username;
        public string RoomName;

        public void Execute()
        {
            Common.Context.CurrentUser = Common.App.Context.Storage.Get<User>().Single(u => u.Username == Username);
            var status = Common.App.RoomController.JoinRoom(RoomName);
            if (status != RoomJoinStatus.OK)
            {
                throw new Exception($"Room {RoomName} wasn't joined successfully by {Username}: {status}");
            }
        }
    }
}