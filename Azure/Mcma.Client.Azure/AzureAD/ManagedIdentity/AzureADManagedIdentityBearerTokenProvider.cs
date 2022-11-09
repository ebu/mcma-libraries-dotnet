using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.Auth;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Azure.AzureAD.ManagedIdentity;

public class AzureADManagedIdentityBearerTokenProvider : IBearerTokenProvider
{
    public AzureADManagedIdentityBearerTokenProvider(
        AuthenticatorKey key,
        IOptionsSnapshot<AzureADManagedIdentityOptions> managedIdentityOptionsSnapshot)
    {
        var managedIdentityOptions = managedIdentityOptionsSnapshot.Get(key.ToString());
        
        AzureServiceTokenProvider =
            managedIdentityOptions.AzureAdInstance != null
                ? new AzureServiceTokenProvider(managedIdentityOptions.ConnectionString, managedIdentityOptions.AzureAdInstance)
                : new AzureServiceTokenProvider(managedIdentityOptions.ConnectionString);
    }

    private AzureServiceTokenProvider AzureServiceTokenProvider { get; }

    public async Task<BearerToken> GetAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var resource = $"{request.RequestUri}://{request.RequestUri}.default";
        
        var authResult = await AzureServiceTokenProvider.GetAuthenticationResultAsync(resource, cancellationToken: cancellationToken);
        
        return new BearerToken
        {
            Token = authResult.AccessToken,
            ExpiresOn = authResult.ExpiresOn
        };
    }
}