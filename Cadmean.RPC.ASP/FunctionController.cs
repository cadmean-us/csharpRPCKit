using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cadmean.RPC.ASP;

public class FunctionController : ControllerBase, IDisposable
{
    protected FunctionCall Call { get; private set; }
    protected RpcService RpcService { get; private set; } = null!;
    protected string FunctionName { get; private set; } = "";

    private CachedFunctionInfo functionInfo = null!;

    private Exception? internalException;

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
                args[i] = ConvertArgumentToParameterType(arg, parameter.ParameterType);
            }
        }
        else
        {
            args = new object[0];
        }

        return args;
    }

    private static object ConvertArgumentToParameterType(object arg, Type parameterType)
    {
        return arg switch
        {
            JObject json => json.ToObject(parameterType),
            JArray array => ConvertArrayArgumentToListParameterType(array, parameterType),
            _ => ConvertArgumentToSimpleParameterType(arg, parameterType)
        };
    }
        
    private static object ConvertArgumentToSimpleParameterType(object arg, Type parameterType)
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

    private static object ConvertArrayArgumentToListParameterType(JArray arr, Type parameterType)
    {
        var elementTypes = parameterType.GetGenericArguments();
        if (elementTypes.Length == 0)
        {
            return null;
        }

        var elementType = elementTypes[0];
            
        var listType = typeof(IList<>);
        listType = listType.MakeGenericType(elementType);
        if (!listType.IsAssignableFrom(parameterType))
        {
            return null;
        }
            
        var list = Activator.CreateInstance(parameterType);
        if (list == null)
        {
            return null;
        }
            
        var add = parameterType.GetMethod("Add")!;

        foreach (var e in arr)
        {
            add.Invoke(list, new []{ ConvertArgumentToParameterType(e, elementType) });
        }

        return list;
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
                RpcService.InvokeExceptionHandler(ex.InnerException ?? ex);
                return FunctionOutput.WithError(RpcErrorCode.InternalServerError);
            default:
                internalException = ex;
                RpcService.InvokeExceptionHandler(ex);
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
            output.MetaData["dotnetResultType"] = output.Result?.GetType().FullName;
            output.MetaData["dotnetException"] = internalException?.ToString();
        }
            
        return output;
    }

    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}