namespace neon
{
    public class BoxCollider : IComponent
    {
        public BoxCollider()
        {
           
        }

        public BoxCollider(BoxCollider other)
        {

        }

        public IComponent Clone() => new BoxCollider(this);
    }
}
