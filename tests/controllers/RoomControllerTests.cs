using Xunit;
using LobbyEngine;
using System.Linq;
using LobbyEngine.Controllers.Statuses;
using LobbyEngine.Controllers;
using LobbyEngine.Exceptions;
using LobbyEngine.Models;
using System;

namespace LobbyEngine.Tests
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

        RoomController GetRoomController(IContext context)
        {
            return new RoomController(context, new GameStarter());
        }

        [Fact]
        public async void RoomCanNotBeCreatedAnonymously()
        {
            var roomController = GetRoomController(GetContextWithoutProvidedUser());
            await Assert.ThrowsAsync<UserNotSpecifiedException>(() => roomController.CreateRoom("room", "game"));
        }

        [Fact]
        public async void RoomCanNotBeJoinedAnonymously()
        {
            var roomController = GetRoomController(GetContextWithoutProvidedUser()); 
            await Assert.ThrowsAsync<UserNotSpecifiedException>(() => roomController.JoinRoom("room"));
        }

        [Fact]
        public async void GameCanNotBeStartedAnonymously()
        {
            var roomController = GetRoomController(GetContextWithoutProvidedUser());
            await Assert.ThrowsAsync<UserNotSpecifiedException>(() => roomController.StartGame());
        }

        [Fact]
        public async void RoomsListCanNotBeRetrievedAnonymously()
        {
            var roomController = GetRoomController(GetContextWithoutProvidedUser());
            await Assert.ThrowsAsync<UserNotSpecifiedException>(() => roomController.GetRooms());
        }

        [Fact]
        public async void UserCanJoinRoomIfUserIsNotInRoom()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            Assert.Equal(RoomJoinStatus.OK, await roomController.JoinRoom("room"));
            var room = (await storage.Get<Room>(r => r.Name == "room")).Single();
            Assert.Equal(new User[] { owner, user }, room.Players);
        }

        [Fact]
        public async void UserCanNotJoinNonExistingRoom()
        {
            var roomController = GetRoomController(GetContextWithProvidedUser());
            Assert.Equal(RoomJoinStatus.RoomNotFound, await roomController.JoinRoom("room"));
        }

        [Fact]
        public async void UserCanNotJoinFullRoom()
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
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user2;
            await roomController.JoinRoom("room");

            context.CurrentUser = user3;
            await roomController.JoinRoom("room");

            context.CurrentUser = user4;
            await roomController.JoinRoom("room");

            context.CurrentUser = user5;
            Assert.Equal(RoomJoinStatus.RoomIsFull, await roomController.JoinRoom("room"));
            Assert.Equal(4, (await storage.Get<Room>()).Single().Players.Count);
        }

        [Fact]
        public async void UserCanCreateRoomIfUserIsNotInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            Assert.Equal(RoomCreateStatus.OK, await roomController.CreateRoom("room", "game"));
            var room = (await context.Storage.Get<Room>()).Single();
            Assert.Equal("room", room.Name);
            Assert.Equal("game", room.GameType);
        }

        [Fact]
        public async void UserBecomesPlayerAndOwnerOfCreatedRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            await  roomController.CreateRoom("room", "game");
            var room = (await context.Storage.Get<Room>()).Single();
            var user = context.GetUser();
            Assert.Equal(new User[] { user }, room.Players);
            Assert.Equal(user, room.Owner);
        }

        [Fact]
        public async void CreatedRoomHasSpecifiedName()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room-with-some-name", "game");
            var room = (await context.Storage.Get<Room>()).Single();
            Assert.Equal("room-with-some-name", room.Name);
        }

        [Fact]
        public async void GameIdIsSavedInRoom()
        {
            var owner = new User() { Username = "owner" };
            var user = new User() { Username = "user" };
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");
            
            context.CurrentUser = user;
            await roomController.JoinRoom("room");

            context.CurrentUser = owner;
            await roomController.StartGame();

            var room = (await context.Storage.Get<Room>()).Single();
            Assert.Equal("GameId", room.GameId);
        }

        [Fact]
        public async void UsersCanNotCreateRoomsWithTheSameName()
        {
            var user1 = new User();
            var user2 = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = user1
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user2;
            Assert.Equal(RoomCreateStatus.DuplicateName, await roomController.CreateRoom("room", "game"));
            Assert.Single(await context.Storage.Get<Room>());
        }

        [Fact]
        public async void UserCanNotCreateRoomWhenUserIsInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");
            Assert.Equal(RoomCreateStatus.AlreadyInRoom, await roomController.CreateRoom("room2", "game"));
            var rooms = await context.Storage.Get<Room>();
            Assert.Single(rooms);
            var room = rooms.Single();
            Assert.Equal("room", room.Name);
        }

        [Fact]
        public async void UserCanNotJoinSameRoomIfUserIsAlreadyInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");
            Assert.Equal(RoomJoinStatus.AlreadyInRoom, await roomController.JoinRoom("room"));
        }

        [Fact]
        public async void UserCanNotJoinDifferentRoomIfUserIsAlreadyInRoom()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            await roomController.CreateRoom("room2", "game");
            Assert.Equal(RoomJoinStatus.AlreadyInRoom, await roomController.JoinRoom("room"));
            var room = (await context.Storage.Get<Room>(r => r.Name == "room")).Single();
            Assert.Equal(new User[] { owner }, room.Players);
            var room2 = (await context.Storage.Get<Room>(r => r.Name == "room2")).Single();
            Assert.Equal(new User[] { user }, room2.Players);
        }

        [Fact]
        public async void UserCanLeaveRoomIfUserIsInRoom()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            await roomController.JoinRoom("room");
            Assert.Equal(RoomLeaveStatus.OK, await roomController.LeaveRoom());
            var room = (await context.Storage.Get<Room>()).Single();
            Assert.Equal(new User[] { owner }, room.Players);
        }

        [Fact]
        public async void UserCanNotLeaveRoomIfUserIsNotInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            Assert.Equal(RoomLeaveStatus.NotInRoom, await roomController.LeaveRoom());
        }

        [Fact]
        public async void UserCanNotLeaveRoomIfGameAlreadyStarted()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            await roomController.JoinRoom("room");

            context.CurrentUser = owner;
            await roomController.StartGame();
            Assert.Equal(RoomLeaveStatus.GameInProgress, await roomController.LeaveRoom());
            var room = (await storage.Get<Room>()).Single();
            Assert.Equal(new User[] { owner, user }, room.Players);
        }

        [Fact]
        public async void OwnerCanStartGameIfRoomHasTwoOrMorePlayers()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            await roomController.JoinRoom("room");

            context.CurrentUser = owner;
            Assert.Equal(GameStartStatus.OK, await roomController.StartGame());
        }

        [Fact]
        public async void UserCanNotStartGameIfUserIsNotOwner()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            await roomController.JoinRoom("room");
            Assert.Equal(GameStartStatus.NotAnOwner, await roomController.StartGame());
            Assert.Null((await storage.Get<Room>()).Single().GameId);
        }

        [Fact]
        public async void UserCanNotStartGameIfUserIsNotInRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            Assert.Equal(GameStartStatus.NotInRoom, await roomController.StartGame());
        }

        [Fact]
        public async void OwnerCanNotStartGameWithoutOtherPlayers()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");
            Assert.Equal(GameStartStatus.NotEnoughPlayers, await roomController.StartGame());
            Assert.Null((await context.Storage.Get<Room>()).Single().GameId);
        }

        [Fact]
        public async void OwnerCanNotStartAlreadyStartedGame()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            await roomController.JoinRoom("room");

            context.CurrentUser = owner;
            await roomController.StartGame();
            var room = (await context.Storage.Get<Room>()).Single();
            var gameId = room.GameId;
            Assert.Equal(GameStartStatus.GameInProgress, await roomController.StartGame());
            room = (await context.Storage.Get<Room>()).Single();
            Assert.Equal(gameId, room.GameId);
        }

        [Fact]
        public async void RoomGetsDeletedIfOwnerLeavesRoom()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");
            await roomController.LeaveRoom();
            Assert.Empty(await context.Storage.Get<Room>());
        }

        [Fact]
        public async void UserGetsEmptyListOfRoomsIfThereAreNoRooms()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            Assert.Empty(await roomController.GetRooms());
        }

        [Fact]
        public async void RoomHasInGameStatusIfOwnerStartedGame()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            await roomController.JoinRoom("room");

            context.CurrentUser = owner;
            await roomController.StartGame();
            var ownerRoom = (await roomController.GetRooms()).Single();
            Assert.Equal(RoomViewStatus.InGame, ownerRoom.Status);

            context.CurrentUser = user;
            var userRoom = (await roomController.GetRooms()).Single();
            Assert.Equal(RoomViewStatus.InGame, userRoom.Status);
        }

        [Fact]
        public async void RoomHasInRoomStatusIfOwnerCreatedRoom()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");
            var ownerRoom = (await roomController.GetRooms()).Single();
            Assert.Equal(RoomViewStatus.InRoom, ownerRoom.Status);

            context.CurrentUser = user;
            await roomController.JoinRoom("room");
            var userRoom = (await roomController.GetRooms()).Single();
            Assert.Equal(RoomViewStatus.InRoom, userRoom.Status);
        }

        [Fact]
        public async void RoomHasJoinableStatusIfUserIsNotInRoom()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            var userRoom = (await roomController.GetRooms()).Single();
            Assert.Equal(RoomViewStatus.Joinable, userRoom.Status);
        }

        [Fact]
        public async void UserCanSeeListOfPlayers()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            var room = (await roomController.GetRooms()).Single();
            Assert.Equal(new User[] { owner }, room.Players);
        }

        [Fact]
        public async void UserCanSeeOwner()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            var room = (await roomController.GetRooms()).Single();
            Assert.Equal(owner, room.Owner);
        }

        [Fact]
        public async void UserCanSeeRoomName()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            Assert.Equal("room", (await roomController.GetRooms()).Single().Name);
        }

        [Fact]
        public async void UserCanSeeGameType()
        {
            var owner = new User();
            var user = new User();
            var storage = new MemoryStorage();
            var context = new ModifiableContext()
            {
                Storage = storage,
                CurrentUser = owner
            };
            var roomController = GetRoomController(context);
            await roomController.CreateRoom("room", "game");

            context.CurrentUser = user;
            Assert.Equal("game", (await roomController.GetRooms()).Single().GameType);
        }

        [Fact]
        public async void UserCanNotCreateRoomWithoutName()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            await Assert.ThrowsAsync<ArgumentException>(() => roomController.CreateRoom("", "game"));
            await Assert.ThrowsAsync<ArgumentException>(() => roomController.CreateRoom(null, "game"));
        }

        [Fact]
        public async void UserCanNotCreateRoomWithoutGameType()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            await Assert.ThrowsAsync<ArgumentException>(() => roomController.CreateRoom("room", ""));
            await Assert.ThrowsAsync<ArgumentException>(() => roomController.CreateRoom("room", null));
        }

        [Fact]
        public async void UserCanNotJoinRoomWithoutName()
        {
            var context = GetContextWithProvidedUser();
            var roomController = GetRoomController(context);
            await Assert.ThrowsAsync<ArgumentException>(() => roomController.JoinRoom(""));
            await Assert.ThrowsAsync<ArgumentException>(() => roomController.JoinRoom(null));
        }
    }
}