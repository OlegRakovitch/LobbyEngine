using System.Linq;
using RattusEngine.Models;

namespace RattusEngine
{
    public interface IStorage
    {
        void Save<T>(T entity) where T: Entity;
        void DeleteAll<T>() where T: Entity;
        void Delete<T>(T entity) where T: Entity;
        IQueryable<T> Get<T>() where T: Entity;
    }
}