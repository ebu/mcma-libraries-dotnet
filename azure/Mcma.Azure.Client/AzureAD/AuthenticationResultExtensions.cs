using Mcma.Client.AccessTokens;
using Microsoft.Identity.Client;

namespace Mcma.Azure.Client.AzureAd
{
    public static class AuthenticationResultExtensions
    {
        public static BearerToken ToBearerToken(this AuthenticationResult authResult)
            => new BearerToken { Token = authResult.AccessToken, ExpiresOn = authResult.ExpiresOn };
    }
}