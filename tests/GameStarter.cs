using System.Collections.Generic;
using System.Threading.Tasks;

namespace RattusEngine.Tests
{
    public class GameStarter : IGameStarter
    {
        public Task<string> StartGame(string gameType, IEnumerable<string> players)
        {
            return Task.FromResult($"{gameType}:[{string.Join(',', players)}]");
        }
    }
}