using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public interface IComponentIteratorProvider
    {
        public IComponentIterator Get<T>(IQuery query, QueryType queryType) where T : class, IComponent;
        public IComponentIterator Get<T1, T2>(IQuery query, QueryType queryType) where T1 : class, IComponent where T2 : class, IComponent;
        public IComponentIterator Get<T1, T2, T3>(IQuery query, QueryType queryType) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent;
        public IComponentIterator Get<T1, T2, T3, T4>(IQuery query, QueryType queryType) where T1 : class, IComponent where T2 : class, IComponent where T3 : class, IComponent where T4 : class, IComponent;
    }
}
