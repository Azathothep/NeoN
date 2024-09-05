using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace neon
{
    public class Entities
    {
        public static Entities storage { get; private set; }

        private static Random random = new Random();

        private HashSet<EntityID> m_EntityIDs;

        private static UInt32 RandomUInt32()
        {
            UInt32 thirtyBits = (UInt32)random.Next(1 << 30);
            UInt32 twoBits = (UInt32)(random.Next(1 << 2));
            return (thirtyBits << 2) | twoBits;
        }

        public Entities()
        {
            if (storage == null)
            {
                storage = this;
            } else
                throw new InvalidOperationException($"An object of type {this.GetType()} has already been created!");

            m_EntityIDs = new HashSet<EntityID>();
        }

        public static EntityID GetID()
        {
            UInt32 id = RandomUInt32();

            while (storage.m_EntityIDs.Contains(id))
                id = RandomUInt32();

            //Debug.WriteLine($"New entity created with id {id}");

            return new EntityID(id);
        }

        public void Destroy(EntityID id)
        {
            m_EntityIDs.Remove(id);
        }
    }
}
