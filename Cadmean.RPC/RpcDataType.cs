using System;
using System.Collections;

namespace Cadmean.RPC
{
    public static class RpcDataType
    {
        public const string String = "string";
        public const string Integer = "int";
        public const string Float = "float";
        public const string Boolean = "bool";
        public const string Date = "date";
        public const string List = "list";
        public const string Object = "object";
        public const string AuthTicket = "auth";
        public const string Null = "null";

        public static string ResolveRpcDataType(object o)
        {
            return o switch
            {
                null => Null,
                byte _ => Integer,
                short _ => Integer,
                int _ => Integer,
                long _ => Integer,
                sbyte _ => Integer,
                ushort _ => Integer,
                uint _ => Integer,
                ulong _ => Integer,
                float _ => Float,
                double _ => Float,
                decimal _ => Float,
                string _ => String,
                bool _ => Boolean,
                DateTime _ => Date,
                IEnumerable _ => List,
                JwtAuthorizationTicket _ => AuthTicket,
                _ => Object,
            };
        }
    }
}