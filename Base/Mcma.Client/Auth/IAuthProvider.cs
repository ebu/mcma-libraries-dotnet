namespace Mcma.Client.Auth;

public interface IAuthProvider
{
    IAuthenticator Get(string authType, string serviceName, string resourceType);
}