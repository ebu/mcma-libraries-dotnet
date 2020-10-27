using System.Collections.Generic;

namespace Mcma.Client
{
    public class ResourceManagerOptions
    {
        public string ServicesUrl { get; set;  }

        public string ServicesAuthType { get; set; }

        public string ServicesAuthContext { get; set; }

        public Service ToServiceRegistryServiceData()
            => new Service
            {
                Name = "Service Registry",
                AuthType = ServicesAuthType,
                AuthContext = ServicesAuthContext,
                Resources = new List<ResourceEndpoint>
                {
                    new ResourceEndpoint
                    {
                        ResourceType = nameof(Service),
                        HttpEndpoint = ServicesUrl
                    }
                }
            };
    }
}