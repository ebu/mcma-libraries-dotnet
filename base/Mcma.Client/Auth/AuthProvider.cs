using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mcma.Serialization;
using Newtonsoft.Json.Linq;

namespace Mcma.Client
{
    internal class AuthProvider : IAuthProvider
    {
        public AuthProvider(IEnumerable<IAuthenticatorFactory> handlerFactories,
                            IEnumerable<IAuthenticatorFactoryRegistration> handlerFactoryRegistrations)
        {
            HandlerFactories = handlerFactories?.ToArray() ?? new IAuthenticatorFactory[0];
            HandlerFactoryRegistrations = handlerFactoryRegistrations?.ToArray() ?? new IAuthenticatorFactoryRegistration[0];
        }

        private IAuthenticatorFactory[] HandlerFactories { get; }

        private IAuthenticatorFactoryRegistration[] HandlerFactoryRegistrations { get; }

        private Dictionary<(string, string), IAuthenticator> Cache { get; } = new Dictionary<(string, string), IAuthenticator>();

        public async Task<IAuthenticator> GetAsync(string authType, object authContext)
        {
            var registration = HandlerFactoryRegistrations.FirstOrDefault(x => x.AuthType.Equals(authType,StringComparison.OrdinalIgnoreCase));
            if (registration == null)
                throw new McmaException($"No authenticators registered for auth type '{authType}'");

            authContext ??= registration.DefaultAuthContext;
            
            var cacheKey = (authType, authContext as string ?? JToken.FromObject(authContext).ToString());
            
            if (Cache.ContainsKey(cacheKey))
                return Cache[cacheKey];

            var factory = HandlerFactories.FirstOrDefault(x => x.GetType() == registration.FactoryType);
            if (factory == null)
                throw new McmaException(
                    $"Auth type '{authType}' is registered to use authenticator factory of type {registration.FactoryType}, but no authenticator factory of this type was registered.");

            if (authContext is string authContextStr && (authContextStr.StartsWith("{") || authContextStr.StartsWith("[")))
            {
                try
                {
                    authContext = JToken.Parse(authContextStr).ToMcmaObject(factory.ContextType);
                }
                catch
                {
                    // nothing to do, just pass it along
                }
            }
            
            Cache[cacheKey] = await factory.GetAsync(authContext);

            return Cache[cacheKey];
        }
    }
}