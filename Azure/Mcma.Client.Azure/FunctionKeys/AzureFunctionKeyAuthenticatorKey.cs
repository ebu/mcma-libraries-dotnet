using Mcma.Client.Auth;

namespace Mcma.Client.Azure.FunctionKeys;

public class AzureFunctionKeyAuthenticatorKey : AuthenticatorKey
{
    public AzureFunctionKeyAuthenticatorKey(string serviceName = null, string resourceType = null)
        : base(AzureConstants.FunctionKeyAuthType, serviceName, resourceType)
    {
    }
}
