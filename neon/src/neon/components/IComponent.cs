namespace neon
{
    public interface IComponent
    {
        public EntityID EntityID { get; }

        public IComponent Clone();
    }
}
