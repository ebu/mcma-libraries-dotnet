using System;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.AccessTokens;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Mcma.Azure.Client.AzureAd
{
    public class AzureADPublicClientBearerTokenProvider : IBearerTokenProvider<AzureADAuthContext>, IDisposable
    {
        public AzureADPublicClientBearerTokenProvider(IOptionsMonitor<AzureADPublicClientBearerTokenProviderOptions> optionsMonitor)
        {
            OptionsSubscription = optionsMonitor.OnChange(Configure);
            Configure(optionsMonitor.CurrentValue);
        }

        private IDisposable OptionsSubscription { get; }

        private IPublicClientApplication Client { get; set; }

        private string UserAccountId { get; set; }

        private void Configure(AzureADPublicClientBearerTokenProviderOptions options)
        {
            if (Client == null && options?.ClientOptions == null)
                throw new McmaException("No Azure AD public client options provided.");

            if (options?.ClientOptions != null)
                Client = PublicClientApplicationBuilder.CreateWithApplicationOptions(options.ClientOptions).Build();

            UserAccountId = options?.UserAccountId;
        }

        public async Task<BearerToken> GetAsync(AzureADAuthContext authContext, CancellationToken cancellationToken = default)
        {
            authContext.ValidateScope();

            AuthenticationResult authResult = null;

            if (!string.IsNullOrWhiteSpace(UserAccountId))
            {
                try
                {
                    var account = await Client.GetAccountAsync(UserAccountId);
                    if (account != null)
                        authResult = await Client.AcquireTokenSilent(new[] {authContext.Scope }, account).ExecuteAsync(cancellationToken);
                }
                catch (MsalUiRequiredException)
                {
                    // catch and move on, as this will then fall into the interactive call below
                }
            }

            if (authResult == null)
            {
                authResult = await Client.AcquireTokenInteractive(new[] {authContext.Scope }).ExecuteAsync(cancellationToken);

                UserAccountId = authResult.Account.HomeAccountId.Identifier;
            }

            return authResult.ToBearerToken();
        }

        public void Dispose()
        {
            OptionsSubscription?.Dispose();
        }
    }
}