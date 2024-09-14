using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private EntityActiveStateNotifier m_ActiveStateNotifier;

        private Random random = new Random();
        public EntityStorage(EntityActiveStateNotifier activeStateNotifier)
        {
            m_ActiveStateNotifier = activeStateNotifier;
        }

        private UInt32 RandomUInt32()
        {
            UInt32 thirtyBits = (UInt32)random.Next(1 << 30);
            UInt32 twoBits = (UInt32)(random.Next(1 << 2));
            return (thirtyBits << 2) | twoBits;
        }

        public EntityID GetID(bool isComponent = false)
        {
            UInt32 id = RandomUInt32();

            while (m_EntityIDs.Contains(id))
                id = RandomUInt32();

            //Debug.WriteLine($"New entity created with id {id}");

            return new EntityID(id, isComponent);
        }

        public void Destroy(EntityID entityID)
        {
            Components.RemoveAll(entityID);
         
            EntityID[] children = GetChildren(entityID); // logically, must not include any children components because they should have been destroyed just previously

            foreach (var c in children)
                Destroy(c);

            EntityID? parentID = GetParent(entityID);
            if (parentID != null)
                RemoveRelation(parentID, entityID);

            m_EntityIDs.Remove(entityID);
        }

        public void SetRelation(EntityID parentID, EntityID childID)
        {
            if (parentID == null || childID == null)
                throw new ArgumentNullException("Trying to Set Relation with a null parameter");

            if (!m_ParentEntities.TryGetValue(parentID, out HashSet<EntityID>? childSet))
            {
                childSet = new HashSet<EntityID>();
                m_ParentEntities.Add(parentID, childSet);
            }

            childSet.Add(childID);

            m_ChildEntities.Add(childID, parentID);

            childID.Refresh(EntityID.RefreshMode.ActiveState | EntityID.RefreshMode.Depth);
        }

        private void RemoveRelation(EntityID parentID, EntityID childID)
        {
            if (!m_ParentEntities.TryGetValue(parentID, out HashSet<EntityID>? childSet))
                childSet.Remove(childID);

            if (!m_ChildEntities.ContainsKey(childID))
                m_ChildEntities.Remove(childID);
        }

        public EntityID GetParent(EntityID entityID)
        {
            if (m_ChildEntities.TryGetValue(entityID, out EntityID? parent))
                return parent;

            return null;
        }

        public EntityID[] GetChildren(EntityID entityID, bool includeComponents = true)
        {
            if (m_ParentEntities.TryGetValue(entityID, out HashSet<EntityID>? children))
            {
                if (includeComponents)
                    return children.ToArray();

                HashSet<EntityID> childrenSet = new HashSet<EntityID>();
                foreach (var c in children)
                {
                    if (!c.isComponent)
                        childrenSet.Add(c);
                }

                return childrenSet.ToArray();
            }

            return new EntityID[0];
        }

        public void RefreshFamily(EntityID entityID)
        {
            OnEntityRefreshed(entityID);

			EntityID[] children = GetChildren(entityID);

            foreach (var child in children)
            {
                child.Refresh(EntityID.RefreshMode.ActiveState);
            }
        }

        private void OnEntityRefreshed(EntityID entityID)
        {
            m_ActiveStateNotifier.Raise(entityID, entityID.activeInHierarchy);
        }
    }
}
