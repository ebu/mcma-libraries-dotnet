namespace Mcma.Client.Auth;

internal class AuthProvider : IAuthProvider
{
    public AuthProvider(IEnumerable<AuthenticatorRegistration> authenticatorRegistrations)
    {
        AuthenticatorRegistrations = authenticatorRegistrations?.ToArray() ?? Array.Empty<AuthenticatorRegistration>();
    }

    private AuthenticatorRegistration[] AuthenticatorRegistrations { get; }

    public IAuthenticator Get(string authType, string serviceName, string resourceType)
    {
        var exactMatchKey = new AuthenticatorKey(authType, serviceName, resourceType);
        var authTypeAndResourceTypeKey = new AuthenticatorKey(authType, "", resourceType);
        var authTypeAndServiceKey = new AuthenticatorKey(authType, serviceName);
        var authTypeOnlyKey = new AuthenticatorKey(authType);
        
        var registration =
            AuthenticatorRegistrations.FirstOrDefault(x => x.Key == exactMatchKey)
            ?? AuthenticatorRegistrations.FirstOrDefault(x => x.Key == authTypeAndResourceTypeKey)
            ?? AuthenticatorRegistrations.FirstOrDefault(x => x.Key == authTypeAndServiceKey)
            ?? AuthenticatorRegistrations.FirstOrDefault(x => x.Key == authTypeOnlyKey);
        
        if (registration == null)
            throw new McmaException($"No authenticators registered for auth type '{authType}'");
        
        return registration.Authenticator;
    }
}