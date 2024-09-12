using System.Collections;

namespace neon
{
    public interface IComponentIterator
    {
        public bool IsDirty { get; }

        public void SetDirty();

        public IQueryIterator Create();
    }
}
