using System;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace Mcma.Client
{
    public class ResourceManagerProvider : IResourceManagerProvider, IDisposable
    {
        public ResourceManagerProvider(HttpClient httpClient, IAuthProvider authProvider, IOptionsMonitor<ResourceManagerOptions> defaultOptionsMonitor)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            AuthProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));

            DefaultOptionsSubscription = defaultOptionsMonitor.OnChange(SetDefaultOptions);
            SetDefaultOptions(defaultOptionsMonitor.CurrentValue);
        }

        private void SetDefaultOptions(ResourceManagerOptions defaultOptions)
        {
            if (defaultOptions != null &&
                (string.IsNullOrWhiteSpace(defaultOptions.ServicesUrl) ||
                 !Uri.IsWellFormedUriString(defaultOptions.ServicesUrl, UriKind.RelativeOrAbsolute)))
                throw new McmaException($"Invalid services url in default resource manager options: {defaultOptions.ServicesUrl}");

            DefaultOptions = defaultOptions;
        }

        private HttpClient HttpClient { get; }

        private IAuthProvider AuthProvider { get; }
        
        private IDisposable DefaultOptionsSubscription { get; }

        private ResourceManagerOptions DefaultOptions { get; set; }

        public IResourceManager Get(ResourceManagerOptions options = null, McmaTracker tracker = null)
            => new ResourceManager(AuthProvider,
                                   HttpClient,
                                   (options ?? DefaultOptions) ??
                                   throw new McmaException("Config for resource manager not provided, and there is no default config available"),
                                   tracker);

        public void Dispose()
        {
            DefaultOptionsSubscription?.Dispose();
        }
    }
} 