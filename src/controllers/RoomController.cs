using System.Linq;
using RattusEngine.Controllers.Statuses;
using RattusEngine.Exceptions;
using RattusEngine.Models;
using RattusEngine.Views;

namespace RattusEngine.Controllers
{
    public class RoomController
    {
        private IContext context;

        public RoomController(IContext context)
        {
            this.context = context;
        }

        User GetUser()
        {
            var user = context.GetUser();
            if (user == null)
            {
                throw new UserNotSpecifiedException();
            }
            return user;
        }

        bool IsFullRoom(Room room)
        {
            return room.Players.Count == 4;
        }

        public RoomCreateStatus CreateRoom(string roomName)
        {
            var user = GetUser();
            var storage = context.Storage;
            if (storage.Get<Room>().Any(r => r.Name == roomName))
            {
                return RoomCreateStatus.DuplicateName;
            }
            else if (storage.Get<Room>().Any(r => r.Players.Contains(user)))
            {
                return RoomCreateStatus.AlreadyInRoom;
            }
            else
            {
                var room = new Room() { Name = roomName };
                room.Players.Add(user);
                room.Owner = user;
                storage.Save(room);
                return RoomCreateStatus.OK;
            }
        }

        public RoomJoinStatus JoinRoom(string roomName)
        {
            var user = GetUser();
            var storage = context.Storage;
            var room = storage.Get<Room>().SingleOrDefault(r => r.Name == roomName);
            if (room == null)
            {
                return RoomJoinStatus.RoomNotFound;
            }
            else if (IsFullRoom(room))
            {
                return RoomJoinStatus.RoomIsFull;
            }
            else if (storage.Get<Room>().Any(r => r.Players.Contains(user)))
            {
                return RoomJoinStatus.AlreadyInRoom;
            }
            else
            {
                room.Players.Add(user);
                storage.Save(room);
                return RoomJoinStatus.OK;
            }
        }

        public RoomLeaveStatus LeaveRoom()
        {
            var user = GetUser();
            var storage = context.Storage;
            var room = storage.Get<Room>().SingleOrDefault(r => r.Players.Contains(user));
            if (room == null)
            {
                return RoomLeaveStatus.NotInRoom;
            }
            else if (room.Game != null)
            {
                return RoomLeaveStatus.GameInProgress;
            }
            else
            {
                if (room.Owner.Equals(user))
                {
                    room.Players.Clear();
                    storage.Delete(room);
                }
                else
                {
                    room.Players.Remove(user);
                }
                return RoomLeaveStatus.OK;
            }
        }

        public GameStartStatus StartGame()
        {
            var user = GetUser();
            var storage = context.Storage;
            var room = storage.Get<Room>().SingleOrDefault(r => r.Players.Contains(user));
            if (room == null)
            {
                return GameStartStatus.NotInRoom;
            }
            else if (room.Game != null)
            {
                return GameStartStatus.GameInProgress;
            }
            else if (!user.Equals(room.Owner))
            {
                return GameStartStatus.NotAnOwner;
            }
            else if (room.Players.Count < 2)
            {
                return GameStartStatus.NotEnoughPlayers;
            }
            else
            {
                var game = new Game();
                storage.Save(game);
                room.Game = game;
                storage.Save(room);
                return GameStartStatus.OK;
            }
        }

        public RoomView[] GetRooms()
        {
            var user = GetUser();
            var joinedRoom = context.Storage.Get<Room>().SingleOrDefault(r => r.Players.Contains(user));
            if (joinedRoom != null)
            {
                return new RoomView[] { new RoomView {
                    Name = joinedRoom.Name,
                    Status = joinedRoom.Game == null ? RoomViewStatus.InRoom : RoomViewStatus.InGame
                }};
            }
            else
            {
                var joinableRooms = context.Storage.Get<Room>().Where(r => r.Game == null);
                return joinableRooms.Select(r => new RoomView {
                    Name = r.Name,
                    Status = IsFullRoom(r) ? RoomViewStatus.Full : RoomViewStatus.Joinable
                }).ToArray();
            }
        }
    }
}