using System;
using System.Linq;
using RattusEngine.Controllers.Statuses;
using RattusEngine.Models;

namespace RattusEngine.Fixtures
{
    public class GivenPlayersCreatedRooms
    {
        public string Username;
        public string RoomName;

        public void Execute()
        {
            Common.Context.CurrentUser = Common.App.Context.Storage.Get<User>().Single(u => u.Username == Username);
            var status = Common.App.RoomController.CreateRoom(RoomName);
            if (RoomCreateStatus.OK != status)
            {
                throw new Exception($"Room {RoomName} wasn't created successfully by {Username}: {status}");
            }
        }
    }
}