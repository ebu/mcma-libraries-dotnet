using Mcma.Client.AccessTokens;
using Microsoft.Identity.Client;

namespace Mcma.Azure.Client.AzureAD
{
    public static class AuthenticationResultExtensions
    {
        public static BearerToken ToBearerToken(this AuthenticationResult authResult)
            => new BearerToken { Token = authResult.AccessToken, ExpiresOn = authResult.ExpiresOn };
    }
}