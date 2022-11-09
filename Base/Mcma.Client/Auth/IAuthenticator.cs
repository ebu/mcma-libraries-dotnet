namespace Mcma.Client.Auth;

public interface IAuthenticator
{
    Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default);
}