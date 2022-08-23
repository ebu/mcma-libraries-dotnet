using Mcma.Client.Auth.AccessTokens;
using Microsoft.Identity.Client;

namespace Mcma.Client.Azure.AzureAD;

public static class AuthenticationResultExtensions
{
    public static BearerToken ToBearerToken(this AuthenticationResult authResult) => new() { Token = authResult.AccessToken, ExpiresOn = authResult.ExpiresOn };
}