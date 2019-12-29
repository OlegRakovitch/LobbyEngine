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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));

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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));

            context.CurrentUser = user2;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));

            context.CurrentUser = user3;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));

            context.CurrentUser = user4;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));

            context.CurrentUser = user5;
            Assert.Equal(RoomJoinStatus.RoomIsFull, roomController.JoinRoom("room"));
            Assert.Equal(4, storage.Get<Room>().Single().Players.Count);
        }

        [Fact]
        public void UserBecomesPlayerAndOwnerOfCreatedRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));
            Assert.True(context.Storage.Get<Room>().Single().Players.Contains(context.GetUser()));
            var room = context.Storage.Get<Room>().Single(r => r.Name == "room");
            var user = context.GetUser();
            Assert.Equal(new User[] { user }, room.Players);
            Assert.Equal(user, room.Owner);
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));

            context.CurrentUser = user2;
            Assert.Equal(RoomCreateStatus.DuplicateName, roomController.CreateRoom("room"));
            Assert.Equal(1, context.Storage.Get<Room>().Count());
            var room = storage.Get<Room>().Single();
            Assert.Equal(new User[] { user1 }, room.Players);
        }

        [Fact]
        public void UserCanNotCreateRoomWhenUserIsInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));
            Assert.Equal(RoomCreateStatus.AlreadyInRoom, roomController.CreateRoom("room2"));
            Assert.Equal(1, context.Storage.Get<Room>().Count());
            var room = context.Storage.Get<Room>().Single();
            Assert.Equal("room", room.Name);
            Assert.Equal(new User[] { context.GetUser() }, room.Players);
        }

        [Fact]
        public void UserCanNotJoinSameRoomIfUserIsAlreadyInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));
            Assert.Equal(RoomJoinStatus.AlreadyInRoom, roomController.JoinRoom("room"));
            var room = context.Storage.Get<Room>().Single();
            Assert.Equal(new User[] { context.GetUser() }, room.Players);
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));

            context.CurrentUser = user;
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room2"));
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));
            var room = context.Storage.Get<Room>().Single(r => r.Name == "room");
            Assert.Equal(new User[] { context.GetUser() }, room.Players);

            context.CurrentUser = user;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));
            Assert.Equal(RoomLeaveStatus.OK, roomController.LeaveRoom());
            room = context.Storage.Get<Room>().Single(r => r.Name == "room");
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));

            context.CurrentUser = user;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));

            context.CurrentUser = owner;
            Assert.Equal(GameStartStatus.OK, roomController.StartGame());
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));

            context.CurrentUser = user;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));

            context.CurrentUser = owner;
            Assert.Equal(GameStartStatus.OK, roomController.StartGame());
            var room = storage.Get<Room>().Single();
            Assert.Equal(new User[] { owner, user }, room.Players);
            Assert.NotNull(room.Game);
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));

            context.CurrentUser = user;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));
            Assert.Equal(GameStartStatus.NotAnOwner, roomController.StartGame());
            var room = storage.Get<Room>().Single();
            Assert.Equal(new User[] { owner, user }, room.Players);
            Assert.Null(room.Game);
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));
            Assert.Equal(GameStartStatus.NotEnoughPlayers, roomController.StartGame());
            var room = context.Storage.Get<Room>().Single();
            Assert.Null(room.Game);
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));

            context.CurrentUser = user;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));

            context.CurrentUser = owner;
            Assert.Equal(GameStartStatus.OK, roomController.StartGame());
            var room = context.Storage.Get<Room>().Single();
            var game = room.Game;
            Assert.Equal(GameStartStatus.GameInProgress, roomController.StartGame());
            Assert.Equal(game, room.Game);
        }

        [Fact]
        public void RoomGetsDeletedIfOwnerLeavesRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = new RoomController(context);
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));
            Assert.Single(context.Storage.Get<Room>());
            Assert.Equal(RoomLeaveStatus.OK, roomController.LeaveRoom());
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));

            context.CurrentUser = user;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));

            context.CurrentUser = owner;
            Assert.Equal(GameStartStatus.OK, roomController.StartGame());
            var ownerRooms = roomController.GetRooms();
            Assert.Single(ownerRooms);
            var ownerRoom = ownerRooms.Single();
            Assert.Equal(RoomViewStatus.InGame, ownerRoom.Status);
            Assert.Equal(new User[] { owner, user }, ownerRoom.Players);
            Assert.Equal(owner, ownerRoom.Owner);

            context.CurrentUser = user;
            var userRooms = roomController.GetRooms();
            Assert.Single(userRooms);
            var userRoom = userRooms.Single();
            Assert.Equal(RoomViewStatus.InGame, userRoom.Status);
            Assert.Equal(new User[] { owner, user }, userRoom.Players);
            Assert.Equal(owner, userRoom.Owner);
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));
            var ownerRooms = roomController.GetRooms();
            Assert.Single(ownerRooms);
            var ownerRoom = ownerRooms.Single();
            Assert.Equal(RoomViewStatus.InRoom, ownerRoom.Status);
            Assert.Equal(new User[] { owner }, ownerRoom.Players);
            Assert.Equal(owner, ownerRoom.Owner);

            context.CurrentUser = user;
            Assert.Equal(RoomJoinStatus.OK, roomController.JoinRoom("room"));
            var userRooms = roomController.GetRooms();
            Assert.Single(userRooms);
            var userRoom = userRooms.Single();
            Assert.Equal(RoomViewStatus.InRoom, userRoom.Status);
            Assert.Equal(new User[] { owner, user }, userRoom.Players);
            Assert.Equal(owner, userRoom.Owner);
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
            Assert.Equal(RoomCreateStatus.OK, roomController.CreateRoom("room"));

            context.CurrentUser = user;
            var userRooms = roomController.GetRooms();
            Assert.Single(userRooms);
            var userRoom = userRooms.Single();
            Assert.Equal(RoomViewStatus.Joinable, userRoom.Status);
            Assert.Equal(new User[] { owner }, userRoom.Players);
            Assert.Equal(owner, userRoom.Owner);
        }
    }
}