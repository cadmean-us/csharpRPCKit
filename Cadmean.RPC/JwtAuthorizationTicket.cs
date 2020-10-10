using System;

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

        public bool Equals(JwtAuthorizationTicket other)
        {
            return AccessToken == other.AccessToken && RefreshToken == other.RefreshToken;
        }

        public override bool Equals(object obj)
        {
            return obj is JwtAuthorizationTicket other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AccessToken, RefreshToken);
        }

        public override string ToString()
        {
            return $"Access: {AccessToken}\nRefresh: {RefreshToken}";
        }
    }
}