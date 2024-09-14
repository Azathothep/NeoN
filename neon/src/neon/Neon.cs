namespace neon
{
    public class Neon
    {
        public static void Initialize()
        {
            // Entities

            EntityActiveStateNotifier activeStateNotifier = new EntityActiveStateNotifier();

            IEntityStorage entityStorage = new EntityStorage(activeStateNotifier);

            Entities.SetStorage(entityStorage);

            // Components

            ComponentStorageNotifier storageNotifier = new ComponentStorageNotifier();

            IComponentStorage componentStorage = new ComponentStorage(storageNotifier, activeStateNotifier);

            Components.SetStorage(componentStorage);

            // Queries

            IQueryStorage queryStorage = new QueryStorage(componentStorage.IteratorProvider, storageNotifier);

            QueryBuilder.SetStorage(queryStorage);
        }
    }
}
