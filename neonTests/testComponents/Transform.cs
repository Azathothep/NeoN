using System.Numerics;

namespace neon
{
    public class Transform : IComponent
    {
        public EntityID EntityID { get; set; }

        public Vector2 position;
        public Vector2 rotation;
        public Vector2 scale;

        public Transform()
        {
        }

        public Transform(Transform other)
        {
            position = other.position;
            rotation = other.rotation;
            scale = other.scale;
        }

        public IComponent Clone() => new Transform(this);
    }
}
