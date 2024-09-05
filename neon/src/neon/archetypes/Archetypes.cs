using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace neon
{

    public class Archetypes
    {
        public static Archetypes instance { get; private set; }

        private int m_LastID = -1;

        public Archetypes() {
            if (instance == null)
                instance = this;
            else
                throw new InvalidOperationException($"An object of type {this.GetType()} has already been created!");
        }

        public static ArchetypeID GetID()
        {
            if (instance == null)
                new Archetypes();

            return new ArchetypeID(++instance.m_LastID);
        }
    }
}
