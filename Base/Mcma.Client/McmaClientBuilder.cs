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
                .AddSingleton(provider => provider.GetRequiredService<IResourceManagerProvider>().Get())
                .AddHttpClient<IResourceManagerProvider, ResourceManagerProvider>();
    }
        
    public IServiceCollection Services { get; }
        
    public AuthenticatorRegistry Auth { get; }

    public McmaClientBuilder Configure(Action<ResourceManagerProviderOptions> configure)
    {
        Services.Configure(configure);
        return this;
    }

    public McmaClientBuilder ConfigureDefaults(Action<ResourceManagerOptions> configureDefaults)
    {
        Services.Configure<ResourceManagerProviderOptions>(opts =>
        {
            opts.DefaultOptions = new ResourceManagerOptions();
            configureDefaults(opts.DefaultOptions);
        });

        return this;
    }

    public McmaClientBuilder ConfigureDefaults(string serviceRegistryUrl = null, string serviceRegistryAuthType = null, string serviceRegistryAuthContext = null)
    {
        serviceRegistryUrl ??= McmaResourceManagerEnvironmentVariables.ServiceRegistryUrl;
            
        Services.Configure<ResourceManagerProviderOptions>(
            opts =>
                opts.DefaultOptions = new ResourceManagerOptions(serviceRegistryUrl, serviceRegistryAuthType, serviceRegistryAuthContext));

        return this;
    }
}