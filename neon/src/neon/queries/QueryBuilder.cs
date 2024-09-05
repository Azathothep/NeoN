using System.Diagnostics;

namespace neon
{
    public enum QueryType
    {
        Cached,
        Uncached
    }

    public class QueryBuilder
    {
        public static QueryBuilder instance { get; private set; }

        private TablesCollection m_TablesCollection;

        private List<IQuery> m_CachedQueries = new List<IQuery>();
        private List<IQueryResult> m_CachedResults = new List<IQueryResult>();

        public QueryBuilder(TablesCollection tableCollection)
        {
            m_TablesCollection = tableCollection;

            if (instance != null)
                Console.WriteLine($"Queries singleton has already been created!");
            else
                instance = this;
        }

        public static IEnumerable<(EntityID, T1, T2)> Get<T1, T2>(Query<T1, T2> query, QueryType queryType) where T1 : class, IComponent where T2 : class, IComponent
        {
            if (queryType == QueryType.Uncached)
                return NewQuery(query);

            if (TryGet(query, out QueryResult<T1, T2> result))
            {
                //Debug.WriteLine("Got cached QueryResult");
                return result.GetResult();
            }

            QueryResult<T1, T2> queryResult = NewQuery(query);

            instance.m_CachedQueries.Add(query);
            instance.m_CachedResults.Add(queryResult);

            return queryResult;
        }

        private static QueryResult<T1, T2> NewQuery<T1, T2>(Query<T1, T2> query) where T1 : class, IComponent where T2 : class, IComponent
        {
            Debug.WriteLine($"Building new QueryResult of type {typeof(T1)} & {typeof(T2)}");

            QueryResult<T1, T2> qResult = new QueryResult<T1, T2>(instance.m_TablesCollection, query).Build();

            return qResult;
            
        }

        private static bool TryGet<T1, T2>(Query<T1, T2> query, out QueryResult<T1, T2> result) where T1 : class, IComponent where T2: class, IComponent
        {
            int index;
            for (index = 0; index < instance.m_CachedQueries.Count; index++)
            {
                if (instance.m_CachedQueries[index].Equals(query))
                    break;
            }

            if (index == instance.m_CachedQueries.Count)
            {
                result = null;
                return false;
            }

            result = (QueryResult<T1, T2>)instance.m_CachedResults[index];
            return true;
        }
    }
}
