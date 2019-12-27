using System;

namespace RattusEngine.Models
{
    public abstract class Entity
    {
        public string Id;

        public Entity()
        {
            Id = Guid.NewGuid().ToString();
        }

        public override bool Equals(object obj)
        {
            var other = obj as Entity;
            return other != null && other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return $"{GetType().Name}{Id}".GetHashCode();
        }
    }
}