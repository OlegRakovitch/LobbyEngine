using System.Linq;

namespace RattusEngine.Fixtures
{
    public abstract class QueryFixture<T>
    {
        protected abstract T[] Data { get; }
        protected abstract string[] Fields { get; }

        public object[] Query()
        {
            var type = typeof(T);
            var fields = Fields.Select(field => type.GetField(field));
            return Data.Select(row => fields.Select(field => new object[] { field.Name, field.GetValue(row) }).ToArray()).ToArray();
        }
    }
}