using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace neon
{
    public enum QueryResultMode
    {
        Safe,
        Unsafe
    }

    public class QueryResult<T1, T2> : IQueryResult, IEnumerable<(EntityID, T1, T2)> where T1 : class, IComponent where T2 : class, IComponent
    {
        private class QueryResultEnumerator : IEnumerator<(EntityID, T1, T2)> // Make generic, recast everything past
        {
            private (Archetype, List<EntityID>)[] m_Archetypes; // List<EntityID> must stay List ? Maybe dangerous ?

            private int m_ArchetypeIndex = 0;
            private int m_ArchetypePosition = -1;

            private int[] m_ColumnIndices;

            private ComponentID[] m_ComponentIDs;

            public QueryResultEnumerator((Archetype, List<EntityID>)[] archetypes)
            {
                m_ComponentIDs = new ComponentID[]
                {
                    Components.GetID<T1>(),
                    Components.GetID<T2>(),
                };

                m_Archetypes = archetypes;

                if (m_Archetypes.Length == 0)
                    return;

                m_ColumnIndices = GetIndices(archetypes[0].Item1);
            }

            public (EntityID, T1, T2) Current => GetAt(m_ArchetypeIndex, m_ArchetypePosition, m_ColumnIndices);

            object IEnumerator.Current => Current;

            public void Dispose() { }

            public bool MoveNext()
            {
                if (m_ArchetypeIndex >= m_Archetypes.Length)
                {
                    return false;
                }

                m_ArchetypePosition++;
                if (m_ArchetypePosition >= m_Archetypes[m_ArchetypeIndex].Item2.Count)
                {
                    m_ArchetypeIndex++;
                    if (m_ArchetypeIndex >= m_Archetypes.Length)
                        return false;

                    m_ArchetypePosition = 0;
                    m_ColumnIndices = GetIndices(m_Archetypes[m_ArchetypeIndex].Item1);
                }

                return true;
            }

            public void Reset()
            {
                m_ArchetypeIndex = 0;
                m_ArchetypePosition = -1;
            }

            private (EntityID, T1, T2) GetAt(int archetypeIndex, int archetypePosition, int[] columnIndices)
            {
                return (m_Archetypes[archetypeIndex].Item2[archetypePosition],
                    (T1)m_Archetypes[archetypeIndex].Item1.Columns[columnIndices[0]][archetypePosition],
                    (T2)m_Archetypes[archetypeIndex].Item1.Columns[columnIndices[1]][archetypePosition]);
            }

            private int[] GetIndices(Archetype archetype)
            {
                int[] indices = new int[2];

                int queryIDIndex = 0;

                List<ComponentID> componentIDs = archetype.ComponentSet.ComponentIDs;
                for (int i = 0; i < componentIDs.Count; i++)
                {
                    if (m_ComponentIDs[queryIDIndex] == componentIDs[i])
                    {
                        indices[queryIDIndex] = i;
                        queryIDIndex++;

                        if (queryIDIndex >= m_ComponentIDs.Length)
                            break;
                    }
                }

                return indices;
            }
        }

        private Query<T1, T2> m_Query;

        private bool m_IsDirty = true;
        public bool IsDirty => m_IsDirty;

        (Archetype, List<EntityID>)[] m_RequestedArchetypes;

        private QueryResultMode m_Mode;

        public QueryResult(Query<T1, T2> query, QueryResultMode mode = QueryResultMode.Safe) {
            m_Query = query;
            m_Mode = mode;
        }

        public void Build()
        {
            m_RequestedArchetypes = Components.RequestArchetypes(m_Query);

            m_IsDirty = false;
        }

        private List<(EntityID, T1, T2)> Store(QueryResultEnumerator enumerator)
        {
            List<(EntityID, T1, T2)> storage = new();

            while (enumerator.MoveNext())
                storage.Add(enumerator.Current);

            return storage;
        }

        public IEnumerator<(EntityID, T1, T2)> GetEnumerator()
        {
            var enumerator = new QueryResultEnumerator(m_RequestedArchetypes);

            if (m_Mode == QueryResultMode.Safe)
                return Store(enumerator).GetEnumerator();
            
            return enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
