using System;
using System.Collections.Generic;
using System.Linq;

namespace neon
{
    public class EntityID
    {
        private UInt32 value;

        public EntityID(UInt32 value)
        {
            this.value = value;
        }

        public static implicit operator UInt32(EntityID entity)
        {
            return entity.value;
        }

        public static implicit operator EntityID(UInt32 value)
        {
            return new EntityID(value);
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
