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
        private static Archetypes storage;

        private int m_LastID = -1;

        public Archetypes() {
            if (storage == null)
                storage = this;
            else
                throw new InvalidOperationException($"An object of type {this.GetType()} has already been created!");
        }

        public static ArchetypeID GetID()
        {
            if (storage == null)
                new Archetypes();

            return new ArchetypeID(++storage.m_LastID);
        }
    }
}
