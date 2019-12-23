using System;
using System.Collections.Generic;

namespace RattusEngine
{
    public class Room : Entity
    {
        public string Name;

        public ICollection<User> Players = new List<User>();

        public User Owner;

        public Game Game;
    }
}
