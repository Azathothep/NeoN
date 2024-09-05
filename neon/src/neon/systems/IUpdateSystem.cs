using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public interface IUpdateSystem
    {
        public void Update(TimeSpan timeSpan);
    }
}
