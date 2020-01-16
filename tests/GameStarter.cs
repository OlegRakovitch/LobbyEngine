using System.Collections.Generic;
using System.Threading.Tasks;

namespace LobbyEngine.Tests
{
    public class GameStarter : IGameStarter
    {
        public Task<string> StartGame(string gameType, IEnumerable<string> players)
        {
            return Task.FromResult($"GameId");
        }
    }
}