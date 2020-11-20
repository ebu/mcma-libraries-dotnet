using System;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace Mcma.Client
{
    public class ResourceManagerProvider : IResourceManagerProvider
    {
        public ResourceManagerProvider(HttpClient httpClient, IAuthProvider authProvider, IOptions<ResourceManagerProviderOptions> options)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            AuthProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));

            if (options.Value?.DefaultOptions != null &&
                (string.IsNullOrWhiteSpace(options.Value.DefaultOptions.ServicesUrl) ||
                 !Uri.IsWellFormedUriString(options.Value.DefaultOptions.ServicesUrl, UriKind.RelativeOrAbsolute)))
                throw new McmaException($"Invalid services url in default resource manager options: {options.Value.DefaultOptions.ServicesUrl}");

            DefaultOptions = options.Value?.DefaultOptions;
        }

        private HttpClient HttpClient { get; }

        private IAuthProvider AuthProvider { get; }

        private ResourceManagerOptions DefaultOptions { get; }

        public IResourceManager Get(McmaTracker tracker = null, ResourceManagerOptions options = null)
            => new ResourceManager(AuthProvider,
                                   HttpClient,
                                   (options ?? DefaultOptions) ??
                                   throw new McmaException("Config for resource manager not provided, and there is no default config available"),
                                   tracker);
    }
} 