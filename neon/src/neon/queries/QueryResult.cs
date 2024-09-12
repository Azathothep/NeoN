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

    public class QueryResult<T> : QueryResult, IEnumerable<(EntityID, T)> where T : class, IComponent
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T)> GetEnumerator() => GetEnumerator<(EntityID, T)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class QueryResult<T1, T2> : QueryResult, IEnumerable<(EntityID, T1, T2)> where T1 : class, IComponent where T2 : class, IComponent
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T1, T2)> GetEnumerator() => GetEnumerator<(EntityID, T1, T2)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class QueryResult<T1, T2, T3> : QueryResult, IEnumerable<(EntityID, T1, T2, T3)> where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T1, T2, T3)> GetEnumerator() => GetEnumerator<(EntityID, T1, T2, T3)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class QueryResult<T1, T2, T3, T4> : QueryResult, IEnumerable<(EntityID, T1, T2, T3, T4)> where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
    {
        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe) : base(iterableQuery, mode) { }

        public IEnumerator<(EntityID, T1, T2, T3, T4)> GetEnumerator() => GetEnumerator<(EntityID, T1, T2, T3, T4)>();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

    public abstract class QueryResult : IQueryResult
    {
        protected bool m_IsDirty = true; // Dirty when component changes

        public bool IsDirty => m_IsDirty;

		private IEnumerable? m_Storage;

        private QueryResultMode m_Mode;

        private IComponentIterator m_IterableQuery;

        public QueryResult(IComponentIterator iterableQuery, QueryResultMode mode = QueryResultMode.Safe)
        {
			m_IterableQuery = iterableQuery;
			m_Mode = mode;
        }

        public void SetDirty() => m_IsDirty = true;

		protected IEnumerable<T> MakeStorage<T>(IEnumerator<T> enumerator)
		{
			var storage = new List<T>();

			while (enumerator.MoveNext())
				storage.Add(enumerator.Current);

			return storage;
		}

        protected IEnumerator<T> GetEnumerator<T>()
        {
			if (m_Mode == QueryResultMode.Safe && !m_IsDirty)
			{
				return (IEnumerator<T>)m_Storage.GetEnumerator();
			}

			var enumerator = (IEnumerator<T>)m_IterableQuery.Create();

			if (m_Mode == QueryResultMode.Safe) // && is dirty
			{
				m_Storage = MakeStorage(enumerator);
                m_IsDirty = false;
				return (IEnumerator<T>)m_Storage.GetEnumerator();
			}

            return enumerator;
        }
    }
}
