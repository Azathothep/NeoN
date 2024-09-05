using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public class Table<T> : ITable<T> where T : class, IComponent
    {
        private Dictionary<EntityID, T> m_Components; // Change this to double list

        public int Count => m_Components.Count;

        public event Action<T> OnComponentAdded;
        public event Action<T> OnComponentRemoved;

        public Table()
        {
            m_Components = new Dictionary<EntityID, T>();
        }

        public bool Add(T component, EntityID id)
        {
            if (m_Components.ContainsKey(id))
                return false;

            m_Components.Add(id, component);
            OnComponentAdded?.Invoke(component);

            return true;

        }

        public bool Remove(EntityID id)
        {
            if (m_Components.ContainsKey(id) == false)
                return false;

            T component = m_Components[id];
            m_Components.Remove(id);
            OnComponentRemoved?.Invoke(component);

            return true;
        }

        public bool Contains(EntityID id)
        {
            return m_Components.ContainsKey(id);
        }

        public bool TryGet(EntityID id, out T component)
        {
            if (Contains(id))
            {
                component = m_Components[id];
                return true;
            }

            component = null;
            return false;
        }

        public T Get(EntityID id)
        {
            if (m_Components.ContainsKey(id))
                return m_Components[id];

            return null;
        }

        public IEnumerator<KeyValuePair<EntityID, T>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<EntityID, T>>)m_Components).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_Components).GetEnumerator();
        }

        IComponent ITableUntyped.Get(EntityID id)
        {
            return Get(id);
        }
    }
}
