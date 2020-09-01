using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cadmean.RPC.ASP
{
    public class FunctionController : ControllerBase
    {
        [HttpPost]
        public async Task<FunctionOutput> Post([FromBody] FunctionCall call)
        {
            var t = GetType();
            var callMethod = t.GetMethod("OnCall");
            if (CallMethodIsValid(callMethod))
                return FunctionOutput.WithError(1);
            var parameters = callMethod.GetParameters();
            var args = new object[parameters.Length];
            for (int i = 0; i < Math.Min(parameters.Length, call.Arguments.Length); i++)
            {
                args[i] = call.Arguments[i];
            }

            object result;
            try
            {
                result = await (Task<object>) callMethod.Invoke(this, args);
            }
            catch (FunctionException ex)
            {
                return FunctionOutput.WithError(ex.Code);
            }
            catch
            {
                return FunctionOutput.WithError(3);
            }
            
            return FunctionOutput.WithResult(result);
        }

        private bool CallMethodIsValid(MethodInfo methodInfo)
        {
            return methodInfo != null && !methodInfo.IsAbstract;
        }
    }
}