using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;

namespace neon
{
    public static partial class Components
    {
        private static IComponentStorage storage;

        public static void SetStorage(IComponentStorage storage)
        {
            Components.storage = storage;
        }

        public static bool Has<T>(EntityID entityID) where T : class, IComponent
        {
            return storage.Has<T>(entityID);
        }

        public static T? Get<T>(EntityID entityID) where T : class, IComponent
        {
            return storage.Get<T>(entityID);
        }

		public static IComponent[] GetAll(EntityID entityID) => storage.GetAll(entityID);

        public static (T1?, T2?) Get<T1, T2>(EntityID entityID) where T1 : class, IComponent where T2 : class, IComponent
        {
            object[] rawComponents = storage.GetComponentsInternal(entityID, new ComponentID[]
            {
                Components.GetID<T1>(),
                Components.GetID<T2>()
            });

            return ((T1)rawComponents[0], ((T2)rawComponents[1]));
        }

        public static (T1?, T2?, T3?) Get<T1, T2, T3>(EntityID entityID) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent
        {
            object[] rawComponents = storage.GetComponentsInternal(entityID, new ComponentID[]
            {
                Components.GetID<T1>(),
                Components.GetID<T2>(),
                Components.GetID<T3>()
            });

            return ((T1)rawComponents[0], (T2)rawComponents[1], (T3)rawComponents[2]);
        }

        public static (T1?, T2?, T3?, T4?) Get<T1, T2, T3, T4>(EntityID entityID) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent
        {
            object[] rawComponents = storage.GetComponentsInternal(entityID, new ComponentID[]
            {
                Components.GetID<T1>(),
                Components.GetID<T2>(),
                Components.GetID<T3>(),
                Components.GetID<T4>()
            });

            return ((T1)rawComponents[0], (T2)rawComponents[1], (T3)rawComponents[2], (T4)rawComponents[3]);
        }

        public static bool TryGet<T>(EntityID entityID, out T? result) where T : class, IComponent
        {
            result = storage.Get<T>(entityID);
            return result != null;
        }

        public static T? Add<T>(EntityID entityID) where T : class, IComponent, new()
        {
            return storage.Add(entityID, new T());
        }

        public static T? Add<T>(EntityID entityID, T inputComponent) where T : class, IComponent
        {
            return storage.Add(entityID, (T)inputComponent.Clone());
        }

        public static IComponent? Add(EntityID entityID, IComponent inputComponent, Type type)
        {
            return storage.Add(entityID, inputComponent, type);
        }

        public static void Remove<T>(EntityID entityID) where T : class, IComponent
        {
            storage.Remove<T>(entityID);
        }

        public static void RemoveAll(EntityID entityID)
        {
            storage.Remove(entityID);
        }

        public static EntityID GetOwner<T>(T component) where T : class, IComponent => storage.GetOwner(component);

        public static T[] GetInChildren<T>(EntityID entityID, bool propagate = false) where T : class, IComponent
        {
            return storage.GetInChildren<T>(entityID, propagate);
        }

        public static T[] GetInParents<T>(EntityID entityID) where T : class, IComponent
        {
            return storage.GetInParents<T>(entityID);
        }
    }
}
