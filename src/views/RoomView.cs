using System.Collections.Generic;
using RattusEngine.Controllers.Statuses;
using RattusEngine.Models;

namespace RattusEngine.Views
{
    public class RoomView
    {
        public string Name;
        public RoomViewStatus Status;
        public IEnumerable<User> Players;
    }
}