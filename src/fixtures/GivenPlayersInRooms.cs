using System.Linq;

namespace RattusEngine.Fixtures
{
    public class GivenPlayersInRooms
    {
        public string Username;
        public string RoomName;

        public void Execute()
        {
            var storage = Common.App.Context.Storage;
            var room = storage.Get<Room>().Single(r => r.Name == RoomName);
            var user = storage.Get<User>().Single(u => u.Username == Username);
            room.Players.Add(user);
            storage.Save(room);
        }
    }
}