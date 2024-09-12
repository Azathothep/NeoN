using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public class EntityStorage : IEntityStorage
    {
        private HashSet<EntityID> m_EntityIDs = new();

        private Dictionary<EntityID, HashSet<EntityID>> m_ParentEntities = new();
        private Dictionary<EntityID, EntityID> m_ChildEntities = new();

        private Random random = new Random();

        private UInt32 RandomUInt32()
        {
            UInt32 thirtyBits = (UInt32)random.Next(1 << 30);
            UInt32 twoBits = (UInt32)(random.Next(1 << 2));
            return (thirtyBits << 2) | twoBits;
        }

        public EntityID GetID()
        {
            UInt32 id = RandomUInt32();

            while (m_EntityIDs.Contains(id))
                id = RandomUInt32();

            //Debug.WriteLine($"New entity created with id {id}");

            return new EntityID(id);
        }

        public void Destroy(EntityID entityID)
        {
            HashSet<EntityID> children = GetChildren(entityID);

            foreach (var c in children)
                Destroy(c);

            EntityID? parentID = GetParent(entityID);
            if (parentID != null)
                RemoveRelation(parentID, entityID);

            Components.RemoveAll(entityID);
            m_EntityIDs.Remove(entityID);
        }

        public void SetRelation(EntityID parentID, EntityID childID)
        {
            if (parentID == null || childID == null)
                throw new ArgumentNullException("Trying to SetRelation with a null parameter");

            if (!m_ParentEntities.TryGetValue(parentID, out HashSet<EntityID>? childSet))
            {
                childSet = new HashSet<EntityID>();
                m_ParentEntities.Add(childID, childSet);
            }

            childSet.Add(childID);

            m_ChildEntities.Add(childID, parentID);
        }

        private void RemoveRelation(EntityID parentID, EntityID childID)
        {
            if (!m_ParentEntities.TryGetValue(parentID, out HashSet<EntityID>? childSet))
                childSet.Remove(childID);

            if (!m_ChildEntities.ContainsKey(childID))
                m_ChildEntities.Remove(childID);
        }

        public EntityID? GetParent(EntityID entityID)
        {
            if (m_ChildEntities.TryGetValue(entityID, out EntityID? parent))
                return parent;

            return null;
        }

        public HashSet<EntityID> GetChildren(EntityID entityID)
        {
            if (m_ParentEntities.TryGetValue(entityID, out HashSet<EntityID>? children))
                return children;

            return new HashSet<EntityID>();
        }
    }
}
