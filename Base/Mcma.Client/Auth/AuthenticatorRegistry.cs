using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client.Auth;

public class AuthenticatorRegistry
{
    internal AuthenticatorRegistry(IServiceCollection services, ServiceLifetime serviceLifetime, string name)
    {
        Services = services;
        ServiceLifetime = serviceLifetime;
        Name = name;

        Services.Add(new(typeof(IAuthProvider), x => new AuthProvider(x.GetRequiredKeyedService<IEnumerable<AuthenticatorRegistration>>(Name), Name), serviceLifetime));
    }

    public IServiceCollection Services { get; }
    
    private ServiceLifetime ServiceLifetime { get; }

    private string Name { get; }
    
    private List<AuthenticatorKey> RegisteredKeys { get; } = [];

    private ServiceDescriptor GetServiceDescriptorForType<TAuthenticator>(string serviceKey)
        where TAuthenticator : class, IAuthenticator
        => new(typeof(TAuthenticator), serviceKey, typeof(TAuthenticator), ServiceLifetime);

    private Func<string, ServiceDescriptor> GetServiceDescriptorFactory<TAuthenticator>(Func<IServiceProvider, TAuthenticator> authenticatorFactory)
        where TAuthenticator : class, IAuthenticator
        => serviceKey => new(typeof(TAuthenticator), serviceKey, (svcProvider, _) => authenticatorFactory(svcProvider), ServiceLifetime);

    private bool TryAdd<TAuthenticator>(AuthenticatorKey key, Func<string, ServiceDescriptor> createServiceDescriptor)
        where TAuthenticator : class, IAuthenticator
    {
        if (RegisteredKeys.Any(k => k == key))
            return false;

        var serviceKey = $"{Name}/{key.Key}";

        Services.Add(createServiceDescriptor(serviceKey));

        Services.Add(
            new(typeof(AuthenticatorRegistration),
                Name,
                (svcProvider, _) => new AuthenticatorRegistration(key, svcProvider.GetRequiredKeyedService<TAuthenticator>(serviceKey)),
                ServiceLifetime));

        RegisteredKeys.Add(key);

        return true;
    }

    private static McmaException GetAlreadyRegisteredException(AuthenticatorKey key)
    {
        var keyInfo = $"auth type '{key.AuthType}'";
        if (!string.IsNullOrWhiteSpace(key.ServiceName))
            keyInfo += $" and service '{key.ServiceName}'";
        if (!string.IsNullOrWhiteSpace(key.ResourceType))
            keyInfo += $" and resource type '{key.ResourceType}'";

        return new McmaException(
            $"An authentication handler for {keyInfo} has already been registered. " +
            "If you wish to register a default in the case that no handler was previously registered, please use TryAdd.");
    }

    private AuthenticatorRegistry Add<TAuthenticator>(AuthenticatorKey key, Func<string, ServiceDescriptor> createServiceDescriptor)
        where TAuthenticator : class, IAuthenticator
    {
        if (!TryAdd<TAuthenticator>(key, createServiceDescriptor))
            throw GetAlreadyRegisteredException(key);

        return this;
    }

    public bool TryAdd<TAuthenticator>(AuthenticatorKey key)
        where TAuthenticator : class, IAuthenticator
        => TryAdd<TAuthenticator>(key, GetServiceDescriptorForType<TAuthenticator>);

    public bool TryAdd<TAuthenticator>(AuthenticatorKey key, Func<IServiceProvider, TAuthenticator> serviceFactory)
        where TAuthenticator : class, IAuthenticator
        => TryAdd<TAuthenticator>(key, GetServiceDescriptorFactory(serviceFactory));

    public AuthenticatorRegistry Add<TAuthenticator>(AuthenticatorKey key)
        where TAuthenticator : class, IAuthenticator
        =>
        Add<TAuthenticator>(key, GetServiceDescriptorForType<TAuthenticator>);

    public AuthenticatorRegistry Add<TAuthenticator>(AuthenticatorKey key, Func<IServiceProvider, TAuthenticator> serviceFactory)
        where TAuthenticator : class, IAuthenticator
        =>
        Add<TAuthenticator>(key, GetServiceDescriptorFactory(serviceFactory));

    public AuthenticatorRegistry Add<TKey, TAuthenticator>(string serviceName = null, string resourceType = null)
        where TKey : AuthenticatorKey, new()
        where TAuthenticator : class, IAuthenticator
        =>
        Add<TAuthenticator>(AuthenticatorKey.Create<TKey>(serviceName, resourceType));

    public AuthenticatorRegistry Add<TKey, TAuthenticator>(Func<IServiceProvider, TAuthenticator> serviceFactory, string serviceName = null, string resourceType = null)
        where TKey : AuthenticatorKey, new()
        where TAuthenticator : class, IAuthenticator
        =>
        Add(AuthenticatorKey.Create<TKey>(serviceName, resourceType), serviceFactory);
}