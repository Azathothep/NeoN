namespace neon
{
    public interface IQueryResult
    {
        public bool IsDirty { get; }

        public void Build();
    }
}
