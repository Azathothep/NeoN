using System;
using System.Collections.Generic;
using System.Linq;

namespace neon
{
    public class EntityID
    {
        private string value;

        public EntityID(string value)
        {
            this.value = value;
        }

        public static implicit operator string(EntityID entity)
        {
            return entity.value;
        }

        public static implicit operator EntityID(string value)
        {
            return new EntityID(value);
        }
    }
}
