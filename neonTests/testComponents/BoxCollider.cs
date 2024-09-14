namespace neon
{
    public class BoxCollider : IComponent
    {
        public EntityID EntityID {get; set;}
        public BoxCollider()
        {
           
        }

        public BoxCollider(BoxCollider other)
        {

        }

        public IComponent Clone() => new BoxCollider(this);
    }
}
