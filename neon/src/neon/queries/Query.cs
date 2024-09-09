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
            m_Filters = ProcessFilters(filters);

            Debug.WriteLine($"New query created with hashCode: {this.GetHashCode()}");
        }

        private IQueryFilter[] MakeFilters()
        {
            IQueryFilter[] queryFilters = new IQueryFilter[2];

            queryFilters[0] = new QueryFilter<T1>(FilterTerm.Has);
            queryFilters[1] = new QueryFilter<T2>(FilterTerm.Has);

            return queryFilters.ToArray();
        }

        private IQueryFilter[] ProcessFilters(IQueryFilter[] filters)
        {
            List<ComponentID> templateComponentIDs = new List<ComponentID>
            {
                Components.GetID<T1>(),
                Components.GetID<T2>()
            };

            List<IQueryFilter> queryFilters = new List<IQueryFilter>();

            // remove doubles

            for (int i = 0; i < filters.Length; i++)
            {
                // Don't add if another filter with this component is already present

                if (queryFilters.Find((f) => f.ComponentID == filters[i].ComponentID) != null)
                {
                    Debug.WriteLine($"QueryFilter's component {filters[i].ComponentID} already present in Query. Keeping only the first one.");
                    continue;
                }

                // Don't add if filter component is one of the template parameter components & is indicated as "Has not"

                if (filters[i].Term == FilterTerm.HasNot && templateComponentIDs.Contains(filters[i].ComponentID))
                    continue;

                queryFilters.Add(filters[i]);
            }

            // add T1 & T2 if not present

            if (queryFilters.Find((f) => f.ComponentID == templateComponentIDs[0]) == null)
                queryFilters.Add(new QueryFilter<T1>(FilterTerm.Has));

            if (queryFilters.Find((f) => f.ComponentID == templateComponentIDs[1]) == null)
                queryFilters.Add(new QueryFilter<T2>(FilterTerm.Has));

            // sort them with "Has" first, then "Has Not", then "Might Have"

            queryFilters.Sort(SortByTerm);

            return queryFilters.ToArray();
        }

        private int SortByTerm(IQueryFilter f1, IQueryFilter f2)
        {
            return f1.Term.CompareTo(f2.Term);
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
