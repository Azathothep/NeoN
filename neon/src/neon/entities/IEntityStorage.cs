using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public interface IEntityStorage
    {
        public EntityID GetID(bool isComponent = false);
        public void Destroy(EntityID entityID);

        public void SetRelation(EntityID parentID, EntityID childID);

        public EntityID[] GetRoots();

        public EntityID GetParent(EntityID entityID);

        public EntityID[] GetChildren(EntityID entityID, bool includeComponents = true);

        public void UpdateState(EntityID entityID);
    }
}
