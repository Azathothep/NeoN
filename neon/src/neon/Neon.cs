namespace neon
{
    public class Neon
    {
        public struct Architecture
        {
            public IHookStorage HookStorage;
            public IEntityStorage EntityStorage;
            public IComponentStorage ComponentStorage;
            public IQueryStorage QueryStorage;
        }

        public static Architecture Initialize()
        {
            // Hooks

            IHookStorage hookStorage = new HookStorage();

            Hooks.SetStorage(hookStorage);

            // Entities

            IEntityStorage entityStorage = new EntityStorage();

            Entities.SetStorage(entityStorage);

            // Components

            ComponentStorageNotifier storageNotifier = new ComponentStorageNotifier();

            IComponentStorage componentStorage = new ComponentStorage(storageNotifier);

            Components.SetStorage(componentStorage);

            // Queries

            IQueryStorage queryStorage = new QueryStorage(componentStorage.IteratorProvider, storageNotifier);

            QueryBuilder.SetStorage(queryStorage);

            // Architecture

            Architecture architecture = new Architecture()
            {
                HookStorage = hookStorage,
                EntityStorage = entityStorage,
                ComponentStorage = componentStorage,
                QueryStorage = queryStorage
            };

            return architecture;
        }
    }
}
