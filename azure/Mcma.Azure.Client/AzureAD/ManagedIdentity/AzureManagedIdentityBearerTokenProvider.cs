using System;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.AccessTokens;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;

namespace Mcma.Azure.Client.AzureAD.ManagedIdentity
{
    public class AzureManagedIdentityBearerTokenProvider : IBearerTokenProvider<AzureADAuthContext>, IDisposable
    {
        public AzureManagedIdentityBearerTokenProvider(IOptionsMonitor<AzureManagedIdentityBearerTokenProviderOptions> optionsMonitor)
        {
            OptionsSubscription = optionsMonitor.OnChange(Configure);
            Configure(optionsMonitor.CurrentValue);
        }

        private IDisposable OptionsSubscription { get; }

        private AzureServiceTokenProvider AzureServiceTokenProvider { get; set; }

        private void Configure(AzureManagedIdentityBearerTokenProviderOptions options)
        {
            AzureServiceTokenProvider =
                options.AzureAdInstance != null
                    ? new AzureServiceTokenProvider(options.ConnectionString, options.AzureAdInstance)
                    : new AzureServiceTokenProvider(options.ConnectionString);
        }

        public async Task<BearerToken> GetAsync(AzureADAuthContext authContext, CancellationToken cancellationToken = default)
        {
            authContext.ValidateScope();

            var scopeAsUrl = new Uri(authContext.Scope);

            var resource = $"{scopeAsUrl.Scheme}://{scopeAsUrl.Host}";

            var authResult = await AzureServiceTokenProvider.GetAuthenticationResultAsync(resource, cancellationToken: cancellationToken);

            return new BearerToken
            {
                Token = authResult.AccessToken,
                ExpiresOn = authResult.ExpiresOn
            };
        }

        public void Dispose()
        {
            OptionsSubscription?.Dispose();
        }
    }
}