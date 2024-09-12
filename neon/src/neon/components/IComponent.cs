namespace neon
{
    public interface IComponent
    {
        public EntityID ID { get; set; }

        public IComponent Clone();
    }
}
