namespace neon
{
    public class Hook
    {
        public event Action Event;

        public void Raise() => Event?.Invoke();
    }
}
