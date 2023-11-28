using Mcma.Model;

namespace Mcma.Client.Resources;

public static class ResourceManagerProviderExtensions
{
    public static IResourceManager Get(this IResourceManagerProvider resourceManagerProvider,
                                       string serviceRegistryUrl,
                                       string serviceRegistryAuthType = null,
                                       string serviceRegistryAuthContext = null,
                                       McmaTracker tracker = null)
        =>
        resourceManagerProvider.Get(new(serviceRegistryUrl, serviceRegistryAuthType, serviceRegistryAuthContext), tracker);
}