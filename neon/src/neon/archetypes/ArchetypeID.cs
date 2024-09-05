using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public class ArchetypeID
    {
        private int m_ID;
        public int ID => m_ID;

        public ArchetypeID(int id) {
            m_ID = id;
        }

        public override bool Equals(object obj)
        {
            return obj is ArchetypeID a && a.ID == this.ID;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 1430287;
                hashCode = hashCode * 7302013 ^ m_ID.GetHashCode();
                return hashCode;
            }
        }
    }
}
