using LobbyEngine.Models;

namespace LobbyEngine
{
    public interface IContext
    {
        IStorage Storage { get; }
        User GetUser();
    }
}