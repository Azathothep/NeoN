namespace neon
{
    public class BoxCollider : IComponent
    {
        public EntityID ID {get; set;}
        public BoxCollider()
        {
           
        }

        public BoxCollider(BoxCollider other)
        {

        }

        public IComponent Clone() => new BoxCollider(this);
    }
}
