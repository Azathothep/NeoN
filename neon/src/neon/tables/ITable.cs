using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public interface ITable<T> : ITableUntyped, IEnumerable<KeyValuePair<EntityID, T>> where T : class, IComponent
    {
        public bool Add(T component, EntityID id);
        public bool Remove(EntityID id);
        public bool Contains(EntityID id);
        public new T Get(EntityID id);

        public event Action<T> OnComponentAdded;
        public event Action<T> OnComponentRemoved;
    }
}
