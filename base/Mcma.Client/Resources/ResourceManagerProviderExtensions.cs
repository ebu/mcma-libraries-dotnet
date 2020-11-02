namespace Mcma.Client
{
    public static class ResourceManagerProviderExtensions
    {
        public static IResourceManager Get(this IResourceManagerProvider resourceManagerProvider,
                                           string servicesUrl,
                                           string servicesAuthType = null,
                                           string servicesAuthContext = null,
                                           McmaTracker tracker = null)
            => resourceManagerProvider.Get(tracker,
                                           new ResourceManagerOptions
                                           {
                                               ServicesUrl = servicesUrl,
                                               ServicesAuthType = servicesAuthType,
                                               ServicesAuthContext = servicesAuthContext
                                           });
    }
} 