using System;
using System.Collections.Generic;
using System.Linq;

namespace neon
{
    public class EntityID
    {
        private UInt64 m_ID;

        public enum Flag
        {
            Enabled = 1
        }

        public bool enabled
        {
            get => (m_ID & (ulong)Flag.Enabled) == 1; // Checking if last bit of ID is 1 or 0
            set {
                if (value == true)
                {
                    m_ID = m_ID | (ulong)Flag.Enabled;
                } else
                {
                    m_ID = m_ID & ~(ulong)Flag.Enabled;
                }
            }
        }

        public EntityID(UInt32 value)
        {
            this.m_ID = (value << 32) + 1; // Shifting ID by 32 bits to the left, + 1 to set is as enabled
        }

        public T? Add<T>() where T : class, IComponent, new() => neon.Components.Add<T>(this);
        public T? Add<T>(T inputComponent) where T : class, IComponent => neon.Components.Add<T>(this, inputComponent);

        public T? Get<T>() where T : class, IComponent => neon.Components.Get<T>(this);

        public (T1?, T2?) Get<T1, T2>() where T1 : class, IComponent where T2 : class, IComponent => neon.Components.Get<T1, T2>(this);

        public (T1?, T2?, T3?) Get<T1, T2, T3>() where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent => neon.Components.Get<T1, T2, T3>(this);

        public (T1?, T2?, T3?, T4?) Get<T1, T2, T3, T4>() where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent => neon.Components.Get<T1, T2, T3, T4>(this);


        public void Remove<T>() where T : class, IComponent => neon.Components.Remove<T>(this);

        public EntityID? GetParent() => neon.Entities.GetParent(this);
        public HashSet<EntityID> GetChildren() => neon.Entities.GetChildren(this);

        public void SetParent(EntityID parentID) => neon.Entities.SetRelation(parentID, this);
        public void SetChild(EntityID childID) => neon.Entities.SetRelation(this, childID);

        public static implicit operator UInt32(EntityID entity)
        {
            return (UInt32)(entity.m_ID >> 32);
        }

        public static implicit operator EntityID(UInt32 value)
        {
            return new EntityID(value);
        }

        public override int GetHashCode()
        {
            return ((UInt32)(m_ID >> 32)).GetHashCode();
        }
    }
}
