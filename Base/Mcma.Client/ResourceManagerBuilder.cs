using Mcma.Client.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client;

public class ResourceManagerBuilder
{
    internal ResourceManagerBuilder(IServiceCollection services, string? name = null)
    {
        Name = name ?? Microsoft.Extensions.Options.Options.DefaultName;

        Options = services.AddOptions<ResourceManagerOptions>(name);
        HttpClient = services.AddHttpClient(Name);
    }

    public string Name { get; }

    public OptionsBuilder<ResourceManagerOptions> Options { get; }

    public IHttpClientBuilder HttpClient { get; }
}