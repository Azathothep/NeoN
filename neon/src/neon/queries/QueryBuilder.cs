using System.Collections.Generic;
using System.Diagnostics;
using static neon.ComponentStorage;

namespace neon
{
    public enum QueryType
    {
        Cached,
        Uncached
    }

    public class QueryBuilder
    {
        private static IQueryStorage storage;

        private QueryBuilder() { }

        public static void SetStorage(IQueryStorage storage) {
            QueryBuilder.storage = storage;
        }

        public static IEnumerable<(EntityID, T1)> Get<T1>(Query<T1> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : class, IComponent
        {
            return (IEnumerable<(EntityID, T1)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1>(query.Filters, queryType);
                    return new QueryResult<T1>(iterator, mode);
                }
            );
        }

        public static IEnumerable<(EntityID, T1, T2)> Get<T1, T2>(Query<T1, T2> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : class, IComponent where T2 : class, IComponent
        {
            return (IEnumerable<(EntityID, T1, T2)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1, T2>(query.Filters, queryType);
                    return new QueryResult<T1, T2>(iterator, mode);
                }
            );
        }

        public static IEnumerable<(EntityID, T1, T2, T3)> Get<T1, T2, T3>(Query<T1, T2, T3> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            return (IEnumerable<(EntityID, T1, T2, T3)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1, T2, T3>(query.Filters, queryType);
                    return new QueryResult<T1, T2, T3>(iterator, mode);
                }
            );
        }

        public static IEnumerable<(EntityID, T1, T2, T3, T4)> Get<T1, T2, T3, T4>(Query<T1, T2, T3, T4> query, QueryType queryType, QueryResultMode mode = QueryResultMode.Safe) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            return (IEnumerable<(EntityID, T1, T2, T3, T4)>)
                storage.Get(query, queryType, (provider) =>
                {
                    IComponentIterator iterator = provider.Get<T1, T2, T3, T4>(query.Filters, queryType);
                    return new QueryResult<T1, T2, T3, T4>(iterator, mode);
                }
            );
        }
    }
}
