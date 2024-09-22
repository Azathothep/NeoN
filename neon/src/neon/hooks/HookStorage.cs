using System.Diagnostics;
using System.Net;

namespace neon
{
    public class HookStorage : IHookStorage
    {
        private Dictionary<IHookType, IHookMap> m_Hooks = new();

        public HookStorage() { }

        public HookTrigger<HookID> Create<HookID>() where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (m_Hooks.ContainsKey(type))
                return null;

            IHookMap map = new HookMap<HookID>();
            m_Hooks.Add(type, map);

            HookTrigger<HookID> trigger = new HookTrigger<HookID>((h, o) => Trigger(h, o));

            return trigger;
        }

        public HookTrigger<HookID> Create<HookID, T>() where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (m_Hooks.ContainsKey(type))
                return null;

            IHookMap map = new HookMap<HookID>();
            m_Hooks.Add(type, map);

            HookTrigger<HookID> trigger = new HookTrigger<HookID>((h, o) => Trigger<HookID, T>(h, o));

            return trigger;
        }

        private void Trigger<HookID>(HookID hook, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Trigger(hook, o);
        }

        private void Trigger<HookID, T>(HookID hook, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
                return;

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Trigger(hook, o);
        }

        public void Add<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                Debug.WriteLine($"Trying to hook to {typeof(HookID)} but it hasn't been created yet");
                return;
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action, o);
        }

        public void Add<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                Debug.WriteLine($"Trying to hook to {typeof(HookID)} but it hasn't been created yet");
                return;
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action);
        }

        public void Add<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                Debug.WriteLine($"Trying to hook to {typeof(HookID)}, {typeof(T)} but it hasn't been created yet");
                return;
            }

            HookMap<HookID> hookMap = (HookMap<HookID>)map;

            hookMap.Add(hook, action, o);
        }

        public void Add<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible
        {
            IHookType type = new HookType<HookID, T>();

            if (!m_Hooks.TryGetValue(type, out IHookMap map))
            {
                Debug.WriteLine($"Trying to hook to {typeof(HookID)}, {typeof(T)} but it hasn't been created yet");
                return;
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
