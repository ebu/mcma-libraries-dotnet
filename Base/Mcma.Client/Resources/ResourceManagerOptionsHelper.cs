using System.Collections.Generic;
using Mcma.Model;

namespace Mcma.Client.Resources
{
    public static class ResourceManagerOptionsHelper
    {
        public static Service ToServiceRegistryServiceData(this ResourceManagerOptions options)
            => new()
            {
                Name = "Service Registry",
                AuthType = options.ServicesAuthType,
                AuthContext = options.ServicesAuthContext,
                Resources = new List<ResourceEndpoint>
                {
                    new()
                    {
                        ResourceType = nameof(Service),
                        HttpEndpoint = options.ServicesUrl
                    }
                }
            };
    }
}