using Newtonsoft.Json;

namespace Cadmean.RPC
{
    public struct FunctionCall
    {
        [JsonProperty("args", DefaultValueHandling = DefaultValueHandling.Populate)]
        public object[] Arguments { get; set; }
        
        [JsonProperty("auth", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Authorization { get; set; }
    }
}