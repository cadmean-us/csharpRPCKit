namespace Cadmean.RPC
{
    public struct RpcConfiguration
    {
        public ITransportProvider Transport;
        public ICodecProvider Codec;
        public IFunctionUrlProvider FunctionUrlProvider;
        public IAuthorizationTicketHolder AuthorizationTicketHolder;

        internal static RpcConfiguration Default = new()
        {
            Transport = new HttpTransportProvider(),
            Codec = new JsonCodecProvider(),
            FunctionUrlProvider = new DefaultFunctionUrlProvider(),
            AuthorizationTicketHolder = new TransientAuthorizationTicketHolder(),
        };
    }
}