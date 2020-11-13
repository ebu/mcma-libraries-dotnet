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
            AuthFactories = handlerFactories?.ToArray() ?? new IAuthenticatorFactory[0];
            AuthFactoryRegistrations = handlerFactoryRegistrations?.ToArray() ?? new IAuthenticatorFactoryRegistration[0];
        }

        private IAuthenticatorFactory[] AuthFactories { get; }

        private IAuthenticatorFactoryRegistration[] AuthFactoryRegistrations { get; }

        private Dictionary<(string, string), IAuthenticator> Cache { get; } = new Dictionary<(string, string), IAuthenticator>();

        public async Task<IAuthenticator> GetAsync(string authType, object authContext)
        {
            var registration = AuthFactoryRegistrations.FirstOrDefault(x => x.AuthType.Equals(authType,StringComparison.OrdinalIgnoreCase));
            if (registration == null)
                throw new McmaException($"No authenticators registered for auth type '{authType}'");
            
            var cacheKey = (authType, authContext as string ?? JToken.FromObject(authContext ?? "").ToString());
            
            if (Cache.ContainsKey(cacheKey))
                return Cache[cacheKey];

            var factory = AuthFactories.FirstOrDefault(x => x.GetType() == registration.FactoryType);
            if (factory == null)
                throw new McmaException(
                    $"Auth type '{authType}' is registered to use authenticator factory of type {registration.FactoryType}, but no authenticator factory of this type was registered.");
            
            Cache[cacheKey] = await factory.GetAsync(authContext);

            return Cache[cacheKey];
        }
    }
}