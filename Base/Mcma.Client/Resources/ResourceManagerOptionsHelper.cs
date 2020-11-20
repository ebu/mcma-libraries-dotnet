﻿using System.Collections.Generic;

namespace Mcma.Client
{
    public static class ResourceManagerOptionsHelper
    {
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