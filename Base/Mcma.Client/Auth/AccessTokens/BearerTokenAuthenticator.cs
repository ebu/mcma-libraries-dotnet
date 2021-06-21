using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Mcma.Client.Auth.AccessTokens
{
    internal class BearerTokenAuthenticator<TAuthContext> : IAuthenticator
    {
        public BearerTokenAuthenticator(IBearerTokenProvider<TAuthContext> bearerTokenProvider, TAuthContext authContext)
        {
            BearerTokenProvider = bearerTokenProvider ?? throw new ArgumentNullException(nameof(bearerTokenProvider));
            AuthContext = authContext;
        }

        private IBearerTokenProvider<TAuthContext> BearerTokenProvider { get; }

        private TAuthContext AuthContext { get; }

        private BearerToken BearerToken { get; set; }

        public async Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            if (BearerToken?.ExpiresOn != null && BearerToken.ExpiresOn >= DateTimeOffset.UtcNow)
                BearerToken = null;

            if (BearerToken == null)
                BearerToken = await BearerTokenProvider.GetAsync(AuthContext, cancellationToken);

            if (BearerToken != null)
                request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {BearerToken.Token}");
        }
    }
}