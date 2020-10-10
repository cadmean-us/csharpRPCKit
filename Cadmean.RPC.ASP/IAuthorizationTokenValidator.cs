namespace Cadmean.RPC.ASP
{
    public interface IAuthorizationTokenValidator
    {
        public bool Validate(string token);
    }
}