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
            Active = 1,
            ActiveParent = 2,
            Component = 4
        }

        public bool active
        {
            get => GetFlag(Flag.Active);
            set {
                if (active == value)
                    return;

                SetFlag(Flag.Active, value);
                Entities.RefreshFamily(this);
            }
        }

		public bool activeParent
		{
			get => GetFlag(Flag.ActiveParent);
			private set
			{
                if (activeParent == value)
                    return;

				SetFlag(Flag.ActiveParent, value);
				Entities.RefreshFamily(this);
			}
		}

        public bool activeInHierarchy => GetFlag(Flag.Active | Flag.ActiveParent);

        public bool isComponent => GetFlag(Flag.Component);

        public void Refresh()
        {
            EntityID parent = this.GetParent();

            this.activeParent = parent != null ? parent.activeInHierarchy : true;
        }

        public EntityID(UInt32 value, bool isComponent = false)
        {
            this.m_ID = ((ulong)value) << 32;

            SetFlag(Flag.Active | Flag.ActiveParent, true);

            SetFlag(Flag.Component, isComponent);
        }

        private void SetFlag(Flag flag, bool state)
        {
            if (state)
                m_ID = m_ID | (ulong)flag;
            else
                m_ID = m_ID & ~(ulong)flag;
        }

        public bool GetFlag(Flag flag)
        {
            ulong result = m_ID & (ulong)flag;
            return result == (ulong)flag;
        }

        public T? Add<T>() where T : class, IComponent, new() => neon.Components.Add<T>(this);

        public T? Add<T>(T inputComponent) where T : class, IComponent => neon.Components.Add<T>(this, inputComponent);

        public T? Get<T>() where T : class, IComponent => neon.Components.Get<T>(this);

        public (T1?, T2?) Get<T1, T2>() where T1 : class, IComponent where T2 : class, IComponent => neon.Components.Get<T1, T2>(this);

        public (T1?, T2?, T3?) Get<T1, T2, T3>() where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent => neon.Components.Get<T1, T2, T3>(this);

        public (T1?, T2?, T3?, T4?) Get<T1, T2, T3, T4>() where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent => neon.Components.Get<T1, T2, T3, T4>(this);

        public void Remove<T>() where T : class, IComponent => Components.Remove<T>(this);

        public EntityID GetParent() => Entities.GetParent(this);

        public HashSet<EntityID> GetChildren() => Entities.GetChildren(this);

        public void SetParent(EntityID parentID) => Entities.SetRelation(parentID, this);

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
