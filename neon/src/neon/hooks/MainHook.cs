namespace neon
{
    public class MainHook
    {
        public event Action<object> Event;

        public void Raise(object o) => Event?.Invoke(o);
    }
}
