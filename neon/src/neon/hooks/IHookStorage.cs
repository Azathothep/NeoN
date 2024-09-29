namespace neon
{
    public interface IHookStorage
    {
        public HookTrigger<HookID> Create<HookID>(Type additionalType = null) where HookID : struct, IConvertible;

        public HookTrigger<HookID> Create<HookID, T>() where HookID : struct, IConvertible;

        public void Add<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible;

        public void Add<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible;

        public void Add<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible;

        public void Add<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible;

        public void Remove<HookID>(HookID hook, Action action, object o) where HookID : struct, IConvertible;

        public void Remove<HookID>(HookID hook, Action<object> action) where HookID : struct, IConvertible;

        public void Remove<HookID, T>(HookID hook, Action action, object o) where HookID : struct, IConvertible;

        public void Remove<HookID, T>(HookID hook, Action<object> action) where HookID : struct, IConvertible;

    }
}
