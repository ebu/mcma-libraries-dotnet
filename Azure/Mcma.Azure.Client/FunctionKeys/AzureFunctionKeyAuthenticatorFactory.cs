using System;
using System.Threading.Tasks;
using Mcma.Client;
using Microsoft.Extensions.Options;

namespace Mcma.Azure.Client.FunctionKeys
{
    public class AzureFunctionKeyAuthenticatorFactory : AuthenticatorFactory<AzureFunctionKeyAuthContext>, IDisposable
    {
        public AzureFunctionKeyAuthenticatorFactory(IOptionsMonitor<AzureFunctionKeyAuthenticatorOptions> optionsMonitor)
        {
            OptionsSubscription = optionsMonitor.OnChange(opts => Options = opts);
            Options = optionsMonitor.CurrentValue ?? new AzureFunctionKeyAuthenticatorOptions();
        }
        
        private IDisposable OptionsSubscription { get; }

        private AzureFunctionKeyAuthenticatorOptions Options { get; set; }

        protected override Task<IAuthenticator> GetAsync(AzureFunctionKeyAuthContext authContext)
        {
            return Task.FromResult<IAuthenticator>(new AzureFunctionKeyAuthenticator(authContext, Options.DecryptionKey));
        }

        public void Dispose()
        {
            OptionsSubscription?.Dispose();
        }
    }
}