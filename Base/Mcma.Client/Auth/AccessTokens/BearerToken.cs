using System;

namespace Mcma.Client.AccessTokens
{
    public class BearerToken
    {
        public string Token { get; set; }

        public DateTimeOffset? ExpiresOn { get; set; }
    }
}