using System.Collections.Generic;

namespace RattusEngine.Models
{
    public class Game : Entity
    {
        public IEnumerable<User> Players;
        public ICollection<Move> Moves = new List<Move>();
    }
}