using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public class Component
    {
        public static Component storage { get; private set; }

        private Dictionary<Type, ComponentID> m_ComponentToID = new();
        private Dictionary<ComponentID, Type> m_IDToComponent = new();

        private int m_LastID = -1;

        public Component()
        {
            if (storage == null)
                storage = this;
            else
                throw new InvalidOperationException($"An object of type {this.GetType()} has already been created!");
        }

        public static Type GetType(ComponentID component)
        {
            return storage.m_IDToComponent[component];
        }

        public static ComponentID GetID<T>() where T : class, IComponent
        {
            Type componentType = typeof(T);

            return GetIDByTypeUnsafe(componentType);
        }

        public static ComponentID GetIDByType(Type componentType)
        {
            if (!componentType.IsSubclassOf(typeof(IComponent)))
                return null;

            return GetIDByTypeUnsafe(componentType);
        }

        private static ComponentID GetIDByTypeUnsafe(Type componentType)
        {
            if (storage.m_ComponentToID.TryGetValue(componentType, out ComponentID componentID))
                return componentID;

            componentID = new ComponentID(++storage.m_LastID);

            storage.AddUnsafe(componentType, componentID);

            return componentID;
        }

        private void AddUnsafe(Type componentType, ComponentID id)
        {
            m_ComponentToID.Add(componentType, id);
            m_IDToComponent.Add(id, componentType);
        }
    }
}
