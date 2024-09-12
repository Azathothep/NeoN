namespace neon
{
    public class Neon
    {
        public static void Initialize()
        {
            // Entities

            IEntityStorage entityStorage = new EntityStorage();

            Entities.SetStorage(entityStorage);

            // Components

            ComponentStorageNotifier notifier = new ComponentStorageNotifier();

            IComponentStorage componentStorage = new ComponentStorage(notifier);

            Components.SetStorage(componentStorage);

            // Queries

            IQueryStorage queryStorage = new QueryStorage(componentStorage.IteratorProvider, notifier);

            QueryBuilder.SetStorage(queryStorage);
        }
    }
}
