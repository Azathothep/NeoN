namespace neon
{
    public class Systems
    {
        private class SystemsStorage
        {
            public List<IUpdateSystem> UpdateSystems = new();
            public List<IDrawSystem> DrawSystems = new();
        }

        private static SystemsStorage storage = new();

        private Systems() { }

        public static void Add(IUpdateSystem updateSystem) => storage.UpdateSystems.Add(updateSystem);

        public static void Add(IDrawSystem drawSystem) => storage.DrawSystems.Add(drawSystem);

        public static void Update(TimeSpan timeSpan)
        {
            for (int i = 0; i < storage.UpdateSystems.Count; i++)
            {
                storage.UpdateSystems[i].Update(timeSpan);
            }
        }

        public static void Draw()
        {
            for (int i = 0; i < storage.DrawSystems.Count; i++)
            {
                storage.DrawSystems[i].Draw();
            }
        }
    }
}
