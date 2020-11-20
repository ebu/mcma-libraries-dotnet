﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Mcma.Client;

namespace Mcma.GoogleCloud.Client
{
    public class GoogleAuthenticator : IAuthenticator
    {
        public GoogleAuthenticator(GoogleAuthenticatorOptions options, GoogleAuthContext authContext)
        {
            Options = options;
            AuthContext = authContext;
        }

        private GoogleAuthenticatorOptions Options { get; }

        private GoogleAuthContext AuthContext { get; }
        
        private OidcToken OidcToken { get; set; }

        private SemaphoreSlim OidcTokenSemaphoreSlim { get; } = new SemaphoreSlim(1, 1);

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

                OidcToken = await googleCredential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(AuthContext.Audience), cancellationToken);

                return OidcToken;
            }
            finally
            {
                OidcTokenSemaphoreSlim.Release();
            }
        }
        
        public async Task AuthenticateAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            var oidcToken = await GetOidcTokenAsync(cancellationToken);
            
            var accessToken = await oidcToken.GetAccessTokenAsync(cancellationToken);
            
            request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {accessToken}");
        }
    }
}