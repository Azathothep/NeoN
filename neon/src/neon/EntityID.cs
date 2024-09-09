using System;
using System.Collections.Generic;
using System.Linq;

namespace neon
{
    public class EntityID
    {
        private UInt32 m_ID;
        public UInt32 ID => m_ID;

        public EntityID(UInt32 value)
        {
            this.m_ID = value;
        }

        public T? Add<T>() where T : class, IComponent, new() => neon.Components.Add<T>(this);
        public T? Add<T>(T inputComponent) where T : class, IComponent => neon.Components.Add<T>(this, inputComponent);

        public T? Get<T>() where T : class, IComponent => neon.Components.Get<T>(this);
        public void Remove<T>() where T : class, IComponent => neon.Components.Remove<T>(this);

        public static implicit operator UInt32(EntityID entity)
        {
            return entity.m_ID;
        }

        public static implicit operator EntityID(UInt32 value)
        {
            return new EntityID(value);
        }

        public override int GetHashCode()
        {
            return m_ID.GetHashCode();
        }
    }
}
