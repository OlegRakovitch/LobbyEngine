using System.Collections.Generic;
using LobbyEngine.Controllers.Statuses;
using LobbyEngine.Models;

namespace LobbyEngine.Views
{
    public class RoomView
    {
        public string Name;
        public string GameType;
        public string GameId;
        public RoomViewStatus Status;
        public IEnumerable<User> Players;
        public User Owner;
    }
}