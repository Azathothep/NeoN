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
        private class QueryStorage
        {
            public Dictionary<IQuery, IQueryResult> CachedQueries = new();
        }

        private static QueryStorage storage = new();

        private QueryBuilder() { }

        public static IEnumerable<(EntityID, T1, T2)> Get<T1, T2>(Query<T1, T2> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : class, IComponent where T2 : class, IComponent
        {
            if (storage.CachedQueries.TryGetValue(query, out IQueryResult? result))
            {
                if (result.IsDirty)
                {
                    Debug.WriteLine("Query dirty : updating");
                    result.Build();
                }

                return ((QueryResult<T1, T2>)result);
            }

            QueryResult<T1, T2> queryResult = CreateQuery(query, mode);

            if (queryType == QueryType.Uncached)
                return queryResult;

            storage.CachedQueries.Add(query, queryResult);

            return queryResult;
        }

        private static QueryResult<T1, T2> CreateQuery<T1, T2>(Query<T1, T2> query, QueryResultMode mode) where T1 : class, IComponent where T2 : class, IComponent
        {
            Debug.WriteLine($"Building new QueryResult with types {typeof(T1)} & {typeof(T2)}");

            QueryResult<T1, T2> qResult = new QueryResult<T1, T2>(query, mode);
            qResult.Build();

            return qResult;
        }
    }
}
