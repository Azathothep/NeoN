using System;
using System.Diagnostics;

namespace neon
{
    public class QueryNode : IEquatable<QueryNode>
    {
        private int m_HashCode;

        private Filter m_QueryFilter;
        private ITableUntyped m_Table;

        public QueryNode(Filter queryFilter, ITableUntyped table)
        {
            m_QueryFilter = queryFilter;
            m_Table = table;

            m_HashCode = m_QueryFilter.GetHashCode();

            Debug.WriteLine($"Building new node of filter {queryFilter.Term}, {queryFilter.Type}");
        }

        public bool Equals(QueryNode other)
        {
            return other.m_QueryFilter.Equals(m_QueryFilter);
        }

        public override bool Equals(object obj)
        {
            if (obj is not QueryNode)
                return false;

            return Equals((QueryNode)obj);
        }

        public override int GetHashCode()
        {
            return m_HashCode;
        }

        public bool Satisfy(EntityID id, out IComponent component)
        {
            component = null;

            if (m_QueryFilter.Term == FilterTerm.Has)
            {
                component = m_Table.Get(id);
                return component != null;
            }
            else if (m_QueryFilter.Term == FilterTerm.HasNot
                && m_Table.Get(id) == null)
                 return true;

            return false;
        }
    }
}
