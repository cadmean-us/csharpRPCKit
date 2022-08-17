using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cadmean.RPC
{
    /// <summary>
    /// Generic object returned by the RPC function. Contains error code, result if any and meta data.
    /// </summary>
    /// <typeparam name="TResult">Type of result</typeparam>
    public struct FunctionOutput<TResult>
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        
        [JsonProperty("result")]
        public TResult Result { get; set; }
        
        [JsonProperty("metaData")]
        public Dictionary<string, object> MetaData { get; set; }

        public static FunctionOutput WithError(string error)
        {
            return new FunctionOutput
            {
                Error = error,
                Result = null,
            };
        }
        
        public static FunctionOutput WithResult(TResult result)
        {
            return new FunctionOutput
            {
                Error = null,
                Result = result,
            };
        }
    }
    
    /// <summary>
    /// Non-generic object returned by the RPC function. Contains error code, result if any and meta data.
    /// </summary>
    public struct FunctionOutput
    {
        [JsonProperty("error")]
        public string Error { get; set; }
        
        [JsonProperty("result")]
        public object Result { get; set; }
        
        [JsonProperty("metaData")]
        public Dictionary<string, object> MetaData { get; set; }
        
        public static FunctionOutput WithError(string error)
        {
            return new FunctionOutput
            {
                Error = error,
                Result = null,
            };
        }
        
        public static FunctionOutput WithError(RpcErrorCode error)
        {
            return WithError(error.Description());
        }
        
        public static FunctionOutput WithResult(object result)
        {
            return new FunctionOutput
            {
                Error = null,
                Result = result,
            };
        }
    }
}