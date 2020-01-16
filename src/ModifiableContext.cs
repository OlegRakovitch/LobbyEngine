using LobbyEngine.Models;

namespace LobbyEngine
{
    public class ModifiableContext : IContext
    {
        public User CurrentUser { get; set; }
        public IStorage Storage { get; set; }
        public User GetUser()
        {
            return CurrentUser;
        }
    }
}