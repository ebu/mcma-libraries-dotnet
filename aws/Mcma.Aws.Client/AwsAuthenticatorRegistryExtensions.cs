using Mcma.Client;

namespace Mcma.Aws.Client
{
    public static class AwsAuthenticatorRegistryExtensions
    {
        public static AuthenticatorRegistry AddAws4Auth(this AuthenticatorRegistry authenticatorRegistry, Aws4AuthContext defaultContext = null)
            => authenticatorRegistry.Add<Aws4AuthContext, Aws4AuthenticatorFactory>(AwsConstants.Aws4, defaultContext);
    }
}