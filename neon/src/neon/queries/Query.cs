using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace neon
{
    public class Query<T1, T2> : IQuery where T1 : class, IComponent where T2 : class, IComponent
    {
        private IQueryFilter[] m_Filters;
        public IQueryFilter[] Filters => m_Filters;

        public Query()
        {
            m_Filters = MakeFilters();
        }

        public Query(IQueryFilter[] filters)
        {
            m_Filters = MakeFilters(filters);

            Debug.WriteLine($"New query created with hashCode: {this.GetHashCode()}");
        }

        private IQueryFilter[] MakeFilters()
        {
            List<ComponentID> baseComponentIDs = new List<ComponentID>
            {
                Components.GetID<T1>(),
                Components.GetID<T2>()
            };

            List<IQueryFilter> queryFilters = new();

            for (int i = 0; i < baseComponentIDs.Count; i++)
            {
                queryFilters.Add(new QueryFilterType(baseComponentIDs[i], FilterTerm.Has));
            }

            return queryFilters.ToArray();
        }

        private IQueryFilter[] MakeFilters(IQueryFilter[] filters)
        {
            List<ComponentID> baseComponentIDs = new List<ComponentID>
            {
                Components.GetID<T1>(),
                Components.GetID<T2>()
            };

            List<IQueryFilter> queryFilters = new List<IQueryFilter>(filters);

            for (int i = 0; i < baseComponentIDs.Count; i++)
            {
                if (queryFilters.Find((f) => f.ComponentID == baseComponentIDs[i]) == null)
                    queryFilters.Add(new QueryFilterType(baseComponentIDs[i], FilterTerm.Has));
            }

            return queryFilters.ToArray();
        }

        public override bool Equals(object obj)
        {
            if (obj is not Query<T1, T2>)
                return false;

            Query<T1, T2> other = (Query<T1, T2>)obj;

            if (this.Filters.Length != other.Filters.Length)
                return false;

            List<IQueryFilter> filterList = other.Filters.ToList();

            for (int i = 0; i < this.Filters.Length; i++)
            {
                if (filterList.Contains(this.Filters[i]))
                    filterList.Remove(this.Filters[i]);
                else
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 190633;

                for (int i = 0; i < m_Filters.Length; i++)
                    hashCode = hashCode * 7194917 ^ m_Filters[i].GetHashCode();

                return hashCode;
            }
        }
    }
}
