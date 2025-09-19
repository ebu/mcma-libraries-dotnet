namespace Mcma.Client.Auth;

public record AuthenticatorRegistration
{
    public AuthenticatorRegistration(AuthenticatorKey key, IAuthenticator authenticator)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Authenticator = authenticator ?? throw new ArgumentNullException(nameof(authenticator));
    }
    
    public AuthenticatorKey Key { get; }
    
    public IAuthenticator Authenticator { get; }
}