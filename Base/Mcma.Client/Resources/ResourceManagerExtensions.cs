using System.Threading;
using System.Threading.Tasks;
using Mcma.Model;

namespace Mcma.Client.Resources
{
    public static class ResourceManagerExtensions
    {
        public static Task<T> ResolveResourceFromFullUrl<T>(this IResourceManager resourceManager, string url) where T : McmaResource
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new McmaException("Url must be provided when resolving a resource from a full url.");

            return resourceManager.GetAsync<T>(url);
        }

        public static Task<T> UpdateAsync<T>(this IResourceManager resourceManager, T resource, CancellationToken cancellationToken = default)
            where T : McmaResource
            => resourceManager.UpdateAsync(resource.Id, resource, cancellationToken);

        public static Task SendNotificationAsync<T>(this IResourceManager resourceManager,
                                                    T resource,
                                                    NotificationEndpoint notificationEndpoint,
                                                    CancellationToken cancellationToken = default)
            where T : McmaResource
            => resourceManager.SendNotificationAsync(resource.Id, resource, notificationEndpoint, cancellationToken);

        public static Task SendJobNotificationAsync<T>(this IResourceManager resourceManager,
                                                       T resource,
                                                       NotificationEndpoint notificationEndpoint,
                                                       CancellationToken cancellationToken = default)
            where T : McmaResource, INotifiable
            => resourceManager.SendNotificationAsync(resource.Id, resource, resource.NotificationEndpoint, cancellationToken);
    }
}