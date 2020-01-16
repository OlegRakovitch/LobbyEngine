using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using LobbyEngine.Models;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LobbyEngine
{
    public class MemoryStorage : IStorage
    {
        Dictionary<Type, Dictionary<string, object>> data = new Dictionary<Type, Dictionary<string, object>>();
        public Task Save<T>(T entity) where T: Entity
        {
            var type = typeof(T);
            if (!data.ContainsKey(type))
            {
                data.Add(type, new Dictionary<string, object>());
            }
            if (!data[type].ContainsKey(entity.Id))
            {
                data[type].Add(entity.Id, entity);
            }
            else
            {
                data[type][entity.Id] = entity;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAll<T>() where T: Entity
        {
            var type = typeof(T);
            if (data.ContainsKey(type))
            {
                data.Remove(type);
            }
            return Task.CompletedTask;
        }

        public Task Delete<T>(T entity) where T: Entity
        {
            var type = typeof(T);
            if (data.ContainsKey(type))
            {
                if (data[type].ContainsKey(entity.Id))
                {
                    data[type].Remove(entity.Id);
                }
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> Get<T>(Expression<Func<T, bool>> filter = null) where T: Entity
        {
            System.Linq.Expressions.Expression<Func<T, bool>> expr = t => (t.Id != null);
            Enumerable.Empty<T>().AsQueryable().Where(t => (t.Id != null));
            var type = typeof(T);
            if (data.ContainsKey(type))
            {
                var result = data[typeof(T)].Select(entity => CloneObject<T>(entity.Value));
                var filtered = filter == null ? result : result.Where(filter.Compile());
                return Task.FromResult(filtered);
            }
            else
            {
                return Task.FromResult(Enumerable.Empty<T>());
            }
        }

        T CloneObject<T>(object source)
        {
            var clone = Activator.CreateInstance<T>();
            var fields = clone.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var value = field.GetValue(source);
                field.SetValue(clone, value);
            }
            return clone;
        }
    }
}