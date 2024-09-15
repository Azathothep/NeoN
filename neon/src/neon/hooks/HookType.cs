using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neon
{
    public class HookType<HookID> : IHookType where HookID : struct, IConvertible
    {
        public override bool Equals(object? obj)
        {
            return obj is HookType<HookID>;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 129631;
                hashCode = hashCode * 1196089 ^ typeof(HookID).GetHashCode();

                return hashCode;
            }
        }
    }

    public class HookType<HookID, T> : IHookType where HookID : struct, IConvertible
    {
        public override bool Equals(object? obj)
        {
            return obj is HookType<HookID, T>;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 125371;
                hashCode = hashCode * 1190611 ^ typeof(HookID).GetHashCode();
                hashCode = hashCode * 1190611 ^ typeof(T).GetHashCode();

                return hashCode;
            }
        }
    }
}
