using System.Reflection;

namespace Cadmean.RPC.ASP
{
    internal class CachedFunctionInfo
    {
        internal string Name;
        internal string Path;
        internal bool RequiresAuthorization;
        internal MethodInfo CallMethod;
        internal bool IsCallMethodAsync;

        public override string ToString()
        {
            return $"{Name} at {Path} {(RequiresAuthorization ? "requires authorization" : "")}";
        }
    }
}