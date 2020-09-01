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
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(dada), outputType);
        }

        public string ContentType => "application/json";
    }
}