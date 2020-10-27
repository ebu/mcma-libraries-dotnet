namespace Mcma.Azure.Client.AzureAd
{
    public class AzureManagedIdentityBearerTokenProviderOptions
    {
        public string ConnectionString { get; set; }
        
        public string AzureAdInstance { get; set; }
    }
}