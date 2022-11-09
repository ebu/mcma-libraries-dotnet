using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.Auth;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Azure.FunctionKeys;

public class AzureFunctionKeyAuthenticator : IAuthenticator
{
    public AzureFunctionKeyAuthenticator(AuthenticatorKey key, IOptionsSnapshot<AzureFunctionKeyOptions> optionsSnapshot)
    {
        var options = optionsSnapshot.Get(key.ToString());

        FunctionKey = options.FunctionKey;
    }

    private string FunctionKey { get; }

    public Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        request.Headers.Add(AzureConstants.FunctionKeyHeader, FunctionKey);
            
        return Task.CompletedTask;
    }
}