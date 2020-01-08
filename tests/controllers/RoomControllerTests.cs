using Xunit;
using RattusEngine;
using System.Linq;
using RattusEngine.Controllers.Statuses;
using RattusEngine.Controllers;
using RattusEngine.Exceptions;
using RattusEngine.Models;

namespace RattusEngine.Tests
{
    public class RoomControllerTests
    {
        IContext GetContextWithoutProvidedUser()
        {
            return new ModifiableContext()
            {
                Storage = new MemoryStorage()
            };
        }

        IContext GetContextWithProvidedUser()
        {
            return new ModifiableContext()
            {
                Storage = new MemoryStorage(),
                CurrentUser = new User()
            };
        }

        [Fact]
        public void RoomCanNotBeCreatedAnonymously()
        {
            var roomController = new RoomController(GetContextWithoutProvidedUser());
            Assert.Throws<UserNotSpecifiedException>(() => roomController.CreateRoom("room"));
        }

        [Fact]
        public void RoomCanNotBeJoinedAnonymously()
        {
            var roomController = new RoomController(GetContextWithoutProvidedUser()); 
            Assert.Throws<UserNotSpecifiedException>(() => roomController.JoinRoom("room"));
        }

        [Fact]
        public void GameCanNotBeStartedAnonymously()
        {
            var roomController = new RoomController(GetContextWithoutProvidedUser());
            Assert.Throws<UserNotSpecifiedException>(() => roomController.StartGame());
        }

        [Fact]
        public void RoomsListCanNotBeRetrievedAnonymously()
        {
            var roomController = new RoomController(GetContextWithoutProvidedUser());
            Assert.Throws<UserNotSpecifiedException>(() => roomController.GetRooms());
        }

