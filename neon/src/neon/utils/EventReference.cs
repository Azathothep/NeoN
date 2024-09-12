namespace neon
{
    public class EventReference
    {
        public event Action Event;

        public void Raise() => Event?.Invoke();
    }

    public class EventReference<T>
    {
        public event Action<T> Event;

        public void Raise(T p1) => Event?.Invoke(p1);
    }

    public class EventReference<T1, T2>
    {
        public event Action<T1, T2> Event;

        public void Raise(T1 p1, T2 p2) => Event?.Invoke(p1, p2);
    }
}
