using System;

namespace Cadmean.RPC.ASP
{
    public class DelegateAuthorizationTokenValidator : IAuthorizationTokenValidator
    {
        private readonly Func<string, bool> validator;

        public DelegateAuthorizationTokenValidator(Func<string, bool> validator)
        {
            this.validator = validator;
        }

        public bool Validate(string token)
        {
            return validator(token);
        }
    }
}