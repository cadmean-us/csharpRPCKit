using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cadmean.RPC.ASP
{
    public class FunctionController : ControllerBase
    {
        protected FunctionCall Call;
        
        [HttpPost]
        public async Task<FunctionOutput> Post()
        {
            Call = await GetFunctionCall();
            
            var callMethod = GetCallMethod();
            if (!CallMethodIsValid(callMethod))
                return FunctionOutput.WithError(1);

            var args = GetArguments(callMethod);

            object result;
            try
            {
                dynamic task = (Task) callMethod.Invoke(this, args);
                await task;
                result = task.GetAwaiter().GetResult();
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

        private async Task<FunctionCall> GetFunctionCall()
        {
            using var r = new StreamReader(Request.Body);
            var str = await r.ReadToEndAsync();
            return (FunctionCall) JsonConvert.DeserializeObject(str, typeof(FunctionCall));
        }

        private MethodInfo GetCallMethod()
        {
            var t = GetType();
            return t.GetMethod("OnCall");
        }

        private object[] GetArguments(MethodInfo callMethod)
        {
            var parameters = callMethod.GetParameters();
            object[] args;

            if (Call.Arguments != null)
            {
                args= new object[parameters.Length];
                for (int i = 0; i < Math.Min(parameters.Length, Call.Arguments.Length); i++)
                {
                    var arg = Call.Arguments[i];
                    if (arg is JObject json)
                    {
                        args[i] = json.ToObject(parameters[i].ParameterType);
                    }
                    else
                    {
                        args[i] = arg;
                    }
                }
            }
            else
            {
                args = new object[0];
            }

            return args;
        }
    }
}