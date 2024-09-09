using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace neon
{
    public enum FilterTerm
    {
        Has,
        HasNot,
        MightHave
    }

    public struct QueryFilter<T> : IQueryFilter where T : class, IComponent
    {
        private FilterTerm m_Term;
        public FilterTerm Term => m_Term;

        private ComponentID m_ComponentID;
        public ComponentID ComponentID => m_ComponentID;

        public QueryFilter(FilterTerm term)
        {
            m_Term = term;
            m_ComponentID = Components.GetID<T>();
        }

        public override bool Equals(object obj)
        {
            return (obj is QueryFilter<T> other) && this.Term == other.Term;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 131317;
                hashCode = hashCode * 4695947 ^ m_Term.GetHashCode();
                hashCode = hashCode * 4695947 ^ m_ComponentID.GetHashCode();

                return hashCode;
            }
        }
    }
}
