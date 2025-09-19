using Mcma.Client.Auth;

namespace Mcma.Client.Aws;

public class Aws4AuthenticatorKey : AuthenticatorKey
{
    public Aws4AuthenticatorKey(string serviceName = null, string resourceType = null)
        : base(AwsConstants.Aws4, serviceName, resourceType)
    {
    }
}
