using System;
using System.Collections.Generic;

namespace neon
{
    public class TablesCollection
    {
        public static TablesCollection instance;

        private List<Type> m_Types = new List<Type>();
        private List<ITableUntyped> m_Tables = new List<ITableUntyped>();

        public TablesCollection()
        {

        }

        public bool AddComponent<T> (T component, EntityID id) where T : class, IComponent
        {
            ITable<T> table = GetTable<T>();

            return table.Add(component, id);
        }

        public bool RemoveComponent<T>(EntityID id) where T : class, IComponent
        {
            ITable<T> table = GetTable<T>();

            if (table == null)
                return false;

            return table.Remove(id);
        }

        public T GetComponent<T>(EntityID id) where T : class, IComponent
        {
            ITable<T> table = GetTable<T>();

            if (table == null)
                return null;

            return table.Get(id);
        }

        public IComponent GetComponentOfType(EntityID id, Type type)
        {
            if (!type.IsSubclassOf(typeof(IComponent)))
                return null;

            ITableUntyped table = GetTableOfType(type);
            if (table == null)
                return null;

            return table.Get(id);
        }

        public ITable<T> GetTable<T>() where T : class, IComponent
        {
            for (int i = 0; i < m_Types.Count; i++)
            {
                if (m_Types[i] == typeof(T))
                {
                    return (ITable<T>)m_Tables[i];
                }
            }

            Table<T> table = new Table<T>();
            m_Types.Add(typeof(T));
            m_Tables.Add(table);

            return table;
        }

        public ITableUntyped GetTableOfType(Type type)
        {
            for (int i = 0; i < m_Types.Count; i++)
            {
                if (m_Types[i] == type)
                {
                    return m_Tables[i];
                }
            }

            return null;
        }
    }
}
