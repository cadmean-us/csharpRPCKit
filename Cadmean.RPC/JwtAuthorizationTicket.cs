namespace Cadmean.RPC
{
    public struct JwtAuthorizationTicket
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        

        public JwtAuthorizationTicket(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}