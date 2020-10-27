using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Mcma.Client
{
    public interface IResourceManager
    {
        Task InitAsync();
        
        IServiceClient GetServiceClient(Service service);

        Task<IResourceEndpointClient> GetResourceEndpointClientAsync(string url);

        Task<IEnumerable<T>> QueryAsync<T>((string, string)[] filter, CancellationToken cancellationToken = default) where T : McmaObject;

        Task<T> GetAsync<T>(string resourceId, CancellationToken cancellationToken = default) where T : McmaObject;

        Task<T> CreateAsync<T>(string resourceId, T resource, CancellationToken cancellationToken = default) where T : McmaObject;

        Task<T> UpdateAsync<T>(string resourceId, T resource, CancellationToken cancellationToken = default) where T : McmaObject;

        Task DeleteAsync<T>(string resourceId, CancellationToken cancellationToken = default) where T : McmaObject;

        Task SendNotificationAsync<T>(string resourceId, T resource, NotificationEndpoint notificationEndpoint, CancellationToken cancellationToken = default) where T : McmaObject;
    }
}