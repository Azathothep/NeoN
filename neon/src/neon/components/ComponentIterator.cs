using System.Diagnostics;

namespace neon
{
    public abstract class ComponentIterator : IComponentIterator
    {
        protected bool m_IsDirty = true;
        public bool IsDirty => m_IsDirty;

        protected bool m_IncludeInactive;

        protected Func<(Archetype, List<EntityID>)[]> m_RequestArchetypes;

        protected (Archetype, List<EntityID>)[] m_RequestedArchetypes;

        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive)
        {
            m_RequestArchetypes = requestArchetypes;
            m_IncludeInactive = includeInactive;
            Create();
        }

        public void SetDirty() => m_IsDirty = true;

        public IQueryIterator Create()
        {
            if (m_IsDirty)
            {
                Debug.WriteLine("IsDirty: rebuilding");
                m_RequestedArchetypes = m_RequestArchetypes.Invoke();
                m_IsDirty = false;
            }

            return CreateInternal(m_RequestedArchetypes);
        }

        protected abstract IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes);
    }

    public class ComponentIterator<T> : ComponentIterator where T : class, IComponent
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T>(archetypes, m_IncludeInactive);
    }

    public class ComponentIterator<T1, T2> : ComponentIterator where T1 : class, IComponent where T2 : class, IComponent
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T1, T2>(archetypes, m_IncludeInactive);

    }

    public class ComponentIterator<T1, T2, T3> : ComponentIterator where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T1, T2, T3>(archetypes, m_IncludeInactive);
    }

    public class ComponentIterator<T1, T2, T3, T4> : ComponentIterator where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
    {
        public ComponentIterator(Func<(Archetype, List<EntityID>)[]> requestArchetypes, bool includeInactive) : base(requestArchetypes, includeInactive) { }

        protected override IQueryIterator CreateInternal((Archetype, List<EntityID>)[] archetypes) => new QueryIterator<T1, T2, T3, T4>(archetypes, m_IncludeInactive);
    }
}
