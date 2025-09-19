using System;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Mcma.Client.Auth;
using Mcma.Client.Auth.AccessTokens;
using Microsoft.Extensions.Options;

using BearerToken = Mcma.Client.Auth.AccessTokens.BearerToken;

namespace Mcma.Client.Google;

public class GoogleBearerTokenProvider : IBearerTokenProvider
{
    public GoogleBearerTokenProvider(AuthenticatorKey key, IOptionsSnapshot<GoogleAuthenticatorOptions> optionsSnapshot)
    {
        Options = optionsSnapshot.Get(key.ToString());
    }

    private GoogleAuthenticatorOptions Options { get; }

    private OidcToken OidcToken { get; set; }

    private SemaphoreSlim OidcTokenSemaphoreSlim { get; } = new(1, 1);

    private async Task<OidcToken> GetOidcTokenAsync(CancellationToken cancellationToken)
    {
        if (OidcToken != null)
            return OidcToken;
                
        await OidcTokenSemaphoreSlim.WaitAsync(cancellationToken);
        try
        {
            if (OidcToken != null)
                return OidcToken;
                
            var googleCredential = Options.KeyFile != null
                                       ? await GoogleCredential.FromFileAsync(Options.KeyFile, cancellationToken)
                                       : await GoogleCredential.GetApplicationDefaultAsync(cancellationToken);

            OidcToken = await googleCredential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(Options.Audience), cancellationToken);

            return OidcToken;
        }
        finally
        {
            OidcTokenSemaphoreSlim.Release();
        }
    }

    private static DateTimeOffset? GetExpiresOn(OidcToken oidcToken)
    {
        try
        {
            var tokenRespProp = typeof(OidcToken).GetProperty(nameof(TokenResponse), BindingFlags.Instance | BindingFlags.NonPublic);
            if (tokenRespProp != null && tokenRespProp.GetValue(oidcToken) is TokenResponse tokenResponse)
                return tokenResponse.IssuedUtc.AddSeconds(Convert.ToDouble(tokenResponse.ExpiresInSeconds));
        }
        catch
        {
            // trying to access internal API, so something might break - just exit
        }
        return default;
    }
        
    public async Task<BearerToken> GetAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var oidcToken = await GetOidcTokenAsync(cancellationToken);
            
        var accessToken = await oidcToken.GetAccessTokenAsync(cancellationToken);

        return new BearerToken
        {
            Token = accessToken,
            ExpiresOn = GetExpiresOn(oidcToken)
        };
    }
}