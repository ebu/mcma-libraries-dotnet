using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Utility;

namespace Mcma.Client.Aws;

public class Aws4Signer
{
    private const string CngSuffix = "Cng";
    private const string CryptoServiceProviderSuffix = "CryptoServiceProvider";
    private const string ManagedSuffix = "Managed";
        
    public Aws4Signer(string accessKey, string secretKey, string region, string sessionToken = null, string service = AwsConstants.Services.ExecuteApi)
    {
        AccessKey = accessKey ?? throw new ArgumentNullException(nameof(accessKey));
        SecretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
        Region = region;
        SessionToken = sessionToken;
        Service = service;
    }

    private string AccessKey { get; }

    private string SecretKey { get; }

    private string Region { get; }

    private string SessionToken { get; }

    private string Service { get; }

    public async Task<HttpRequestMessage> SignAsync(HttpRequestMessage request, HashAlgorithm hashAlgorithm = null, CancellationToken cancellationToken = default)
    {
        hashAlgorithm ??= new SHA256Managed();

        var algorithmName = hashAlgorithm.GetType().Name;
        if (algorithmName.EndsWith(CngSuffix))
            algorithmName = algorithmName.Substring(0, algorithmName.Length - CngSuffix.Length);
        else if (algorithmName.EndsWith(CryptoServiceProviderSuffix))
            algorithmName = algorithmName.Substring(0, algorithmName.Length - CryptoServiceProviderSuffix.Length);
        else if (algorithmName.EndsWith(ManagedSuffix))
            algorithmName = algorithmName.Substring(0, algorithmName.Length - ManagedSuffix.Length);
            
        var awsDate = new AwsDate();

        // set the host and date headers
        request.Headers.Host = request.RequestUri.Host;
        request.Headers.Add(AwsConstants.Headers.Date, awsDate.DateTimeString);

        var hashedBody = await request.HashBodyAsync(hashAlgorithm);
        request.Headers.Add(AwsConstants.Headers.ContentHash(algorithmName), hashedBody);

        // build the string to sign from the canonical request
        var stringToSign = StringToSign(awsDate, request.ToHashedCanonicalRequest(hashedBody));

        // get the signing key using the date on the request
        var signingKey = SigningKey(awsDate);

        // build the signature by signing the string to sign with the signing key
        var signature = signingKey.UseToSign(stringToSign).HexEncode();

        // set the auth headers
        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                AwsConstants.Signing.Algorithm,
                $"Credential={AccessKey}/{CredentialScope(awsDate)}, SignedHeaders={request.SignedHeaders()}, Signature={signature}");
            
        // add the session token, if any, to the headers on the request
        if (!string.IsNullOrWhiteSpace(SessionToken))
            request.Headers.Add(AwsConstants.Headers.SecurityToken, SessionToken);

        return request;
    }

    private string StringToSign(AwsDate awsDate, string hashedRequest) 
        =>
            AwsConstants.Signing.Algorithm + "\n" +
            awsDate.DateTimeString + "\n" +
            CredentialScope(awsDate) + "\n" +
            hashedRequest;

    private byte[] SigningKey(AwsDate awsDate)
        =>
            Encoding.UTF8.GetBytes(AwsConstants.Signing.SecretKeyPrefix + SecretKey)
                    .UseToSign(awsDate.DateString)
                    .UseToSign(Region)
                    .UseToSign(Service)
                    .UseToSign(AwsConstants.Signing.ScopeTerminator);

    private string CredentialScope(AwsDate awsDate)
        => $"{awsDate.DateString}/{Region}/{Service}/{AwsConstants.Signing.ScopeTerminator}";
}