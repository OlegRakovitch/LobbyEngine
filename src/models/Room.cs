using System;
using System.Collections.Generic;

namespace LobbyEngine.Models
{
    public class Room : Entity
    {
        public string Name;

        public ICollection<User> Players = new List<User>();

        public User Owner;

        public string GameId;

        public string GameType;
    }
}
