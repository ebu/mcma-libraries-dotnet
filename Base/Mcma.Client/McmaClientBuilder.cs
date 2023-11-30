using Mcma.Client.Auth;
using Mcma.Client.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client;

public class McmaClientBuilder
{
    internal McmaClientBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Auth = new AuthenticatorRegistry(services);

        Services.AddOptions()
                .AddSingleton<IAuthProvider, AuthProvider>()
                .AddSingleton(provider => provider.GetRequiredService<IResourceManagerProvider>().GetDefault())
                .AddHttpClient<IResourceManagerProvider, ResourceManagerProvider>();
    }
        
    public IServiceCollection Services { get; }
        
    public AuthenticatorRegistry Auth { get; }

    public McmaClientBuilder Configure(Action<ResourceManagerProviderOptions> configure)
    {
        Services.Configure(configure);
        return this;
    }

    public McmaClientBuilder ConfigureResourceManagerDefaults(Action<ResourceManagerOptions> configureDefaults)
    {
        Services.Configure<ResourceManagerProviderOptions>(opts =>
        {
            opts.DefaultOptions = new ResourceManagerOptions();
            configureDefaults(opts.DefaultOptions);
        });

        return this;
    }

    public McmaClientBuilder ConfigureResourceManagerDefaults(string serviceRegistryUrl, string serviceRegistryAuthType = null, string serviceRegistryAuthContext = null)
    {
        if (string.IsNullOrWhiteSpace(serviceRegistryUrl))
            throw new ArgumentException($"'{nameof(serviceRegistryUrl)}' cannot be null or whitespace.", nameof(serviceRegistryUrl));

        return ConfigureResourceManagerDefaults(o =>
        {
            o.ServiceRegistryUrl = serviceRegistryUrl;
            o.ServiceRegistryAuthType = serviceRegistryAuthType;
            o.ServiceRegistryAuthContext = serviceRegistryAuthContext;
        });
    }
}