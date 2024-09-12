using System.Collections.Generic;

namespace neon
{
    public class ComponentID : IComparable<ComponentID>
    {
        private int m_ID;
        public int ID => m_ID;

        public ComponentID(int id)
        {
            m_ID = id;
        }

        public override bool Equals(object obj)
        {
            return obj is ComponentID c && c.ID == this.ID;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 1243421;
                hashCode = hashCode * 7302013 ^ m_ID.GetHashCode();
                return hashCode;
            }
        }

        public int CompareTo(ComponentID other)
        {
            return this.ID.CompareTo(other.ID);
        }
    }
}
