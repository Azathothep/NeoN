using System.Diagnostics;

namespace neon
{
    public class Entities
    {
        private class EntitiesStorage
        {
            public HashSet<EntityID> EntityIDs = new();

            public Dictionary<EntityID, HashSet<EntityID>> ParentEntities = new();
            public Dictionary<EntityID, EntityID> ChildEntities = new();
        }

        private static EntitiesStorage storage = new();

        private static Random random = new Random();

        private static UInt32 RandomUInt32()
        {
            UInt32 thirtyBits = (UInt32)random.Next(1 << 30);
            UInt32 twoBits = (UInt32)(random.Next(1 << 2));
            return (thirtyBits << 2) | twoBits;
        }

        private Entities() { }

        public static EntityID GetID()
        {
            UInt32 id = RandomUInt32();

            while (storage.EntityIDs.Contains(id))
                id = RandomUInt32();

            //Debug.WriteLine($"New entity created with id {id}");

            return new EntityID(id);
        }

        public static void Destroy(EntityID entityID)
        {
            HashSet<EntityID> children = GetChildren(entityID);

            foreach (var c in children)
                Destroy(c);

            EntityID? parentID = GetParent(entityID);
            if (parentID != null)
                RemoveRelation(parentID, entityID);

            Components.RemoveAll(entityID);
            storage.EntityIDs.Remove(entityID);
        }

        // Parent - Child system

        public static void SetRelation(EntityID parentID, EntityID childID)
        {
            if (parentID == null || childID == null)
                throw new ArgumentNullException("Trying to SetRelation with a null parameter");

            if (!storage.ParentEntities.TryGetValue(parentID, out HashSet<EntityID>? childSet))
            {
                childSet = new HashSet<EntityID>();
                storage.ParentEntities.Add(childID, childSet);
            }

            childSet.Add(childID);

            storage.ChildEntities.Add(childID, parentID);
        }

        public static void RemoveRelation(EntityID parentID, EntityID childID)
        {
            if (!storage.ParentEntities.TryGetValue(parentID, out HashSet<EntityID>? childSet))
                childSet.Remove(childID);

            if (!storage.ChildEntities.ContainsKey(childID))
                storage.ChildEntities.Remove(childID);
        }

        public static EntityID? GetParent(EntityID entityID)
        {
            if (storage.ChildEntities.TryGetValue(entityID, out EntityID? parent))
                return parent;

            return null;
        }

        public static HashSet<EntityID> GetChildren(EntityID entityID)
        {
            if (storage.ParentEntities.TryGetValue(entityID, out HashSet<EntityID>? children))
                return children;

            return new HashSet<EntityID>();
        }
    }
}