        [Fact]
        public void UserCanJoinRoomIfUserIsNotInRoom()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));
            var room = storage.Get<Room>().Single(r => r.Name == "room");
            Assert.Equal(new User[] { owner, user }, room.Players);
        }

        [Fact]
        public void UserCanNotJoinNonExistingRoom()
        {
            var roomController = new RoomController(GetContextWithProvidedUser());
            Assert.Equal(RoomJoinStatus.RoomNotFound, roomController.JoinRoom("room"));
        }

        [Fact]
        public void UserCanNotJoinFullRoom()
        {
            var user1 = new User();
            var user2 = new User();
            var user3 = new User();
            var user4 = new User();
            var user5 = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = user1
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user2;
            roomController.JoinRoom("room");

            context.CurrentUser = user3;
            roomController.JoinRoom("room");

            context.CurrentUser = user4;
            roomController.JoinRoom("room");

            context.CurrentUser = user5;
            Assert.Equal(RoomJoinStatus.RoomIsFull, roomController.JoinRoom("room"));
            Assert.Equal(4, storage.Get<Room>().Single().Players.Count);
        }

        [Fact]
        public void UserCanCreateRoomIfUserIsNotInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));
            var room = context.Storage.Get<Room>().Single();
            Assert.Equal("room", room.Name);
        }

        [Fact]
        public void UserBecomesPlayerAndOwnerOfCreatedRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");
            var room = context.Storage.Get<Room>().Single();
            var user = context.GetUser();
            Assert.Equal(new User[] { user }, room.Players);
            Assert.Equal(user, room.Owner);
        }

        [Fact]
        public void CreatedRoomHasSpecifiedName()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            roomController.CreateRoom("room-with-some-name");
            var room = context.Storage.Get<Room>().Single();
            Assert.Equal("room-with-some-name", room.Name);
        }

        [Fact]
        public void UsersCanNotCreateRoomsWithTheSameName()
        {
            var user1 = new User();
            var user2 = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = user1
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user2;
            Assert.Equal(RoomCreateStatus.DuplicateName, roomController.CreateRoom("room"));
            Assert.Single(context.Storage.Get<Room>());
        }

        [Fact]
        public void UserCanNotCreateRoomWhenUserIsInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");
            Assert.Equal(RoomCreateStatus.AlreadyInRoom, roomController.CreateRoom("room2"));
            var rooms = context.Storage.Get<Room>();
            Assert.Single(rooms);
            var room = rooms.Single();
            Assert.Equal("room", room.Name);
        }

        [Fact]
        public void UserCanNotJoinSameRoomIfUserIsAlreadyInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");
            Assert.Equal(RoomJoinStatus.AlreadyInRoom, roomController.JoinRoom("room"));
        }

        [Fact]
        public void UserCanNotJoinDifferentRoomIfUserIsAlreadyInRoom()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            roomController.CreateRoom("room2");
            Assert.Equal(RoomJoinStatus.AlreadyInRoom, roomController.JoinRoom("room"));
            var room = context.Storage.Get<Room>().Single(r => r.Name == "room");
            Assert.Equal(new User[] { owner }, room.Players);
            var room2 = context.Storage.Get<Room>().Single(r => r.Name == "room2");
            Assert.Equal(new User[] { user }, room2.Players);
        }

        [Fact]
        public void UserCanLeaveRoomIfUserIsInRoom()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            roomController.JoinRoom("room");
            Assert.Equal(RoomLeaveStatus.OK, roomController.LeaveRoom());
            var room = context.Storage.Get<Room>().Single();
            Assert.Equal(new User[] { owner }, room.Players);
        }

        [Fact]
        public void UserCanNotLeaveRoomIfUserIsNotInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            Assert.Equal(RoomLeaveStatus.NotInRoom, roomController.LeaveRoom());
        }

        [Fact]
        public void UserCanNotLeaveRoomIfGameAlreadyStarted()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            roomController.JoinRoom("room");

            context.CurrentUser = owner;
            roomController.StartGame();
            Assert.Equal(RoomLeaveStatus.GameInProgress, roomController.LeaveRoom());
            var room = storage.Get<Room>().Single();
            Assert.Equal(new User[] { owner, user }, room.Players);
        }

        [Fact]
        public void OwnerCanStartGameIfRoomHasTwoOrMorePlayers()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            roomController.JoinRoom("room");

            context.CurrentUser = owner;
            Assert.Equal(GameStartStatus.OK, roomController.StartGame());
        }

        [Fact]
        public void GameIdIsSavedInRoomUponGameStart()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            roomController.JoinRoom("room");

            context.CurrentUser = owner;
            roomController.StartGame();

            var room = storage.Get<Room>().Single();
            var game = storage.Get<Game>().Single();
            Assert.Equal(game.Id, room.GameId);
        }

        [Fact]
        public void GamePlayersAreAssignedUponGameStart()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            roomController.JoinRoom("room");

            context.CurrentUser = owner;
            roomController.StartGame();

            var game = storage.Get<Game>().Single();
            Assert.Equal(new User[] { owner, user }, game.Players);
        }

        [Fact]
        public void UserCanNotStartGameIfUserIsNotOwner()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            roomController.JoinRoom("room");
            Assert.Equal(GameStartStatus.NotAnOwner, roomController.StartGame());
            Assert.Null(storage.Get<Room>().Single().GameId);
        }

        [Fact]
        public void UserCanNotStartGameIfUserIsNotInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            Assert.Equal(GameStartStatus.NotInRoom, roomController.StartGame());
        }

        [Fact]
        public void OwnerCanNotStartGameWithoutOtherPlayers()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");
            Assert.Equal(GameStartStatus.NotEnoughPlayers, roomController.StartGame());
            Assert.Null(context.Storage.Get<Room>().Single().GameId);
        }

        [Fact]
        public void OwnerCanNotStartAlreadyStartedGame()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            roomController.JoinRoom("room");

            context.CurrentUser = owner;
            roomController.StartGame();
            var room = context.Storage.Get<Room>().Single();
            var gameId = room.GameId;
            Assert.Equal(GameStartStatus.GameInProgress, roomController.StartGame());
            room = context.Storage.Get<Room>().Single();
            Assert.Equal(gameId, room.GameId);
        }

        [Fact]
        public void RoomGetsDeletedIfOwnerLeavesRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");
            roomController.LeaveRoom();
            Assert.Empty(context.Storage.Get<Room>());
        }

        [Fact]
        public void UserGetsEmptyListOfRoomsIfThereAreNoRooms()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            Assert.Empty(roomController.GetRooms());
        }

        [Fact]
        public void RoomHasInGameStatusIfOwnerStartedGame()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            roomController.JoinRoom("room");

            context.CurrentUser = owner;
            roomController.StartGame();
            var ownerRoom = roomController.GetRooms().Single();
            Assert.Equal(RoomViewStatus.InGame, ownerRoom.Status);

            context.CurrentUser = user;
            var userRoom = roomController.GetRooms().Single();
            Assert.Equal(RoomViewStatus.InGame, userRoom.Status);
        }

        [Fact]
        public void RoomHasInRoomStatusIfOwnerCreatedRoom()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");
            var ownerRoom = roomController.GetRooms().Single();
            Assert.Equal(RoomViewStatus.InRoom, ownerRoom.Status);

            context.CurrentUser = user;
            roomController.JoinRoom("room");
            var userRoom = roomController.GetRooms().Single();
            Assert.Equal(RoomViewStatus.InRoom, userRoom.Status);
        }

        [Fact]
        public void RoomHasJoinableStatusIfUserIsNotInRoom()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = new RoomController(context);
            roomController.CreateRoom("room");

            context.CurrentUser = user;
            var userRoom = roomController.GetRooms().Single();
            Assert.Equal(RoomViewStatus.Joinable, userRoom.Status);
        }
    }
}