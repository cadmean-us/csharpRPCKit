using System;

namespace Cadmean.RPC
{
    public interface ICodecProvider
    {
        public byte[] Encode(object src);
        public object Decode(byte[] dada, Type outputType);

        public TOutput Decode<TOutput>(byte[] data)
        {
            return (TOutput) Decode(data, typeof(TOutput));
        }
        
        public string ContentType { get; }
    }
}