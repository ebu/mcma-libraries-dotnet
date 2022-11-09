using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Client.Auth;

public class AuthenticatorRegistry
{
    internal AuthenticatorRegistry(IServiceCollection services)
    {
        Services = services;
    }
        
    public IServiceCollection Services { get; }
        
    private List<AuthenticatorKey> RegisteredKeys { get; } = new();

    private AuthenticatorRegistry InternalAdd<TAuthenticator>(AuthenticatorKey key, Action<IServiceCollection> addAuthenticatorService)
        where TAuthenticator : class, IAuthenticator
    {
        addAuthenticatorService(Services);
        Services.AddSingleton(svcProvider => new AuthenticatorRegistration(key, svcProvider.GetRequiredService<TAuthenticator>()));
        RegisteredKeys.Add(key);
        return this;
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

    public AuthenticatorRegistry Add<TAuthenticator>(AuthenticatorKey key)
        where TAuthenticator : class, IAuthenticator
        =>
            RegisteredKeys.All(k => k != key)
                ? InternalAdd<TAuthenticator>(key, services => services.AddSingleton<TAuthenticator>())
               : throw GetAlreadyRegisteredException(key);

    public AuthenticatorRegistry Add<TAuthenticator>(AuthenticatorKey key, Func<IServiceProvider, TAuthenticator> serviceFactory)
        where TAuthenticator : class, IAuthenticator
        => 
            RegisteredKeys.All(k => k != key)
               ? InternalAdd<TAuthenticator>(key, services => services.AddSingleton(serviceFactory))
               : throw GetAlreadyRegisteredException(key);

    public bool TryAdd<TAuthenticator>(AuthenticatorKey key)
        where TAuthenticator : class, IAuthenticator
    {
        var exists = RegisteredKeys.Any(k => k == key);
        if (!exists)
            Add<TAuthenticator>(key);
        return !exists;
    }

    public bool TryAdd<TAuthenticator>(AuthenticatorKey key, Func<IServiceProvider, TAuthenticator> serviceFactory)
        where TAuthenticator : class, IAuthenticator
    {
        var exists = RegisteredKeys.Any(k => k == key);
        if (!exists)
            Add(key, serviceFactory);
        return !exists;
    }
}