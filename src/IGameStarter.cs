using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobbyEngine
{
    public interface IGameStarter
    {
        Task<string> StartGame(string gameType, IEnumerable<string> players);
    }
}