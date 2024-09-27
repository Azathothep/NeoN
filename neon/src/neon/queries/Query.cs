using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace neon
{
    public class Query<T1> : Query where T1 : class, IComponent
    {
        public Query(bool includeInactive = false) : base(new ComponentID[] { Components.GetID<T1>() }, includeInactive) { }

        public Query(IQueryFilter[] filters, bool includeInactive = false) : base(new ComponentID[] { Components.GetID<T1>() }, filters, includeInactive) { }

        protected override IQueryFilter[] MakeFilters()
        {
            IQueryFilter[] queryFilters = new IQueryFilter[1];

            queryFilters[0] = new QueryFilter<T1>(FilterTerm.Has);

            return queryFilters.ToArray();
        }

        protected override bool EqualsType(object obj)
        {
            return obj is Query<T1>;
        }
    }

    public class Query<T1, T2> : Query where T1 : class, IComponent where T2 : class, IComponent
    {
        public Query(bool includeInactive = false) : base(new ComponentID[] { Components.GetID<T1>(), Components.GetID<T2>() }, includeInactive) { }

        public Query(IQueryFilter[] filters, bool includeInactive = false) : base(new ComponentID[] { Components.GetID<T1>(), Components.GetID<T2>() }, filters, includeInactive) { }

        protected override IQueryFilter[] MakeFilters()
        {
            IQueryFilter[] queryFilters = new IQueryFilter[2];

            queryFilters[0] = new QueryFilter<T1>(FilterTerm.Has);
            queryFilters[1] = new QueryFilter<T2>(FilterTerm.Has);

            return queryFilters.ToArray();
        }

        protected override bool EqualsType(object obj)
        {
            return obj is Query<T1, T2>;
        }
    }

    public class Query<T1, T2, T3> : Query where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
    {
        public Query(bool includeInactive = false) : base(new ComponentID[] { Components.GetID<T1>(), Components.GetID<T2>(), Components.GetID<T3>() }, includeInactive) { }

        public Query(IQueryFilter[] filters, bool includeInactive = false) : base(new ComponentID[] { Components.GetID<T1>(), Components.GetID<T2>(), Components.GetID<T3>() }, filters, includeInactive) { }

        protected override IQueryFilter[] MakeFilters()
        {
            IQueryFilter[] queryFilters = new IQueryFilter[3];

            queryFilters[0] = new QueryFilter<T1>(FilterTerm.Has);
            queryFilters[1] = new QueryFilter<T2>(FilterTerm.Has);
            queryFilters[2] = new QueryFilter<T3>(FilterTerm.Has);

            return queryFilters.ToArray();
        }

        protected override bool EqualsType(object obj)
        {
            return obj is Query<T1, T2, T3>;
        }
    }

    public class Query<T1, T2, T3, T4> : Query where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
    {
        public Query(bool includeInactive = false) : base(new ComponentID[] { Components.GetID<T1>(), Components.GetID<T2>(), Components.GetID<T3>(), Components.GetID<T4>() }, includeInactive) { }

        public Query(IQueryFilter[] filters, bool includeInactive = false) : base(new ComponentID[] { Components.GetID<T1>(), Components.GetID<T2>(), Components.GetID<T3>(), Components.GetID<T4>() }, filters, includeInactive) { }

        protected override IQueryFilter[] MakeFilters()
        {
            IQueryFilter[] queryFilters = new IQueryFilter[4];

            queryFilters[0] = new QueryFilter<T1>(FilterTerm.Has);
            queryFilters[1] = new QueryFilter<T2>(FilterTerm.Has);
            queryFilters[2] = new QueryFilter<T3>(FilterTerm.Has);
            queryFilters[3] = new QueryFilter<T4>(FilterTerm.Has);

            return queryFilters.ToArray();
        }

        protected override bool EqualsType(object obj)
        {
            return obj is Query<T1, T2, T3, T4>;
        }
    }

    public abstract class Query : IQuery
    {
        protected IQueryFilter[] m_Filters;
        public IQueryFilter[] Filters => m_Filters;

        private ComponentID[] m_ReturnValues;
        public ComponentID[] ReturnValues => m_ReturnValues;

        private bool m_IncludeInactive;
        public bool IncludeInactive => m_IncludeInactive;

        public Query(ComponentID[] returnValues, bool includeInactive = false)
        {
            m_ReturnValues = returnValues;
            m_Filters = MakeFilters();
            m_IncludeInactive = includeInactive;
        }

        public Query(ComponentID[] returnValues, IQueryFilter[] filters, bool includeInactive = false)
        {
            m_ReturnValues = returnValues;
            m_Filters = ProcessFilters(filters);
            m_IncludeInactive = includeInactive;

            Debug.WriteLine($"New query created with hashCode: {this.GetHashCode()}");
        }

        protected abstract IQueryFilter[] MakeFilters();

        private IQueryFilter[] ProcessFilters(IQueryFilter[] filters)
        {
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

                if (filters[i].Term == FilterTerm.HasNot && m_ReturnValues.Contains(filters[i].ComponentID))
                    continue;

                queryFilters.Add(filters[i]);
            }

            // add template types if not present

            for (int i = 0; i < m_ReturnValues.Length; i++)
            {
                if (queryFilters.Find((f) => f.ComponentID == m_ReturnValues[i]) == null)
                    queryFilters.Add(new QueryFilter(FilterTerm.Has, m_ReturnValues[i]));
            }

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
            if (obj is not Query || this.EqualsType(obj) == false)
                return false;

            Query other = (Query)obj;

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

        protected abstract bool EqualsType(object obj);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 190633;

                for (int i = 0; i < m_Filters.Length; i++)
                    hashCode = hashCode * 7194917 ^ m_Filters[i].GetHashCode();

                hashCode = hashCode * 7194917 ^ m_IncludeInactive.GetHashCode();

                return hashCode;
            }
        }
    }
}
