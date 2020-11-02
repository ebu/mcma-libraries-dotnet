using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client
{
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

        public McmaClientBuilder ConfigureDefaults(string servicesUrl, string servicesAuthType = null, string servicesAuthContext = null)
        {
            Services.Configure<ResourceManagerProviderOptions>(
                opts =>
                    opts.DefaultOptions = new ResourceManagerOptions(servicesUrl, servicesAuthType, servicesAuthContext));

            return this;
        }

        public McmaClientBuilder ConfigureDefaultsFromEnvironmentVariables() =>
            ConfigureDefaults(ResourceManagerOptionsHelper.SetFromEnvironmentVariables);
    }
}