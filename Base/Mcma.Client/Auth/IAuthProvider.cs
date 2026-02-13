namespace Mcma.Client.Auth;

public interface IAuthProvider
{
    string Name { get; }

    IAuthenticator Get(string authType, string serviceName, string resourceType);
}