using System.Linq;

namespace RattusEngine
{
    public interface IStorage
    {
        bool Save<T>(T entity) where T: Entity;
        bool DeleteAll<T>() where T: Entity;
        IQueryable<T> Get<T>() where T: Entity;
    }
}