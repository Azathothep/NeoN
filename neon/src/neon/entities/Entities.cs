using System.Diagnostics;

namespace neon
{
    public static class Entities
    {
        private static IEntityStorage storage;

        public static void SetStorage(IEntityStorage storage)
        {
            Entities.storage = storage;
        }

        public static EntityID GetID(bool isComponent = false) => storage.GetID(isComponent);

        public static void Destroy(EntityID entityID) => storage.Destroy(entityID);

        public static void SetRelation(EntityID parentID, EntityID childID) => storage.SetRelation(parentID, childID);

        public static EntityID[] GetRoots() => storage.GetRoots();

        public static EntityID GetParent(EntityID entityID) => storage.GetParent(entityID);

        public static EntityID[] GetChildren(EntityID entityID, bool includeComponents = true) => storage.GetChildren(entityID, includeComponents);

        public static void UpdateState(EntityID entityID) => storage.UpdateState(entityID);
    }
}
