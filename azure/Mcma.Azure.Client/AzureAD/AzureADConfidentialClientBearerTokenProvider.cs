using System;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.AccessTokens;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Mcma.Azure.Client.AzureAd
{
    public class AzureADConfidentialClientBearerTokenProvider : IBearerTokenProvider<AzureADAuthContext>, IDisposable
    {
        public AzureADConfidentialClientBearerTokenProvider(IOptionsMonitor<ConfidentialClientApplicationOptions> optionsMonitor)
        {
            OptionsSubscription = optionsMonitor.OnChange(Configure);
            Configure(optionsMonitor.CurrentValue);
        }

        private IDisposable OptionsSubscription { get; }

        private IConfidentialClientApplication Client { get; set; }

        private void Configure(ConfidentialClientApplicationOptions options)
        {
            if (Client == null && options == null)
                throw new McmaException("No Azure AD public client options provided.");

            if (options != null)
                Client = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(options).Build();
        }

        public async Task<BearerToken> GetAsync(AzureADAuthContext authContext, CancellationToken cancellationToken = default)
        {
            authContext.ValidateScope();

            var authResult = await Client.AcquireTokenForClient(new[] {authContext.Scope }).ExecuteAsync(cancellationToken);
            
            return authResult.ToBearerToken();
        }

        public void Dispose()
        {
            OptionsSubscription?.Dispose();
        }
    }
}