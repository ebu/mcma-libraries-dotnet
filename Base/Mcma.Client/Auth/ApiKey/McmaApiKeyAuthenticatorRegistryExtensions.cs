namespace Mcma.Client.Auth.ApiKey;

public static class McmaApiKeyAuthenticatorRegistryExtensions
{
    public static AuthenticatorRegistry AddMcmaApiKey(this AuthenticatorRegistry authRegistry, AuthenticatorKey key, string apiKey)
        => authRegistry.Add(key, _ => new McmaApiKeyAuthenticator(apiKey));
}