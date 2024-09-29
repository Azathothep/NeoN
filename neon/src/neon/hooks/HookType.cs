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
        private Type m_AdditionalType;

        public HookType()
        {
            m_AdditionalType = null;
        }

        public HookType(Type additionalType)
        {
            m_AdditionalType = additionalType;
        }

        public override bool Equals(object? obj)
        {
            return obj is HookType<HookID> hookType && m_AdditionalType == hookType.m_AdditionalType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 125371;
                hashCode = hashCode * 1190611 ^ typeof(HookID).GetHashCode();
                
                if (m_AdditionalType != null)
                    hashCode = hashCode * 1190611 ^ m_AdditionalType.GetHashCode();

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
