using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace neon
{
    public enum FilterTerm
    {
        Has,
        HasNot
    }

    public struct Filter : IEquatable<Filter>
    {
        private int m_HashCode;

        private FilterTerm m_Term;
        public FilterTerm Term => m_Term;

        private Type m_Type;
        public Type Type => m_Type;

        public Filter(FilterTerm term, Type type)
        {
            m_Term = term;
            m_Type = type;

            m_HashCode = Hashing.HashPair((short)m_Term.GetHashCode(), (short)m_Type.GetHashCode());
        }

        public bool Equals(Filter other)
        {
            return this.Term == other.Term && this.Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return Equals((Filter)obj);
        }

        public override int GetHashCode()
        {
            return m_HashCode;
        }
    }
}
