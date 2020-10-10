using System;
using System.Collections.Generic;
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
        protected RpcService RpcService;
        protected string FunctionName;

        [HttpPost]
        public async Task<FunctionOutput> Post()
        {
            Prepare();
            
            Call = await GetFunctionCall();
            
            var callMethod = GetCallMethod();
            if (!CallMethodIsValid(callMethod))
                return FunctionOutput.WithError(RpcErrorCode.FunctionNotCallable);

            var args = GetArguments(callMethod);

            object result;
            try
            {
                if (IsMethodAsync(callMethod))
                    result = await ExecuteFunctionAsync(callMethod, args);
                else
                    result = ExecuteFunctionSync(callMethod, args);
            }
            catch (FunctionException ex)
            {
                return FunctionOutput.WithError(ex.Code);
            }
            catch 
            {
                return FunctionOutput.WithError(RpcErrorCode.InternalServerError);
            }
            
            var output = FunctionOutput.WithResult(result);
            return ProcessOutput(output);
        }

        private void Prepare()
        {
            RpcService = HttpContext.Items["rpcService"] as RpcService ?? 
                         throw new RpcServerException("RPC service not found");
            FunctionName = HttpContext.Items["functionName"] as string;
        }

        private bool CallMethodIsValid(MethodInfo methodInfo)
        {
            return methodInfo != null && !methodInfo.IsAbstract;
        }

        private async Task<FunctionCall> GetFunctionCall()
        {
            using var r = new StreamReader(Request.Body);
            var str = await r.ReadToEndAsync();
            // ReSharper disable once PossibleNullReferenceException
            return (FunctionCall) JsonConvert.DeserializeObject(str, typeof(FunctionCall));
        }

        private MethodInfo GetCallMethod()
        {
            var cached = RpcService.GetCachedFunction(FunctionName);

            if (cached != null)
                return cached;
            
            var t = GetType();
            var methodInfo = t.GetMethod("OnCall", 
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            RpcService.CacheFunction(FunctionName, methodInfo);
            return methodInfo;
        }

        private object[] GetArguments(MethodInfo callMethod)
        {
            var parameters = callMethod.GetParameters();
            object[] args;

            if (Call.Arguments != null)
            {
                args = new object[parameters.Length];
                for (int i = 0; i < Math.Min(parameters.Length, Call.Arguments.Length); i++)
                {
                    var arg = Call.Arguments[i];
                    var parameter = parameters[i];
                    switch (arg)
                    {
                        case JObject json:
                            args[i] = json.ToObject(parameter.ParameterType);
                            break;
                        default:
                            args[i] = TryConvertArgumentToParameterType(arg, parameter.ParameterType);
                            break;
                    }
                }
            }
            else
            {
                args = new object[0];
            }

            return args;
        }

        private object TryConvertArgumentToParameterType(object arg, Type parameterType)
        {
            if (parameterType == typeof(int))
            {
                return Convert.ToInt32(arg);
            }
            if (parameterType == typeof(long))
            {
                return Convert.ToInt64(arg);
            }
            if (parameterType == typeof(float))
            {
                return Convert.ToSingle(arg);
            }
            if (parameterType == typeof(double))
            {
                return Convert.ToDouble(arg);
            }
            
            return arg;
        }
        
        private bool IsMethodAsync(MethodInfo callMethod)
        {
            return callMethod.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null;
            // var returnType = callMethod.ReturnType;
            // if (returnType.IsGenericType)
            // {
            //     return returnType.IsAssignableFrom(typeof(Task));
            // }
            // else
            // {
            //     return returnType.IsAssignableFrom(typeof(Task));
            // }
        }
        
        private async Task<object> ExecuteFunctionAsync(MethodInfo callMethod, object[] args)
        {
            dynamic task = (Task) callMethod.Invoke(this, args);
            await task;
            return task.GetAwaiter().GetResult();
        }

        private object ExecuteFunctionSync(MethodInfo callMethod, object[] args)
        {
            return callMethod.Invoke(this, args);
        }

        private FunctionOutput ProcessOutput(FunctionOutput output)
        {
            if (!IsRationalToIncludeMetaData(output))
                return output;

            return IncludeMetaData(output);
        }

        private bool IsRationalToIncludeMetaData(FunctionOutput output)
        {
            return RpcService.Configuration.AlwaysIncludeMetadata || output.Result is JwtAuthorizationTicket;
        }
        
        private FunctionOutput IncludeMetaData(FunctionOutput output)
        {
            output.MetaData = new Dictionary<string, object>();
            output.MetaData["clrResultType"] = output.Result.GetType().FullName;
            return output;
        }
    }
}