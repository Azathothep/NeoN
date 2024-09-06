namespace neon
{
    public struct QueryFilterType : IQueryFilter
    {
        private FilterTerm m_Term;
        public FilterTerm Term => m_Term;

        private ComponentID m_ComponentID;
        public ComponentID ComponentID => m_ComponentID;

        public QueryFilterType(ComponentID componentID, FilterTerm term)
        {
            m_ComponentID = componentID;
            m_Term = term;
        }
    }
}
