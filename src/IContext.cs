using RattusEngine.Models;

namespace RattusEngine
{
    public interface IContext
    {
        IStorage Storage { get; }
        User GetUser();
    }
}