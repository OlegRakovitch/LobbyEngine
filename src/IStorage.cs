using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LobbyEngine.Models;

namespace LobbyEngine
{
    public interface IStorage
    {
        Task Save<T>(T entity) where T: Entity;
        Task DeleteAll<T>() where T: Entity;
        Task Delete<T>(T entity) where T: Entity;
        Task<IEnumerable<T>> Get<T>(Expression<Func<T, bool>> filter = null) where T: Entity;
    }
}