using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace neon
{
    public class Query<T1, T2> : IQuery, IEquatable< Query<T1, T2> >
    {
        private int m_HashCode;

        private Filter[] m_Filters;
        public Filter[] Filters => m_Filters;

        public Query(Filter[] filters)
        {
            m_Filters = filters;

            m_HashCode = BuildHash(filters);

            Debug.WriteLine($"New query created with hashCode: {m_HashCode}");
        }

        private int BuildHash(Filter[] filters)
        {
            if (filters.Length == 0)
                return 0;

            if (filters.Length == 1)
                return filters.GetHashCode();

            int hash = Hashing.HashPair((short)filters[0].GetHashCode(), (short)filters[1].GetHashCode());

            for (int i = 2; i < filters.Length; i++)
            {
                hash = Hashing.HashPair((short)hash, (short)filters[i].GetHashCode());
            }

            return hash;
        }

        public bool Equals(Query<T1, T2> other)
        {
            if (this.Filters.Length != other.Filters.Length)
                return false;

            List<Filter> filterList = other.Filters.ToList();

            for (int i = 0; i < this.Filters.Length; i++)
            {
                if (filterList.Contains(this.Filters[i]))
                    filterList.Remove(this.Filters[i]);
                else
                    return false;
            }

            return true;
        }

        public bool Equals(IQuery other)
        {
            if (other is not Query<T1, T2>)
                return false;

            return Equals((Query<T1, T2>)other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Query<T1, T2>);
        }

        public override int GetHashCode()
        {
            return m_HashCode;
        }
    }
}
