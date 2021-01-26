using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Mcma.Aws.Client
{
    public static class AwsRequestExtensions
    {
        public static string CanonicalUri(this HttpRequestMessage request)
            => string.Join("/", request.RequestUri.AbsolutePath.Split('/').Select(x => Uri.EscapeDataString(Uri.EscapeDataString(x))));

        public static string CanonicalQueryParameters(this HttpRequestMessage request)
            =>
                !string.IsNullOrWhiteSpace(request.RequestUri.Query)
                    ? string.Join("&",
                        request.RequestUri.Query
                            // skip the ? at the start of the query string, if present
                            .TrimStart('?')
                            // break into parts on ampersands
                            .Split('&')
                            // break parts into keys and values
                            .Select(x => x.Split('='))
                            // escape the values
                            .Select(
                                x =>
                                    x.Length == 2
                                        ? new[] {x[0], x[1]}
                                        : x.Length == 1
                                            ? new[] {x[0], string.Empty}
                                            : throw new Exception($"Invalid parameters found in query string: {request.RequestUri.Query}"))
                            // order by the keys
                            .OrderBy(x => x[0], StringComparer.Ordinal)
                            // rebuild parts
                            .Select(x => $"{x[0]}={x[1]}"))
                    : string.Empty;
        
        public static string CanonicalHeaders(this HttpRequestMessage request)
            =>
                string.Join("\n",
                    request.Headers
                        // make the keys lowercase and join multiples with a comma
                        .Select(kvp => new[] {kvp.Key.ToLowerInvariant(), string.Join(",", kvp.Value)})
                        // order by the keys
                        .OrderBy(x => x[0], StringComparer.Ordinal)
                        // put keys and values back together using colons
                        .Select(x => $"{x[0]}:{x[1]}"));

        public static string SignedHeaders(this HttpRequestMessage request)
            =>
                string.Join(";",
                    request.Headers
                        .Select(kvp => kvp.Key.ToLowerInvariant())
                        .OrderBy(x => x, StringComparer.Ordinal));

        public static string ToCanonicalRequest(this HttpRequestMessage request, string hashedBody)
            =>
                request.Method.Method.ToUpper() + "\n" +
                CanonicalUri(request) + "\n" +
                CanonicalQueryParameters(request) + "\n" +
                CanonicalHeaders(request) + "\n" +
                "\n" +
                SignedHeaders(request) + "\n" +
                hashedBody;

        public static string ToHashedCanonicalRequest(this HttpRequestMessage request, string hashedBody, HashAlgorithm hashAlgorithm = null)
        {
            hashAlgorithm = hashAlgorithm ?? new SHA256Managed();

            return hashAlgorithm.Hash(request.ToCanonicalRequest(hashedBody));
        }

        public static async Task<string> HashBodyAsync(this HttpRequestMessage request, HashAlgorithm hashAlgorithm = null)
        {
            var bodyToHash = request.Content != null ? await request.Content.ReadAsStringAsync() : string.Empty;
            return hashAlgorithm.Hash(bodyToHash ?? string.Empty);
        }

        public static async Task<string> ToHashedCanonicalRequestAsync(this HttpRequestMessage request, HashAlgorithm hashAlgorithm = null)
        {
            hashAlgorithm = hashAlgorithm ?? new SHA256Managed();
            
            return hashAlgorithm.Hash(request.ToCanonicalRequest(await request.HashBodyAsync(hashAlgorithm)));
        }
    }
}