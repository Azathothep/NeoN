using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public class HookTrigger<HookID> where HookID : struct, IConvertible {
        private Action<HookID, object> m_Raiser;

        public HookTrigger(Action<HookID, object> raiser) => m_Raiser = raiser;

        public void Raise(HookID hook, object o) => m_Raiser(hook, o);
    }
}
