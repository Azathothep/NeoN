using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace neon
{
    public class Entities
    {
        public static Entities instance;

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private HashSet<EntityID> m_EntityIDs;

        public Entities()
        {
            if (instance == null)
            {
                instance = this;
            }

            m_EntityIDs = new HashSet<EntityID>();
        }

        public static EntityID GetID()
        {
            string id = RandomString(8);

            while (instance.m_EntityIDs.Contains(id))
                id = RandomString(8);

            //Debug.WriteLine($"New entity created with id {id}");

            return new EntityID(id);
        }

        public void Destroy(EntityID id)
        {
            m_EntityIDs.Remove(id);
        }
    }
}
