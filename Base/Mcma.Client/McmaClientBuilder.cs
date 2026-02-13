#if NET48_OR_GREATER
using System.Net.Http;
#endif
using Mcma.Client.Auth;
using Mcma.Client.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client;

public class McmaClientBuilder
{
    internal McmaClientBuilder(IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        ServiceLifetime = serviceLifetime;

        services.AddOptions();

        services.Add(ServiceDescriptor.Describe(typeof(IResourceManagerProvider), typeof(ResourceManagerProvider), serviceLifetime));
        services.Add(ServiceDescriptor.Describe(typeof(IResourceManager), provider => provider.GetRequiredService<IResourceManagerProvider>().Get(), serviceLifetime));
    }

    private IServiceCollection Services { get; }
    
    private ServiceLifetime ServiceLifetime { get; }
    
    internal bool IsDefaultResourceManagerConfigured { get; private set; }

    private Action<ResourceManagerBuilder> GetDefaultFromEnvVarsConfigurator(Action<ResourceManagerBuilder> configure)
        => x =>
        {
            x.Options.Configure(ResourceManagerOptions.ConfigureFromEnvVars);
            configure?.Invoke(x);
        };

    public McmaClientBuilder AddResourceManager(string name, Action<ResourceManagerBuilder> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        if (name == Options.DefaultName)
            IsDefaultResourceManagerConfigured = true;

        configure(new ResourceManagerBuilder(Services, ServiceLifetime, name));

        return this;
    }

    public McmaClientBuilder AddDefaultResourceManager(Action<ResourceManagerBuilder> configure)
        => AddResourceManager(Options.DefaultName, configure);

    public McmaClientBuilder AddDefaultResourceManagerFromEnvVars(Action<ResourceManagerBuilder> configure = null)
        => AddResourceManager(Options.DefaultName, GetDefaultFromEnvVarsConfigurator(configure));
}