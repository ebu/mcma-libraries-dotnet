using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Mcma.Common.Google.Metadata
{
    public class MetadataService : IMetadataService
    {
        private const string BaseUrl = "http://169.254.169.254/computeMetadata/v1";
        private const string MetadataFlavorHeaderName = "Metadata-Flavor";
        private const string MetadataFlavorHeaderValue = "Google";

        private ConcurrentDictionary<string, Task<string[]>> MetadataCache { get; } = new ConcurrentDictionary<string, Task<string[]>>(StringComparer.OrdinalIgnoreCase);
        
        private HttpClient HttpClient { get; } = new HttpClient();

        public Task<string[]> GetAsync(string path) => MetadataCache.GetOrAdd(path, RetrieveMetadataFromServerAsync);

        private async Task<string[]> RetrieveMetadataFromServerAsync(string path)
        {
            for (var i = 0; i < 3; i++)
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(500);
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, BaseUrl + "/" + path.TrimStart('/'));
                    request.Headers.Add(MetadataFlavorHeaderName, MetadataFlavorHeaderValue);

                    var response = await HttpClient.SendAsync(request, cts.Token);
                    
                    // ensure we have the Google header we're expecting
                    if (!response.Headers.TryGetValues(MetadataFlavorHeaderName, out var headerValues) ||
                        !headerValues.Contains(MetadataFlavorHeaderValue))
                        break;
                    
                    var strBody = await response.Content.ReadAsStringAsync();

                    return strBody.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
                }
                catch (Exception e) when (e is HttpRequestException || e is WebException || e is OperationCanceledException)
                {
                    // nothing to do, just retry
                }
            }

            return new string[0];
        }
    }
}