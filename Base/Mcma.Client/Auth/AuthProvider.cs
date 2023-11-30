namespace Mcma.Client.Auth;

public class AuthProvider : IAuthProvider
{
    public AuthProvider(IEnumerable<AuthenticatorRegistration> authenticatorRegistrations)
    {
        AuthenticatorRegistrations = authenticatorRegistrations?.ToList() ?? [];
    }

    private List<AuthenticatorRegistration> AuthenticatorRegistrations { get; }

    //public AuthProvider Add<T>(IAuthenticator authenticator) where T : AuthenticatorKey, new()
    //    => Add(new T(), authenticator);

    //public AuthProvider Add(AuthenticatorKey key, IAuthenticator authenticator)
    //{
    //    AuthenticatorRegistrations.Add(new(key, authenticator));
    //    return this;
    //}

    public void Add(AuthenticatorKey key, IAuthenticator authenticator)
        => AuthenticatorRegistrations.Add(new(key, authenticator));

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

        return registration?.Authenticator ?? throw new McmaException($"No authenticators registered for auth type '{authType}'");
    }
}