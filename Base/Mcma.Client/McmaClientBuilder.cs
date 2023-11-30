using Mcma.Client.Auth;
using Mcma.Client.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mcma.Client;

public class McmaClientBuilder
{
    internal McmaClientBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException(nameof(services));
        Auth = new AuthenticatorRegistry(services);

        services.AddOptions()
                .AddSingleton<IAuthProvider, AuthProvider>()
                .AddSingleton(provider => provider.GetRequiredService<IResourceManagerProvider>().Get())
                .AddSingleton<IResourceManagerProvider, ResourceManagerProvider>();
    }

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