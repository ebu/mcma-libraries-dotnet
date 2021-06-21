using System;
using System.Threading.Tasks;

namespace Mcma.Client.Auth.AccessTokens
{
    internal class BearerTokenAuthenticatorFactory<TAuthContext> : AuthenticatorFactory<TAuthContext>
    {
        public BearerTokenAuthenticatorFactory(IBearerTokenProvider<TAuthContext> bearerTokenProvider)
        {
            BearerTokenProvider = bearerTokenProvider ?? throw new ArgumentNullException(nameof(bearerTokenProvider));
        }

        private IBearerTokenProvider<TAuthContext> BearerTokenProvider { get; }

        protected override Task<IAuthenticator> GetAsync(TAuthContext authContext)
            => Task.FromResult<IAuthenticator>(new BearerTokenAuthenticator<TAuthContext>(BearerTokenProvider, authContext));
    }
}