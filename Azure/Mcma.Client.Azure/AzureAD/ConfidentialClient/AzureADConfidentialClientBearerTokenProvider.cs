using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.Auth;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Mcma.Client.Azure.AzureAD.ConfidentialClient;

public class AzureADConfidentialClientBearerTokenProvider : IBearerTokenProvider
{
    public AzureADConfidentialClientBearerTokenProvider(
        AuthenticatorKey key,
        IOptionsSnapshot<AzureADConfidentialClientApplicationOptions> optionsSnapshot)
    {
        var options = optionsSnapshot.Get(key.ToString()); 
        
        Client = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(options).Build();
        Scopes = options.Scopes;
    }
    
    private IConfidentialClientApplication Client { get; }
    
    private string[] Scopes { get; }

    public async Task<BearerToken> GetAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var authResult = await Client.AcquireTokenForClient(Scopes).ExecuteAsync(cancellationToken);
            
        return new BearerToken
        {
            Token = authResult.AccessToken,
            ExpiresOn = authResult.ExpiresOn
        };
    }
}