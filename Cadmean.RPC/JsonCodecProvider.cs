using System;
using System.Text;
using Newtonsoft.Json;

namespace Cadmean.RPC
{
    public class JsonCodecProvider : ICodecProvider
    {
        public byte[] Encode(object src)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(src));
        }

        public object Decode(byte[] dada, Type outputType)
        {
            var jsonStr = Encoding.UTF8.GetString(dada);
            var obj = Activator.CreateInstance(outputType);
            JsonConvert.PopulateObject(jsonStr, obj, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return obj;
        }

        public string ContentType => "application/json";
    }
}