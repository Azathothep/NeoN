namespace neon
{
    public class HookMap<HookID> : IHookMap where HookID : struct, IConvertible
    {
        private Dictionary<HookID, (MainHook, Dictionary<object, Hook>)> m_Hooks = new();

        public void Trigger(HookID hook, object o)
        {
            if (!m_Hooks.TryGetValue(hook, out (MainHook, Dictionary<object, Hook>) hooks))
                return;

            (MainHook mainHook, Dictionary<object, Hook> objectHooks) = hooks;

            mainHook.Raise(o);

            if (!objectHooks.TryGetValue(o, out Hook objectHook))
                return;

            objectHook.Raise();
        }

        public void Add(HookID hook, Action action, object o)
        {
            if (!m_Hooks.TryGetValue(hook, out (MainHook, Dictionary<object, Hook>) hooks))
            {
                hooks = (new MainHook(), new Dictionary<object, Hook>());
                m_Hooks.Add(hook, hooks);
            }

            (MainHook _, Dictionary<object, Hook> objectHooks) = hooks;

            if (!objectHooks.TryGetValue(o, out Hook objectHook))
            {
                objectHook = new Hook();
                objectHooks.Add(o, objectHook);
            }

            objectHook.Event += action;
        }

        public void Add(HookID hook, Action<object> action)
        {
            if (!m_Hooks.TryGetValue(hook, out (MainHook, Dictionary<object, Hook>) hooks))
            {
                hooks = (new MainHook(), new Dictionary<object, Hook>());
                m_Hooks.Add(hook, hooks);
            }

            (MainHook mainHook, Dictionary<object, Hook> _) = hooks;

            mainHook.Event += action;
        }

        public void Remove(HookID hook, Action action, object o)
        {
            if (!m_Hooks.TryGetValue(hook, out (MainHook, Dictionary<object, Hook>) hooks))
                return;

            (MainHook _, Dictionary<object, Hook> objectHooks) = hooks;

            if (!objectHooks.TryGetValue(o, out Hook objectHook))
                return;

            objectHook.Event -= action;
        }

        public void Remove(HookID hook, Action<object> action)
        {
            if (!m_Hooks.TryGetValue(hook, out (MainHook, Dictionary<object, Hook>) hooks))
                return;

            (MainHook mainHook, Dictionary<object, Hook> _) = hooks;

            mainHook.Event -= action;
        }
    }
}
