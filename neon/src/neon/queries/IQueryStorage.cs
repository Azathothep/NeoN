using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public interface IQueryStorage
    {
        public IQueryResult Get(IQuery query, QueryType queryType, Func<IComponentIteratorProvider, IQueryResult> queryResultCreator);
    }
}
