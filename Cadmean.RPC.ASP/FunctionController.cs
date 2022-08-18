using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Cadmean.RPC.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cadmean.RPC.ASP
{
    public class FunctionController : ControllerBase
    {
        protected FunctionCall Call { get; private set; }
        protected RpcService RpcService { get; private set; }
        protected string FunctionName { get; private set; }

        private CachedFunctionInfo functionInfo;

        private Exception internalException;

        [HttpPost]
        public async Task<FunctionOutput> Post()
        {
            Prepare();
            
            Call = await GetFunctionCall();

            var callMethod = functionInfo.CallMethod;

            if (functionInfo.RequiresAuthorization && !ValidateAuthorizationToken())
                return FunctionOutput.WithError(RpcErrorCode.AuthorizationError);
            
            var args = GetArguments(callMethod);

            var output = await TryCallFunction(callMethod, args);
            return ProcessOutput(output);
        }

        [HttpGet]
        public FunctionOutput Get()
        {
            return FunctionOutput.WithError(RpcErrorCode.PreCallChecksFailed);
        }

        private void Prepare()
        {
            RpcService = HttpContext.Items["rpcService"] as RpcService ?? 
                         throw new RpcServerException("RPC service not found");
            functionInfo = HttpContext.Items["functionInfo"] as CachedFunctionInfo ??
                           throw new RpcServerException("No cached function info");
            FunctionName = functionInfo.Name;
        }

        private async Task<FunctionCall> GetFunctionCall()
        {
            using var r = new StreamReader(Request.Body);
            var str = await r.ReadToEndAsync();
            var serializerSettings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.None,
            };
            // ReSharper disable once PossibleNullReferenceException
            return (FunctionCall) JsonConvert.DeserializeObject(str, typeof(FunctionCall), serializerSettings);
        }

        private bool ValidateAuthorizationToken()
        {
            if (string.IsNullOrWhiteSpace(Call.Authorization))
                return false;

            return RpcService.Configuration.AuthorizationTokenValidator.Validate(Call.Authorization);
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

        private static object TryConvertArgumentToParameterType(object arg, Type parameterType)
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

            if (parameterType == typeof(DateTime) && arg is string s)
            {
                return DateTime.Parse(s);
            }
            
            return arg;
        }
        
        
        private async Task<FunctionOutput> TryCallFunction(MethodInfo callMethod, object[] args)
        {
            object result;
            try
            {
                if (functionInfo.IsCallMethodAsync)
                    result = await ExecuteFunctionAsync(callMethod, args);
                else
                    result = ExecuteFunctionSync(callMethod, args);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
            
            return FunctionOutput.WithResult(result);
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

        private FunctionOutput HandleException(Exception ex)
        {
            switch (ex)
            {
                case ArgumentException:
                    internalException = ex;
                    return FunctionOutput.WithError(RpcErrorCode.InvalidArguments);
                case TargetParameterCountException:
                    return FunctionOutput.WithError(RpcErrorCode.InvalidArguments);
                case FunctionException fEx:
                    return FunctionOutput.WithError(fEx.Code);
                case TargetInvocationException when ex.InnerException is FunctionException fEx2:
                    return FunctionOutput.WithError(fEx2.Code);
                case TargetInvocationException:
                    internalException = ex.InnerException;
                    return FunctionOutput.WithError(RpcErrorCode.InternalServerError);
                default:
                    internalException = ex;
                    return FunctionOutput.WithError(RpcErrorCode.InternalServerError);
            }
        }

        private FunctionOutput ProcessOutput(FunctionOutput output)
        {
            if (!IsRationalToIncludeMetaData(output))
                return output;

            return IncludeMetaData(output);
        }

        private bool IsRationalToIncludeMetaData(FunctionOutput output)
        {
            return RpcService.Configuration.AlwaysIncludeMetadata || 
                   RpcService.Configuration.DebugMode || 
                   output.Result is JwtAuthorizationTicket;
        }
        
        private FunctionOutput IncludeMetaData(FunctionOutput output)
        {
            output.MetaData = new Dictionary<string, object>();

            output.MetaData["resultType"] = RpcDataType.ResolveRpcDataType(output.Result);

            if (RpcService.Configuration.DebugMode)
            {
                output.MetaData["clrResultType"] = output.Result?.GetType().FullName;
                output.MetaData["internalException"] = internalException?.GetType().FullName;
                output.MetaData["exceptionStackTrace"] = internalException?.StackTrace;
            }
            
            return output;
        }
    }
}