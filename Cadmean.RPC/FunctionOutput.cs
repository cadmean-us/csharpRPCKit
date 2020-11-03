using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cadmean.RPC
{
    public struct FunctionOutput<TResult>
    {
        [JsonProperty("error")]
        public int Error { get; set; }
        
        [JsonProperty("result")]
        public TResult Result { get; set; }
        
        [JsonProperty("metaData")]
        public Dictionary<string, object> MetaData { get; set; }

        public static FunctionOutput WithError(int error)
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
                Error = 0,
                Result = result,
            };
        }
    }
    
    public struct FunctionOutput
    {
        [JsonProperty("error")]
        public int Error { get; set; }
        
        [JsonProperty("result")]
        public object Result { get; set; }
        
        [JsonProperty("metaData")]
        public Dictionary<string, object> MetaData { get; set; }
        
        public static FunctionOutput WithError(int error)
        {
            return new FunctionOutput
            {
                Error = error,
                Result = null,
            };
        }
        
        public static FunctionOutput WithError(RpcErrorCode error)
        {
            return WithError((int) error);
        }
        
        public static FunctionOutput WithResult(object result)
        {
            return new FunctionOutput
            {
                Error = 0,
                Result = result,
            };
        }
    }
}