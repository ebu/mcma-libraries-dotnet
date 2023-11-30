using Mcma.Client.Auth;
using Mcma.Model;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Resources;

public class ResourceManagerProvider : IResourceManagerProvider
{
    public ResourceManagerProvider(IHttpClientFactory httpClientFactory, IAuthProvider authProvider, IOptionsSnapshot<ResourceManagerOptions> optionsSnapshot)
    {
        HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        AuthProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));
        OptionsSnapshot = optionsSnapshot ?? throw new ArgumentNullException(nameof(optionsSnapshot));
    }

    private IHttpClientFactory HttpClientFactory { get; }

    private IAuthProvider AuthProvider { get; }

    private IOptionsSnapshot<ResourceManagerOptions> OptionsSnapshot { get; }

    public IResourceManager Get(McmaTracker tracker = null)
        => Get(Options.DefaultName, tracker);

    public IResourceManager Get(string name = null, McmaTracker tracker = null)
    {
        var options = OptionsSnapshot.Get(name);
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        options.Validate();

        return new ResourceManager(AuthProvider, HttpClientFactory.CreateClient(name), options, tracker);
    }
}