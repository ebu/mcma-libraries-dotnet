namespace Mcma.Client.Auth;

public interface IAuthProvider
{
    void Add(AuthenticatorKey key, IAuthenticator authenticator);

    IAuthenticator Get(string authType, string serviceName, string resourceType);
}