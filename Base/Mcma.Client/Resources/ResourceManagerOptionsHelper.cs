using Mcma.Model;

namespace Mcma.Client.Resources;

public static class ResourceManagerOptionsHelper
{
    public static Service ToServiceRegistryServiceData(this ResourceManagerOptions options)
        => new()
        {
            Name = "Service Registry",
            AuthType = options.ServiceRegistryAuthType,
            Resources = new List<ResourceEndpoint>
            {
                new()
                {
                    ResourceType = nameof(Service),
                    HttpEndpoint = options.ServiceRegistryUrl?.TrimEnd('/') + "/services" 
                },
                new()
                {
                    ResourceType = nameof(JobProfile),
                    HttpEndpoint = options.ServiceRegistryUrl?.TrimEnd('/') + "/job-profile" 
                }
            }
        };
}