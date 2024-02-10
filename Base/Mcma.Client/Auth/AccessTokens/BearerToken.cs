namespace Mcma.Client.Auth.AccessTokens;

public readonly struct BearerToken(string token, DateTimeOffset? expiresOn)
{
    public string Token { get; } = token;

    public DateTimeOffset? ExpiresOn { get; } = expiresOn;
}
