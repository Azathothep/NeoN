using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public interface IComponentStorage
    {
        IComponentIteratorProvider IteratorProvider { get; }

        public T? Get<T>(EntityID entityID) where T : class, IComponent;

        public object[] GetComponentsInternal(EntityID entityID, ComponentID[] componentIDs);

        public bool Has<T>(EntityID entityID) where T : class, IComponent;

        public T? Add<T>(EntityID entityID, T component) where T : class, IComponent;

        public void Remove<T>(EntityID entityID) where T : class, IComponent;

        public void Remove(EntityID entityID);

        //CHANGE THIS TO AN EVENTREFERENCE
        public void OnEntityActiveStateChanged(EntityID entityID, bool newActiveState);
    }
}
