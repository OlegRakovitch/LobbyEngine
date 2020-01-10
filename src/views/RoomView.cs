using System.Collections.Generic;
using RattusEngine.Controllers.Statuses;
using RattusEngine.Models;

namespace RattusEngine.Views
{
    public class RoomView
    {
        public string Name;
        public string GameType;
        public RoomViewStatus Status;
        public IEnumerable<User> Players;
        public User Owner;
    }
}