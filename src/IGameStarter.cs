using System.Collections.Generic;
using System.Threading.Tasks;

namespace RattusEngine
{
    public interface IGameStarter
    {
        Task<string> StartGame(string gameType, IEnumerable<string> players);
    }
}