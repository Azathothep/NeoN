using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public class QueryStorage : IQueryStorage
    {
        private Dictionary<IQuery, IQueryResult> m_CachedQueries = new();

        private Dictionary<ComponentID, HashSet<IQueryResult>> m_ComponentIDToQueryResults = new();

        private IComponentIteratorProvider m_IteratorProvider;

        private ComponentStorageNotifier m_ComponentStorageNotifier;

        public QueryStorage(IComponentIteratorProvider provider, ComponentStorageNotifier componentStorageNotifier)
        {
            m_IteratorProvider = provider;
            m_ComponentStorageNotifier = componentStorageNotifier;
            m_ComponentStorageNotifier.Event += OnComponentStorageModified;
        }

        public IQueryResult Get(IQuery query, QueryType queryType, Func<IComponentIteratorProvider, IQueryResult> queryResultCreator)
        {
            if (m_CachedQueries.TryGetValue(query, out IQueryResult result))
            {
                return result;
            }

            IQueryResult queryResult = queryResultCreator.Invoke(m_IteratorProvider);

            if (queryType == QueryType.Uncached)
                return queryResult;

            CacheQueryResult(query, queryResult);

            return queryResult;
        }

        private void CacheQueryResult(IQuery query, IQueryResult queryResult)
        {
            m_CachedQueries.Add(query, queryResult);

            for (int i = 0; i < query.Filters.Length; i++)
            {
                ComponentID componentID = query.Filters[i].ComponentID;
                if (!m_ComponentIDToQueryResults.TryGetValue(componentID, out HashSet<IQueryResult> resultSet))
                {
                    resultSet = new HashSet<IQueryResult>();
                    m_ComponentIDToQueryResults.Add(componentID, resultSet);
                }

                resultSet.Add(queryResult);
            }
        }

        private void OnComponentStorageModified(ComponentID componentID)
        {
            if (m_ComponentIDToQueryResults.TryGetValue(componentID, out HashSet<IQueryResult> resultSet))
            {
                foreach (var result in resultSet)
                    result.SetDirty();
            }
        }
    }
}
