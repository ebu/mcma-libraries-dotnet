using System;
using System.Threading.Tasks;
using Mcma.Serialization;
using Newtonsoft.Json.Linq;

namespace Mcma.Client
{
    public abstract class AuthenticatorFactory<TAuthContext> : IAuthenticatorFactory
    {
        Type IAuthenticatorFactory.ContextType => typeof(TAuthContext); 

        protected static bool TryParseContext(string authContextString, out TAuthContext authContext)
        {
            authContext = default;
            try
            {
                authContext = JToken.Parse(authContextString).ToMcmaObject<TAuthContext>();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<IAuthenticator> GetAsync(object authContext)
        {
            if (authContext is string contextStr && TryParseContext(contextStr, out var parsedContext))
                authContext = parsedContext;

            if (!(authContext is TAuthContext typedContext))
                throw new McmaException(
                    $"Expected an auth context of type {typeof(TAuthContext).Name} but was given an auth context of type {authContext?.GetType().Name ?? "[null]"}");

            return GetAsync(typedContext);
        }

        protected abstract Task<IAuthenticator> GetAsync(TAuthContext authContext);
    }
}