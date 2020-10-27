using Microsoft.Identity.Client;

namespace Mcma.Azure.Client.AzureAd
{
    public class AzureADPublicClientBearerTokenProviderOptions
    {
        public PublicClientApplicationOptions ClientOptions { get; set; }
        
        public string UserAccountId { get; set; }
    }
}