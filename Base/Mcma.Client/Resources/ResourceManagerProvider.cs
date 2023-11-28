using Mcma.Client.Auth;
using Mcma.Model;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Resources;

public class ResourceManagerProvider : IResourceManagerProvider
{
    public ResourceManagerProvider(HttpClient httpClient, IAuthProvider authProvider, IOptions<ResourceManagerProviderOptions> options)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        AuthProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));
        DefaultOptions = options.Value?.DefaultOptions ?? new();
    }

    private HttpClient HttpClient { get; }

    private IAuthProvider AuthProvider { get; }

    private ResourceManagerOptions DefaultOptions { get; }

    public IResourceManager GetDefault(McmaTracker tracker = null)
        => Get(DefaultOptions, tracker);

    public IResourceManager Get(ResourceManagerOptions options, McmaTracker tracker = null)
    {
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        options.Validate();

        return new ResourceManager(AuthProvider, HttpClient, options, tracker);
    }
}