using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{ 
    public class ComponentSet
    {
        private List<ComponentID> m_ComponentIDs; // Change to HashSet ?
        public List<ComponentID> ComponentIDs => m_ComponentIDs;

        public ComponentSet(ComponentID type)
        {
            m_ComponentIDs= new List<ComponentID> { type };
        }

        public ComponentSet(List<ComponentID> types)
        {
            m_ComponentIDs = new List<ComponentID>(types);
            m_ComponentIDs.Sort();
        }

        public ComponentSet Add(ComponentID componentID)
        {
            if (m_ComponentIDs.Contains(componentID))
                return this;

            List<ComponentID> newList = new List<ComponentID>(m_ComponentIDs);
            newList.Add(componentID);
            return new ComponentSet(newList);
        }

        public ComponentSet Remove(ComponentID componentID)
        {
            if (!m_ComponentIDs.Contains(componentID))
            {
                throw new ArgumentException($"Cannot remove {componentID} from componentSet : not part of the set");
            }

            if (m_ComponentIDs.Count == 1)
                return null;

            List<ComponentID> newList = new List<ComponentID>(m_ComponentIDs);
            newList.Remove(componentID);
            return new ComponentSet(newList);
        }

        public override bool Equals(object obj)
        {
            return obj is ComponentSet other && m_ComponentIDs.SequenceEqual(other.ComponentIDs);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();

            for (int i = 0; i < m_ComponentIDs.Count; i++)
                hash.Add(m_ComponentIDs[i]);

            return hash.ToHashCode();
        }
    }
}
