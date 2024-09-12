using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public interface IQuery
    {
        public bool IncludeInactive { get; }

        public IQueryFilter[] Filters { get; }

        public ComponentID[] ReturnValues { get; }
    }
}
