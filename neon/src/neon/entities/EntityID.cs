using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace neon
{
    public class EntityID
    {
        private UInt64 m_ID;

        public enum Flag
        {
            Active = 0,
            ActiveParent = 1,
            Component = 2
        }

        public bool active
        {
            get => GetFlag(Flag.Active); // Checking if last bit of ID is 1 or 0
            set {
                if (!active && value == true)
                {
                    SetFlag(Flag.Active, true);
                    Entities.RefreshActiveState(this);
                } else if (active && value == false)
                {
                    SetFlag(Flag.Active, false);
                    Entities.RefreshActiveState(this);
                }
            }
        }

		public bool activeParent
		{
			get => GetFlag(Flag.ActiveParent);
			private set
			{
				if (!activeParent && value == true)
				{
					SetFlag(Flag.ActiveParent, true);
					Entities.RefreshActiveState(this);
				}
				else if (activeParent && value == false)
				{
					SetFlag(Flag.ActiveParent, false);
					Entities.RefreshActiveState(this);
				}
			}
		}

        public bool activeInHierarchy => GetFlag(Flag.ActiveParent) & GetFlag(Flag.Active);

        public bool isComponent => GetFlag(Flag.Component);

        public void RefreshActiveParent()
        {
            EntityID parent = this.GetParent();

            this.activeParent = parent != null ? parent.activeInHierarchy : true;
        }

        public EntityID(UInt32 value, bool isComponent = false)
        {
            this.m_ID = ((ulong)value << 32);

            SetFlag(Flag.Active, true);

            RefreshActiveParent();

            SetFlag(Flag.Component, isComponent);
        }

        private void SetFlag(Flag flag, bool state)
        {
            if (state)
            {
                ulong mask = ((ulong)1 << (int)flag);
                m_ID = m_ID | mask;
            } else
            {
                ulong mask = ~((ulong)1 << (int)flag);
                m_ID = m_ID & mask;
            }
        }

        public bool GetFlag(Flag flag)
        {
            ulong mask = ((ulong)1 << (int)flag);
            ulong result = (m_ID & mask);
            return result != 0;
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
