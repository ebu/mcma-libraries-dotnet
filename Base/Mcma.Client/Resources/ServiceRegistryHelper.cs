using Mcma.Model;

namespace Mcma.Client.Resources;

public static class ServiceRegistryHelper
{
    public const string ServiceName = "Service Registry";

    public static Service GetServiceData(ResourceManagerOptions options)
        => new()
        {
            Name = ServiceName,
            AuthType = options.ServiceRegistryAuthType,
            Resources =
            [
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
            ]
        };
}