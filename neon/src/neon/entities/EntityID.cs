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
            Component = 4,
            Flag4 = 8,
            Flag5 = 16,
            Flag6 = 32,
            Flag7 = 64,
            Flag8 = 128
        }

        public enum RefreshMode
        {
            ActiveState = 1,
            Depth = 2
        }

        public bool active
        {
            get => GetFlag(Flag.Active);
            set {
                if (active == value)
                    return;

                SetFlag(Flag.Active, value);
                Entities.UpdateState(this);
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
                Entities.UpdateState(this);
            }
        }

        public bool activeInHierarchy => GetFlag(Flag.Active | Flag.ActiveParent);

        public bool isComponent => GetFlag(Flag.Component);

        public int depth {
            get => (int)((m_ID >> 8) & 63);
            private set {
                ulong shiftedValue = (ulong)value << 8;
                m_ID = m_ID & ~((ulong)63 << 8);
                m_ID = m_ID | shiftedValue;
            }
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

        public void Refresh(RefreshMode mode)
        {
            EntityID parent = this.GetParent();

            if (Has(mode, RefreshMode.ActiveState))
                this.activeParent = parent != null ? parent.activeInHierarchy : true;

            if (Has(mode, RefreshMode.Depth))
                this.depth = CalculateDepth();
        }

        private bool Has(RefreshMode mode, RefreshMode flag)
        {
            return (mode & RefreshMode.ActiveState) == RefreshMode.ActiveState;
        }

        private int CalculateDepth()
        {
            int depth = 0;

            EntityID parent = Entities.GetParent(this);

            while (parent != null)
            {
                depth++;
                parent = Entities.GetParent(parent);
            }

            return depth;
        }

        public T? Add<T>() where T : class, IComponent, new() => neon.Components.Add<T>(this);

        public T? Add<T>(T inputComponent) where T : class, IComponent => neon.Components.Add<T>(this, inputComponent);

        public T? Get<T>() where T : class, IComponent => neon.Components.Get<T>(this);

		public IComponent[] GetAll() => neon.Components.GetAll(this);

        public bool TryGet<T>(out T? component) where T : class, IComponent => neon.Components.TryGet(this, out component);

        public (T1?, T2?) Get<T1, T2>() where T1 : class, IComponent where T2 : class, IComponent => neon.Components.Get<T1, T2>(this);

        public (T1?, T2?, T3?) Get<T1, T2, T3>() where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent => neon.Components.Get<T1, T2, T3>(this);

        public (T1?, T2?, T3?, T4?) Get<T1, T2, T3, T4>() where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent => neon.Components.Get<T1, T2, T3, T4>(this);

        public void Remove<T>() where T : class, IComponent => Components.Remove<T>(this);

        public EntityID GetParent() => Entities.GetParent(this);

        public IComponent GetComponentOfEntityID(EntityID entityID) => GetAll().First(c => c.EntityID == entityID);

        public EntityID[] GetChildren(bool includeComponents = true) => Entities.GetChildren(this, includeComponents);

        public T[] GetInChildren<T>(bool propagate = false) where T : class, IComponent => Components.GetInChildren<T> (this, propagate);

        public T[] GetInParents<T>() where T : class, IComponent => Components.GetInParents<T>(this);

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
