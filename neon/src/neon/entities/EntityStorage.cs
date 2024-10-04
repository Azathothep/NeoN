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

        private Dictionary<EntityID, HashSet<EntityID>> m_ParentToChildren = new();
        private Dictionary<EntityID, EntityID> m_ChildToParent = new();

        private Random random = new Random();

        private HookTrigger<EntityHook> m_HookTrigger;

        public EntityStorage() {
            m_HookTrigger = Hooks.Create<EntityHook>();
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

            EntityID newEntityID = new EntityID(id, isComponent);

            m_EntityIDs.Add(newEntityID);

            Debug.WriteLine($"New entity created with id {id}");

            return newEntityID;
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

            if (!m_ParentToChildren.TryGetValue(parentID, out HashSet<EntityID>? childSet))
            {
                childSet = new HashSet<EntityID>();
                m_ParentToChildren.Add(parentID, childSet);
            }

            childSet.Add(childID);

            m_ChildToParent.Add(childID, parentID);

            childID.Refresh(EntityID.RefreshMode.ActiveState | EntityID.RefreshMode.Depth);

            m_HookTrigger.Raise(EntityHook.OnNewChild, parentID);
            m_HookTrigger.Raise(EntityHook.OnNewParent, childID);
        }

        private void RemoveRelation(EntityID parentID, EntityID childID)
        {
            if (m_ParentToChildren.TryGetValue(parentID, out HashSet<EntityID>? childSet))
                childSet.Remove(childID);

            if (m_ChildToParent.ContainsKey(childID))
                m_ChildToParent.Remove(childID);

            m_HookTrigger.Raise(EntityHook.OnNewChild, parentID);
            m_HookTrigger.Raise(EntityHook.OnNewParent, childID);
        }

        public EntityID GetParent(EntityID entityID)
        {
            if (m_ChildToParent.TryGetValue(entityID, out EntityID? parent))
                return parent;

            return null;
        }

        public EntityID[] GetChildren(EntityID entityID, bool includeComponents = true)
        {
            if (m_ParentToChildren.TryGetValue(entityID, out HashSet<EntityID>? children))
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

        public void UpdateState(EntityID entityID)
        {
            m_HookTrigger.Raise(entityID.activeInHierarchy ? EntityHook.OnEnabled : EntityHook.OnDisabled, entityID);

			EntityID[] children = GetChildren(entityID);

            foreach (var child in children)
            {
                child.Refresh(EntityID.RefreshMode.ActiveState);
            }
        }

        public EntityID[] GetRoots()
        {
            return m_EntityIDs.Where((e) => !m_ChildToParent.ContainsKey(e)).ToArray();
        }
    }
}
