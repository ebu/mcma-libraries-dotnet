#if NETFRAMEWORK
using System.Net.Http;
using Mcma;
using Mcma.Client;

#endif
using Mcma.Client.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client.Resources;

public class ResourceManagerBuilder
{
    internal ResourceManagerBuilder(IServiceCollection services, ServiceLifetime serviceLifetime, string name = null)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Name = name ?? Microsoft.Extensions.Options.Options.DefaultName;

        HttpClient = services.AddHttpClient(Name);
        Auth = new AuthenticatorRegistry(services, serviceLifetime, Name);
        Options = services.AddOptions<ResourceManagerOptions>(Name);
    }

    public IServiceCollection Services { get; }

    public string Name { get; }

    public IHttpClientBuilder HttpClient { get; }

    public AuthenticatorRegistry Auth { get; }

    public OptionsBuilder<ResourceManagerOptions> Options { get; }
}