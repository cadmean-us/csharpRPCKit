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
        [HttpPost]
        public async Task<FunctionOutput> Post()
        {
            using var r = new StreamReader(Request.Body);
            var str = await r.ReadToEndAsync();
            Console.WriteLine(str);
            var call = (FunctionCall) JsonConvert.DeserializeObject(str, typeof(FunctionCall));
            var t = GetType();
            var callMethod = t.GetMethod("OnCall");
            if (!CallMethodIsValid(callMethod))
                return FunctionOutput.WithError(1);
            var parameters = callMethod.GetParameters();
            object[] args;

            if (call.Arguments != null)
            {
                args= new object[parameters.Length];
                for (int i = 0; i < Math.Min(parameters.Length, call.Arguments.Length); i++)
                {
                    var arg = call.Arguments[i];
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
    }
}