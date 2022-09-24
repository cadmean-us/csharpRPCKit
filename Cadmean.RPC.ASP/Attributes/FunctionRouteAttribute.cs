using System;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.ASP.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FunctionRouteAttribute : RouteAttribute
    {
        public readonly string FunctionName;
        public readonly string FullPath;

        public FunctionRouteAttribute(string functionName) : base($"/api/rpc/{functionName}")
        {
            FunctionName = functionName;
            FullPath = $"/api/rpc/{functionName}";
        }
    }
}