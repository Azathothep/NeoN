using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public interface IQueryFilter
    {
        public FilterTerm Term { get; }

        public ComponentID ComponentID { get; }
    }
}
