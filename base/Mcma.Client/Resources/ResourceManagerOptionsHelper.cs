using System.Collections.Generic;

namespace Mcma.Client
{
    public static class ResourceManagerOptionsHelper
    {
        public static void SetFromEnvironmentVariables(this ResourceManagerOptions options)
        {
            options.ServicesUrl = McmaResourceManagerEnvironmentVariables.ServicesUrl;
            options.ServicesAuthType = McmaResourceManagerEnvironmentVariables.ServicesAuthType;
            options.ServicesAuthContext = McmaResourceManagerEnvironmentVariables.ServicesAuthContext;
        }

        public static Service ToServiceRegistryServiceData(this ResourceManagerOptions options)
            => new Service
            {
                Name = "Service Registry",
                AuthType = options.ServicesAuthType,
                AuthContext = options.ServicesAuthContext,
                Resources = new List<ResourceEndpoint>
                {
                    new ResourceEndpoint
                    {
                        ResourceType = nameof(Service),
                        HttpEndpoint = options.ServicesUrl
                    }
                }
            };
    }
}