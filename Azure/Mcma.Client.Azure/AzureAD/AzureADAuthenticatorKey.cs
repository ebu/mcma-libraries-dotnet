using Mcma.Client.Auth;

namespace Mcma.Client.Azure.AzureAD;

public class AzureADAuthenticatorKey : AuthenticatorKey
{
    public AzureADAuthenticatorKey(string serviceName = null, string resourceType = null)
        : base(AzureConstants.AzureAdAuthType, serviceName, resourceType)
    {
    }
}
