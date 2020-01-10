using System;
using System.Linq;
using System.Threading.Tasks;
using RattusEngine.Controllers.Statuses;
using RattusEngine.Exceptions;
using RattusEngine.Models;
using RattusEngine.Views;

namespace RattusEngine.Controllers
{
    public class RoomController
    {
        readonly IContext context;
        readonly IGameStarter starter;

        public RoomController(IContext context, IGameStarter starter)
        {
            this.context = context;
            this.starter = starter;
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

        public async Task<RoomCreateStatus> CreateRoom(string roomName, string gameType)
        {
            var user = GetUser();
            var storage = context.Storage;
            if ((await storage.Get<Room>(r => r.Name == roomName)).Any())
            {
                return RoomCreateStatus.DuplicateName;
            }
            else
            {
                if ((await storage.Get<Room>(r => r.Players.Contains(user))).Any())
                {
                    return RoomCreateStatus.AlreadyInRoom;
                }
                else
                {
                    var room = new Room()
                    {
                        Name = roomName,
                        Owner = user,
                        GameType = gameType
                    };
                    room.Players.Add(user);
                    await storage.Save(room);
                    return RoomCreateStatus.OK;
                }
            }
        }

        public async Task<RoomJoinStatus> JoinRoom(string roomName)
        {
            var user = GetUser();
            var storage = context.Storage;
            var room = (await storage.Get<Room>(r => r.Name == roomName)).SingleOrDefault();
            if (room == null)
            {
                return RoomJoinStatus.RoomNotFound;
            }
            else if (IsFullRoom(room))
            {
                return RoomJoinStatus.RoomIsFull;
            }
            else if ((await storage.Get<Room>(r => r.Players.Contains(user))).Any())
            {
                return RoomJoinStatus.AlreadyInRoom;
            }
            else
            {
                room.Players.Add(user);
                await storage.Save(room);
                return RoomJoinStatus.OK;
            }
        }

        public async Task<RoomLeaveStatus> LeaveRoom()
        {
            var user = GetUser();
            var storage = context.Storage;
            var room = (await storage.Get<Room>(r => r.Players.Contains(user))).SingleOrDefault();
            if (room == null)
            {
                return RoomLeaveStatus.NotInRoom;
            }
            else if (!string.IsNullOrEmpty(room.GameId))
            {
                return RoomLeaveStatus.GameInProgress;
            }
            else
            {
                if (room.Owner.Equals(user))
                {
                    room.Players.Clear();
                    await storage.Delete(room);
                }
                else
                {
                    room.Players.Remove(user);
                }
                return RoomLeaveStatus.OK;
            }
        }

        public async Task<GameStartStatus> StartGame()
        {
            var user = GetUser();
            var storage = context.Storage;
            var room = (await storage.Get<Room>(r => r.Players.Contains(user))).SingleOrDefault();
            if (room == null)
            {
                return GameStartStatus.NotInRoom;
            }
            else if (!string.IsNullOrEmpty(room.GameId))
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
                var gameId = await starter.StartGame(room.GameType, room.Players.Select(player => player.Username));
                room.GameId = gameId;
                await storage.Save(room);
                return GameStartStatus.OK;
            }
        }

        public async Task<RoomView[]> GetRooms()
        {
            var user = GetUser();
            var joinedRoom = (await context.Storage.Get<Room>(r => r.Players.Contains(user))).SingleOrDefault();
            if (joinedRoom != null)
            {
                return new RoomView[] { new RoomView {
                    Name = joinedRoom.Name,
                    Status = string.IsNullOrEmpty(joinedRoom.GameId) ? RoomViewStatus.InRoom : RoomViewStatus.InGame,
                    Players = joinedRoom.Players,
                    Owner = joinedRoom.Owner,
                    GameType = joinedRoom.GameType
                }};
            }
            else
            {
                var joinableRooms = await context.Storage.Get<Room>(r => string.IsNullOrEmpty(r.GameId));
                return joinableRooms.Select(r => new RoomView {
                    Name = r.Name,
                    Status = IsFullRoom(r) ? RoomViewStatus.Full : RoomViewStatus.Joinable,
                    Players = r.Players,
                    Owner = r.Owner,
                    GameType = r.GameType
                }).ToArray();
            }
        }
    }
}