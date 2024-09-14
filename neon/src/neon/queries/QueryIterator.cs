using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public class QueryIterator<T> : QueryIterator, IEnumerator<(EntityID, T?)> where T : class, IComponent
    {
        public QueryIterator((Archetype, List<EntityID>)[] archetypes, bool includeInactive) : base(archetypes, new ComponentID[] { Components.GetID<T>() }, includeInactive)
        { }

        public (EntityID, T?) Current => GetCurrentResult<T>();

        object IEnumerator.Current => Current;
    }

    public class QueryIterator<T1, T2> : QueryIterator, IEnumerator<(EntityID, T1?, T2?)> where T1 : class, IComponent where T2 : class, IComponent
    {
        public QueryIterator((Archetype, List<EntityID>)[] archetypes, bool includeInactive) : base(archetypes, new ComponentID[] { Components.GetID<T1>(), Components.GetID<T2>() }, includeInactive)
        { }

        public (EntityID, T1?, T2?) Current => GetCurrentResult<T1, T2>();

        object IEnumerator.Current => Current;
    }

    public class QueryIterator<T1, T2, T3> : QueryIterator, IEnumerator<(EntityID, T1?, T2?, T3?)> where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
    {
        public QueryIterator((Archetype, List<EntityID>)[] archetypes, bool includeInactive) : base(archetypes, new ComponentID[] { Components.GetID<T1>(), Components.GetID<T2>(), Components.GetID<T3>() }, includeInactive)
        { }

        public (EntityID, T1?, T2?, T3?) Current => GetCurrentResult<T1, T2, T3>();

        object IEnumerator.Current => Current;
    }

    public class QueryIterator<T1, T2, T3, T4> : QueryIterator, IEnumerator<(EntityID, T1?, T2?, T3?, T4?)> where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
    {
        public QueryIterator((Archetype, List<EntityID>)[] archetypes, bool includeInactive) : base(archetypes, new ComponentID[] { Components.GetID<T1>(), Components.GetID<T2>(), Components.GetID<T3>(), Components.GetID<T4>() }, includeInactive)
        { }

        public (EntityID, T1?, T2?, T3?, T4?) Current => GetCurrentResult<T1, T2, T3, T4>();

        object IEnumerator.Current => Current;
    }

    public abstract class QueryIterator : IQueryIterator
    {
        private (Archetype, List<EntityID>)[] m_Archetypes; // List<EntityID> must stay List ? Maybe dangerous ?

        private int m_ArchetypeIndex = 0;
        private int m_ArchetypePosition = -1;

        private int[] m_ColumnIndices;

        private ComponentID[] m_ComponentIDs;

        public Func<bool> MoveNextAction;

        private bool m_IncludeInactive;

        public QueryIterator((Archetype, List<EntityID>)[] archetypes, ComponentID[] componentIDs, bool includeInactive)
        {
            m_Archetypes = archetypes;
            m_ComponentIDs = componentIDs;

            if (m_Archetypes.Length == 0)
                return;

            m_ColumnIndices = GetIndices(archetypes[0].Item1);

            m_IncludeInactive = includeInactive;

			m_ArchetypePosition = m_IncludeInactive ? -1 : m_Archetypes[m_ArchetypeIndex].Item1.DisabledEndIndex;
        }

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
                return MoveToNextArchetype();
            }

            return true;
        }

        private bool MoveToNextArchetype()
        {
            while (m_ArchetypePosition >= m_Archetypes[m_ArchetypeIndex].Item2.Count)
            {
                m_ArchetypeIndex++;
                if (m_ArchetypeIndex >= m_Archetypes.Length)
                    return false;

                m_ArchetypePosition = m_IncludeInactive ? 0 : m_Archetypes[m_ArchetypeIndex].Item1.DisabledEndIndex + 1;
            }

            m_ColumnIndices = GetIndices(m_Archetypes[m_ArchetypeIndex].Item1);

            return true;
        }

        public void Reset()
        {
            m_ArchetypeIndex = 0;
            m_ArchetypePosition = -1;
        }

        protected (EntityID, T1?) GetCurrentResult<T1>() where T1 : class, IComponent
        {
            return (m_Archetypes[m_ArchetypeIndex].Item2[m_ArchetypePosition],
                Get<T1>(m_ColumnIndices[0]));
        }

        protected (EntityID, T1?, T2?) GetCurrentResult<T1, T2>() where T1 : class, IComponent where T2 : class, IComponent
        {
            return (m_Archetypes[m_ArchetypeIndex].Item2[m_ArchetypePosition],
                Get<T1>(m_ColumnIndices[0]),
                Get<T2>(m_ColumnIndices[1]));
        }

        protected (EntityID, T1?, T2?, T3?) GetCurrentResult<T1, T2, T3>() where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            return (m_Archetypes[m_ArchetypeIndex].Item2[m_ArchetypePosition],
                Get<T1>(m_ColumnIndices[0]),
                Get<T2>(m_ColumnIndices[1]),
                Get<T3>(m_ColumnIndices[2]));
        }

        protected (EntityID, T1?, T2?, T3?, T4?) GetCurrentResult<T1, T2, T3, T4>() where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            return (m_Archetypes[m_ArchetypeIndex].Item2[m_ArchetypePosition],
                Get<T1>(m_ColumnIndices[0]),
                Get<T2>(m_ColumnIndices[1]),
                Get<T3>(m_ColumnIndices[2]),
                Get<T4>(m_ColumnIndices[3]));
        }

        private T? Get<T>(int columnIndice) where T : class, IComponent
        {
            if (columnIndice < 0)
                return null;

            return (T)m_Archetypes[m_ArchetypeIndex].Item1.Columns[columnIndice][m_ArchetypePosition];
        }

        private int[] GetIndices(Archetype archetype)
        {
            int[] indices = new int[m_ComponentIDs.Length];

            List<ComponentID> componentIDs = archetype.ComponentSet.ComponentIDs;
            for (int i = 0; i < m_ComponentIDs.Length; i++)
                indices[i] = componentIDs.IndexOf(m_ComponentIDs[i]);

            return indices;
        }
    }
}
