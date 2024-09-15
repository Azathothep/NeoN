namespace neon
{
    public interface IHookStorage
    {
        public void Trigger<HookID>(HookID hook, object o = null) where HookID : struct, IConvertible;

        public void Trigger<HookID, T>(HookID hook, object o = null) where HookID : struct, IConvertible;

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
