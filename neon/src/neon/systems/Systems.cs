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

        public static void Add(IGameSystem gameSystem)
        {
            if (gameSystem is IUpdateSystem)
                storage.UpdateSystems.Add((IUpdateSystem)gameSystem);
            else if (gameSystem is IDrawSystem)
                storage.DrawSystems.Add((IDrawSystem)gameSystem);
        }

        public static void Remove(IGameSystem gameSystem)
        {
            if (gameSystem is IUpdateSystem)
                storage.UpdateSystems.Remove((IUpdateSystem)gameSystem);
            else if (gameSystem is IDrawSystem)
                storage.DrawSystems.Remove((IDrawSystem)gameSystem);
        }

        public static IGameSystem[] GetLoadedSystems()
        {
            IGameSystem[] gameSystems = new IGameSystem[storage.UpdateSystems.Count + storage.DrawSystems.Count];

            for (int i = 0; i < storage.UpdateSystems.Count; i++)
                gameSystems[i] = storage.UpdateSystems[i];

            for (int i = 0; i < storage.DrawSystems.Count; i++)
                gameSystems[storage.UpdateSystems.Count + i] = storage.DrawSystems[i];

            return gameSystems;
        }

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
