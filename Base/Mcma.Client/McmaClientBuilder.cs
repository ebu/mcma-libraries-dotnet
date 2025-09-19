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
        Auth = new AuthenticatorRegistry(services, serviceLifetime);

        services.AddOptions()
                .AddMcmaAuthentication(serviceLifetime);

        services.Add(
            ServiceDescriptor.Describe(
                typeof(IResourceManagerProvider),
                serviceLifetime == ServiceLifetime.Singleton ? GetSingletonResourceManagerProvider : GetScopedOrTransientResourceManagerProvider,
                serviceLifetime));

        services.Add(ServiceDescriptor.Describe(typeof(IResourceManager), provider => provider.GetRequiredService<IResourceManagerProvider>().Get(), serviceLifetime));
    }

    private static IResourceManagerProvider GetSingletonResourceManagerProvider(IServiceProvider serviceProvider)
        => new ResourceManagerProvider(
            serviceProvider.GetRequiredService<IHttpClientFactory>(),
            serviceProvider.GetRequiredService<IAuthProvider>(),
            serviceProvider.GetRequiredService<IOptionsMonitor<ResourceManagerOptions>>());

    private static IResourceManagerProvider GetScopedOrTransientResourceManagerProvider(IServiceProvider serviceProvider)
        => new ResourceManagerProvider(
            serviceProvider.GetRequiredService<IHttpClientFactory>(),
            serviceProvider.GetRequiredService<IAuthProvider>(),
            serviceProvider.GetRequiredService<IOptionsSnapshot<ResourceManagerOptions>>());

    private IServiceCollection Services { get; }

    private AuthenticatorRegistry Auth { get; }

    internal bool IsDefaultResourceManagerConfigured { get; private set; }

    public McmaClientBuilder AddAuth(Action<AuthenticatorRegistry> addAuth)
    {
        if (addAuth is null)
            throw new ArgumentNullException(nameof(addAuth));

        addAuth(Auth);
        return this;
    }

    public McmaClientBuilder AddResourceManager(string name, Action<ResourceManagerBuilder> configure)
    {
        if (configure is null)
            throw new ArgumentNullException(nameof(configure));

        configure(new ResourceManagerBuilder(Services, name));
        return this;
    }

    public McmaClientBuilder AddDefaultResourceManager(Action<ResourceManagerBuilder> configure)
    {
        IsDefaultResourceManagerConfigured = true;
        return AddResourceManager(Options.DefaultName, configure);
    }

    public McmaClientBuilder AddDefaultResourceManagerFromEnvVars()
        => AddDefaultResourceManager(x => x.Options.Configure(ResourceManagerOptions.ConfigureFromEnvVars));
}