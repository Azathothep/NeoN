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

        private int m_DisabledEndIndex;
        public int DisabledEndIndex => m_DisabledEndIndex;

        public int EntityCount => m_Columns[0].Count;

        public Archetype(ComponentSet componentSet)
        {
            m_ID = Archetypes.GetID();

            m_ComponentSet = componentSet;

            for (int i = 0; i < m_ComponentSet.ComponentIDs.Count; i++)
            {
                m_Columns.Add(new Column());
            }

            m_DisabledEndIndex = -1;
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

        public List<IComponent> RemoveEntity(int row)
        {
            List<IComponent> components = new List<IComponent>();
            int maxIndex = m_Columns[0].Count - 1;

            for (int i = 0; i < m_Columns.Count; i++)
            {
                components.Add(m_Columns[i][row]);
                m_Columns[i][row] = m_Columns[i][maxIndex];
                m_Columns[i].RemoveAt(maxIndex);
            }

            return components;
        }

        public int OnEntityActiveStateChanged(int row, bool newActiveState)
        {
            int newIndex = row;

            if (newActiveState && row <= m_DisabledEndIndex)
            {
                newIndex = m_DisabledEndIndex;
                m_DisabledEndIndex--;
            }
            else if (newActiveState == false && row > m_DisabledEndIndex)
            {
                newIndex = m_DisabledEndIndex + 1;
                m_DisabledEndIndex++;
            }

			if (row != newIndex)
				SwapEntities(row, newIndex);

            return newIndex;
        }

        private void SwapEntities(int row1, int row2)
        {
            for (int i = 0; i < m_Columns.Count; i++)
            {
                IComponent temp = m_Columns[i][row1];
                m_Columns[i][row1] = m_Columns[i][row2];
                m_Columns[i][row2] = temp;
            }
        }
    }
}
