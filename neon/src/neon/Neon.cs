namespace neon
{
    public class Neon
    {
        public static void Initialize()
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
        }
    }
}
