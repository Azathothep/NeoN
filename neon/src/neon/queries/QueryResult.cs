using System;
using System.Collections;
using System.Collections.Generic;

namespace neon
{
    public class QueryResult<T1, T2> : IQueryResult, IEnumerable<(EntityID, T1, T2)> where T1 : class, IComponent where T2 : class, IComponent
    {
        private TablesCollection m_TablesCollection;

        private List<QueryNode> m_QueryNodes = new List<QueryNode>();

        private List<(EntityID, T1, T2)> m_Result = new List<(EntityID, T1, T2)>();

        private bool m_IsDirty;
        public bool IsDirty => m_IsDirty;

        public QueryResult(TablesCollection tableCollection, Query<T1, T2> query)
        {
            m_TablesCollection = tableCollection;

            m_IsDirty = true;

            BuildNodes(query);
        }

        public void BuildNodes(Query<T1, T2> query)
        {
            for (int i = 0; i < query.Filters.Length; i++)
            {
                // Find a way to check if query List already contains nodes BEFORE building a new one
                QueryNode qNode = new QueryNode(query.Filters[i], m_TablesCollection.GetTableOfType(query.Filters[i].Type));
                if (!m_QueryNodes.Contains(qNode))
                    m_QueryNodes.Add(qNode);
            }
        }

        public QueryResult<T1, T2> Build()
        {
            ITable<T1> m_FirstTable = m_TablesCollection.GetTable<T1>();

            foreach (var pair in m_FirstTable)
            {
                if (PassFilter(pair.Key, out (EntityID, T1, T2) result))
                    m_Result.Add(result);
            }

            m_IsDirty = false;

            return this;
        }

        private bool PassFilter(EntityID id, out (EntityID, T1, T2) result)
        {
            List<IComponent> components = new List<IComponent>();

            for (int i = 0; i < m_QueryNodes.Count; i++)
            {
                if (m_QueryNodes[i].Satisfy(id, out IComponent component))
                {
                    components.Add(component);
                    continue;
                }

                result = default;
                return false;
            }

            result = GetResult(id, components);
            return true;
        }

        private T GetComponent<T>(List<IComponent> components) where T : class, IComponent
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == typeof(T))
                    return (T)components[i];
            }

            return null;
        }

        public QueryResult<T1, T2> GetResult()
        {
            if (m_IsDirty)
                Refresh();

            return this;
        }

        private void SetDirty() => m_IsDirty = true;

        private void Refresh()
        {
            m_IsDirty = false;
        }

        public (EntityID, T1, T2) GetResult(EntityID id, List<IComponent> components)
        {
            return (id, GetComponent<T1>(components), GetComponent<T2>(components));
        }

        public IEnumerator<(EntityID, T1, T2)> GetEnumerator()
        {
            return ((IEnumerable<(EntityID, T1, T2)>)m_Result).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_Result).GetEnumerator();
        }
    }
}
