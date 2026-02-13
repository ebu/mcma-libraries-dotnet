#if NET48_OR_GREATER
using System.Net.Http;
#endif
using Mcma.Client.Auth;
using Mcma.Model;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Resources;

public class ResourceManagerProvider : IResourceManagerProvider
{
    public ResourceManagerProvider(IHttpClientFactory httpClientFactory, IEnumerable<IAuthProvider> authProviders, IOptionsMonitor<ResourceManagerOptions> optionsMonitor)
    {
        HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        AuthProviders = authProviders?.ToArray() ?? throw new ArgumentNullException(nameof(authProviders));
        OptionsMonitor = optionsMonitor ?? throw new ArgumentNullException(nameof(optionsMonitor));
    }

    private IHttpClientFactory HttpClientFactory { get; }

    private IAuthProvider[] AuthProviders { get; }

    private IOptionsMonitor<ResourceManagerOptions> OptionsMonitor { get; }

    public IResourceManager Get(McmaTracker tracker = null)
        => Get(Options.DefaultName, tracker);

    public IResourceManager Get(string name = null, McmaTracker tracker = null)
    {
        var options = OptionsMonitor.Get(name);
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        options.Validate();

        var authProvider = AuthProviders.FirstOrDefault(x => string.Equals(x.Name ?? Options.DefaultName, name)) ?? new AuthProvider([]);

        return new ResourceManager(authProvider, HttpClientFactory.CreateClient(name), options, tracker);
    }
}