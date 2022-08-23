using System;

namespace Mcma.Client.Auth.AccessTokens;

public class BearerToken
{
    public string Token { get; set; }

    public DateTimeOffset? ExpiresOn { get; set; }
}