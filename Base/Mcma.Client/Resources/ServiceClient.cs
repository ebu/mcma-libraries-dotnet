using Mcma.Client.Auth;
using Mcma.Model;

namespace Mcma.Client.Resources;

internal class ServiceClient : IServiceClient
{
    internal ServiceClient(IAuthProvider authProvider, HttpClient httpClient, Service service, McmaTracker tracker)
    {
        Service = service;
        Tracker = tracker;

        ResourcesByType =
            service.Resources != null
                ? service.Resources.ToDictionary(r => r.ResourceType,
                                                 r =>
                                                     new ResourceEndpointClient(authProvider,
                                                                                httpClient,
                                                                                service,
                                                                                r,
                                                                                Tracker))
                : new Dictionary<string, ResourceEndpointClient>();
    }
    
    private Service Service { get; }

    private McmaTracker Tracker { get; }

    private IDictionary<string, ResourceEndpointClient> ResourcesByType { get; }

    internal IEnumerable<ResourceEndpointClient> Resources => ResourcesByType.Values;

    public string Name => Service?.Name;

    internal bool HasResourceEndpointClient(string resourceType)
        => ResourcesByType.ContainsKey(resourceType);

    internal IResourceEndpointClient GetResourceEndpointClient(string resourceType) =>
        ResourcesByType.ContainsKey(resourceType) ? ResourcesByType[resourceType] : null;

    internal bool HasResourceEndpointClient(Type resourceType) => HasResourceEndpointClient(resourceType.Name);
            
    internal IResourceEndpointClient GetResourceEndpointClient(Type resourceType) => GetResourceEndpointClient(resourceType.Name);

    public bool HasResourceEndpointClient<T>() => HasResourceEndpointClient(typeof(T));
            
    public IResourceEndpointClient GetResourceEndpointClient<T>() => GetResourceEndpointClient(typeof(T));
}