using System;
using System.Collections.Generic;
using System.Data.Common;

namespace neon
{
    using Column = List<IComponent>;

    public struct ArchetypeEdges
    {
        public Archetype Add;
        public Archetype Remove;
    }

    public class Archetype
    {
        private ArchetypeID m_ID;
        public ArchetypeID ID => m_ID;

        private ComponentSet m_ComponentSet;
        public ComponentSet ComponentSet => m_ComponentSet;

        private List<Column> m_Columns = new();
        public List<Column> Columns => m_Columns;

        private Dictionary<ComponentID, ArchetypeEdges> m_Edges = new();
        public Dictionary<ComponentID, ArchetypeEdges> Edges => m_Edges;

        public Archetype(ComponentSet componentSet)
        {
            m_ID = Archetypes.GetID();

            m_ComponentSet = componentSet;

            for (int i = 0; i < m_ComponentSet.ComponentIDs.Count; i++)
            {
                m_Columns.Add(new Column());
            }
        }

        public int AddEntity(List<IComponent> components)
        {
            if (components.Count != m_Columns.Count)
            {
                // Something weird is happening
            }

            for (int i = 0; i < m_Columns.Count; i++)
            {
                m_Columns[i].Add(components[i]);
            }

            return m_Columns[0].Count - 1;
        }

        // MUST UPDATE OTHER COMPONENT'S ROWS !!!

        public List<IComponent> RemoveEntity(int row)
        {
            List<IComponent> components = new List<IComponent>();

            for (int i = 0; i < m_Columns.Count; i++)
            {
                components.Add(m_Columns[i][row]);
                m_Columns[i].RemoveAt(row);
            }

            return components;
        }
    }
}
