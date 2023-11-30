using Microsoft.Identity.Client;

namespace Mcma.Client.Azure.AzureAD.ConfidentialClient;

public class AzureADConfidentialClientApplicationOptions : ConfidentialClientApplicationOptions
{
    public string[] Scopes { get; set; } = [];
}