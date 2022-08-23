using System;
using System.Threading.Tasks;
using Mcma.Serialization;

namespace Mcma.Client.Auth;

public abstract class AuthenticatorFactory<TAuthContext> : IAuthenticatorFactory
{
    Type IAuthenticatorFactory.ContextType => typeof(TAuthContext);

    protected virtual TAuthContext DefaultAuthContext => default;

    protected static bool TryParseContext(string authContextString, out TAuthContext authContext)
    {
        authContext = default;
        try
        {
            authContext = McmaJson.Parse(authContextString).ToMcmaObject<TAuthContext>();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task<IAuthenticator> GetAsync(object authContext)
    {
        authContext ??= DefaultAuthContext;
            
        if (authContext is string contextStr && TryParseContext(contextStr, out var parsedContext))
            authContext = parsedContext;

        if (!(authContext is TAuthContext typedContext))
            throw new McmaException(
                $"Expected an auth context of type {typeof(TAuthContext).Name} but was given an auth context of type {authContext?.GetType().Name ?? "[null]"}");

        return GetAsync(typedContext);
    }

    protected abstract Task<IAuthenticator> GetAsync(TAuthContext authContext);
}