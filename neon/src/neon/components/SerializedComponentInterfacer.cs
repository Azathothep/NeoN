using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public interface ISerializedComponentInterface
    {
        public IComponent? AddComponentByType(EntityID entityID, IComponent component, Type type);
    }
}
