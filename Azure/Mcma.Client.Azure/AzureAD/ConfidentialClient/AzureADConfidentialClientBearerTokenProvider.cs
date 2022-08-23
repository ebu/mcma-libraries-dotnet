using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Mcma.Client.Azure.AzureAD.ConfidentialClient;

public class AzureADConfidentialClientBearerTokenProvider : IBearerTokenProvider<AzureADAuthContext>
{
    public AzureADConfidentialClientBearerTokenProvider(IOptions<ConfidentialClientApplicationOptions> options)
    {
        if (options.Value == null)
            throw new McmaException("No Azure AD public client options provided.");
            
        Client = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(options.Value).Build();
    }
    private IConfidentialClientApplication Client { get; }

    public async Task<BearerToken> GetAsync(AzureADAuthContext authContext, CancellationToken cancellationToken = default)
    {
        authContext.ValidateScope();

        var authResult = await Client.AcquireTokenForClient(new[] {authContext.Scope }).ExecuteAsync(cancellationToken);
            
        return authResult.ToBearerToken();
    }
}