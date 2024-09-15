using System.Net;

namespace neon
{
    public class HookStorage : IHookStorage
    {
        private Dictionary<IHookType, IHookMap> m_Hooks = new();

        public HookStorage() { }

        public void Trigger<HookID>(HookID hook, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Trigger(hook, o);
        }

        public void Trigger<HookID, T>(HookID hook, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                map = new HookMap<HookID>();
                m_Hooks.Add(type, map);
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Trigger(hook, o);
        }

        public void Add<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                map = new HookMap<HookID>();
                m_Hooks.Add(type, map);
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action, o);
        }

        public void Add<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                map = new HookMap<HookID>();
                m_Hooks.Add(type, map);
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action);
        }

        public void Add<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                map = new HookMap<HookID>();
                m_Hooks.Add(type, map);
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action, o);
        }

        public void Add<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                map = new HookMap<HookID>();
                m_Hooks.Add(type, map);
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action);
        }

        public void Remove<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Remove(hook, action, o);
        }

        public void Remove<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Remove(hook, action);
        }

        public void Remove<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Remove(hook, action, o);
        }

        public void Remove<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Remove(hook, action);
        }
    }
}
