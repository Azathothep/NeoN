namespace neon
{
    public interface IComponent
    {
        public EntityID EntityID { get; set; }

        public IComponent Clone();
    }
}
