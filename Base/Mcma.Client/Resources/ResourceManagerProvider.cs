#if NET48_OR_GREATER
using System.Net.Http;
#endif
using Mcma.Client.Auth;
using Mcma.Model;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Resources;

public class ResourceManagerProvider : IResourceManagerProvider
{
    private ResourceManagerProvider(IHttpClientFactory httpClientFactory, IAuthProvider authProvider, Func<string, ResourceManagerOptions> getOptions)
    {
        HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        AuthProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));
        GetOptions = getOptions ?? throw new ArgumentNullException(nameof(getOptions));
    }

    public ResourceManagerProvider(IHttpClientFactory httpClientFactory, IAuthProvider authProvider, IOptionsMonitor<ResourceManagerOptions> optionsMonitor)
        : this(httpClientFactory, authProvider, optionsMonitor.Get)
    {
    }

    public ResourceManagerProvider(IHttpClientFactory httpClientFactory, IAuthProvider authProvider, IOptionsSnapshot<ResourceManagerOptions> optionsSnapshot)
        : this(httpClientFactory, authProvider, optionsSnapshot.Get)
    {
    }

    private IHttpClientFactory HttpClientFactory { get; }

    private IAuthProvider AuthProvider { get; }

    private Func<string, ResourceManagerOptions> GetOptions { get; }

    public IResourceManager Get(McmaTracker tracker = null)
        => Get(Options.DefaultName, tracker);

    public IResourceManager Get(string name = null, McmaTracker tracker = null)
    {
        var options = GetOptions(name);
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        options.Validate();

        return new ResourceManager(AuthProvider, HttpClientFactory.CreateClient(name), options, tracker);
    }
}