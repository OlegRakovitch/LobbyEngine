using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace RattusEngine
{
    public class MemoryStorage : IStorage
    {
        Dictionary<Type, Dictionary<string, object>> data = new Dictionary<Type, Dictionary<string, object>>();
        public bool Save<T>(T entity) where T: Entity
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
            return true;
        }

        public bool DeleteAll<T>() where T: Entity
        {
            var type = typeof(T);
            if (data.ContainsKey(type))
            {
                data.Remove(type);
            }
            return true;
        }

        public IQueryable<T> Get<T>() where T: Entity
        {
            var type = typeof(T);
            if (data.ContainsKey(type))
            {
                return data[typeof(T)].Select(entity => CloneObject<T>(entity.Value)).AsQueryable();
            }
            else
            {
                return Enumerable.Empty<T>().AsQueryable();
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