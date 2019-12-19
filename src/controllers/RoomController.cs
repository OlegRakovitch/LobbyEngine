using System.Collections.Generic;
using System.Linq;

namespace RattusEngine
{
    public class RoomController
    {
        private IContext context;

        public RoomController(IContext context)
        {
            this.context = context;
        }

        public bool CreateRoom(string roomName)
        {
            var storage = context.Storage;
            return storage.Save(new Room() { Name = roomName });
        }

        public bool JoinRoom(string roomName)
        {
            var storage = context.Storage;
            var room = storage.Get<Room>().Single(r => r.Name == roomName);
            room.Players.Add(context.GetUser());
            return storage.Save(room);
        }

        public bool StartGame(string roomName)
        {
            var storage = context.Storage;
            var room = storage.Get<Room>().Single(r => r.Name == roomName);
            var game = new Game();
            room.Game = game;
            return storage.Save(game) && storage.Save(room);
        }

        public RoomView[] GetRooms()
        {
            var joinedRoom = context.Storage.Get<Room>().SingleOrDefault(r => r.Players.Contains(context.GetUser()));
            bool insideRoom = joinedRoom != null;
            IEnumerable<Room> joinableRooms;
            if (insideRoom)
            {
                joinableRooms = new Room[] { joinedRoom };
            }
            else
            {
                joinableRooms = context.Storage.Get<Room>().Where(r => r.Game == null);
            }
            return joinableRooms.Select(r => new RoomView() {
                Name = r.Name,
                Action = !insideRoom ? "Join" : joinedRoom.Game == null ? "Leave" : "Resume"
            }).ToArray();
        }
    }
}