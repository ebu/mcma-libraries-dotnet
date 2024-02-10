using Mcma.Model;

namespace Mcma.Client.Resources;

public static class ResourceManagerExtensions
{
    public static Task<T?> ResolveResourceFromFullUrl<T>(this IResourceManager resourceManager, string url) where T : McmaResource
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new McmaException("Url must be provided when resolving a resource from a full url.");

        return resourceManager.GetAsync<T>(url);
    }

    public static Task<T> UpdateAsync<T>(this IResourceManager resourceManager, T resource, CancellationToken cancellationToken = default)
        where T : McmaResource
    {
        if (resource is null)
            throw new ArgumentNullException(nameof(resource));

        if (string.IsNullOrWhiteSpace(resource.Id))
            throw new McmaException("Resource does not specify an ID for update");

        return resourceManager.UpdateAsync(resource!.Id!, resource, cancellationToken);
    }

    public static Task SendNotificationAsync<T>(this IResourceManager resourceManager,
                                                T resource,
                                                NotificationEndpoint notificationEndpoint,
                                                CancellationToken cancellationToken = default)
        where T : McmaResource
        => resourceManager.SendNotificationAsync(notificationEndpoint, resource?.Id, resource, cancellationToken);

    public static Task SendResourceNotificationAsync<T>(this IResourceManager resourceManager, T resource, CancellationToken cancellationToken = default)
        where T : McmaResource, INotifiable
    {
        if (resource is null)
            throw new ArgumentNullException(nameof(resource));

        if (resource.NotificationEndpoint is null)
            throw new McmaException($"Resource notification endpoint cannot be null");
        
        return resourceManager.SendNotificationAsync(resource.NotificationEndpoint, resource.Id, resource, cancellationToken);
    }
}