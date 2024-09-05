namespace neon
{
    public class Entities
    {
        private class EntitiesStorage
        {
            public HashSet<EntityID> EntityIDs = new();
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
            Components.RemoveAll(entityID);
            storage.EntityIDs.Remove(entityID);
        }
    }
}
