using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mcma.Client.Auth;
using Mcma.Client.Http;
using Mcma.Logging;
using Mcma.Model;
using Mcma.Serialization;

namespace Mcma.Client.Resources
{
    internal class ResourceManager : IResourceManager
    {
        internal ResourceManager(IAuthProvider authProvider, HttpClient httpClient, ResourceManagerOptions options, McmaTracker tracker)
        {
            AuthProvider = authProvider ?? throw new ArgumentNullException(nameof(authProvider));
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Tracker = tracker;

            McmaHttpClient = new McmaHttpClient(HttpClient);
            ServiceRegistryClient = new ServiceClient(AuthProvider, HttpClient, Options.ToServiceRegistryServiceData(), Tracker);
        }

        private IAuthProvider AuthProvider { get; }

        private HttpClient HttpClient { get; }

        private ResourceManagerOptions Options { get; }

        private McmaTracker Tracker { get; }

        private McmaHttpClient McmaHttpClient { get; }

        private ServiceClient ServiceRegistryClient { get; }

        private List<ServiceClient> Services { get; } = new List<ServiceClient>();

        public async Task InitAsync()
        {
            try
            {
                Services.Clear();

                Services.Add(ServiceRegistryClient);

                var servicesEndpoint = ServiceRegistryClient.GetResourceEndpointClient<Service>();
                
                var response = await servicesEndpoint.QueryAsync<Service>();

                Services.AddRange(response.Results.Select(GetServiceClient));
            }
            catch (Exception error)
            {
                throw new McmaException("ResourceManager: Failed to initialize", error);
            }
        }
        

        private ServiceClient GetServiceClient(Service service) => new(AuthProvider, HttpClient, service, Tracker);

        IServiceClient IResourceManager.GetServiceClient(Service service) => GetServiceClient(service);

        private async Task<ResourceEndpointClient> GetResourceEndpointAsync(string url)
        {
            if (!Services.Any())
                await InitAsync();

            if (string.IsNullOrWhiteSpace(url))
                return null;

            return Services.SelectMany(s => s.Resources)
                           .FirstOrDefault(re => url.StartsWith(re.HttpEndpoint, StringComparison.OrdinalIgnoreCase));
        }

        async Task<IResourceEndpointClient> IResourceManager.GetResourceEndpointClientAsync(string url) => await GetResourceEndpointAsync(url);

        public Task<IEnumerable<T>> QueryAsync<T>(params (string, string)[] filter) where T : McmaObject
            => QueryAsync<T>(CancellationToken.None, filter);
        
        public async Task<IEnumerable<T>> QueryAsync<T>(CancellationToken cancellationToken, params (string, string)[] filter) where T : McmaObject
        {
            if (!Services.Any())
                await InitAsync();

            var resourceEndpointClients = Services.Where(s => s.HasResourceEndpointClient<T>()).Select(s => s.GetResourceEndpointClient<T>()).ToList();
            if (!resourceEndpointClients.Any())
                throw new McmaException($"There are no available resource endpoints for resource of type '{typeof(T)}'");

            var results = new List<T>();
            var usedHttpEndpoints = new List<string>();
            var exceptions = new List<Exception>();

            foreach (var resourceEndpoint in resourceEndpointClients)
            {
                if (usedHttpEndpoints.Contains(resourceEndpoint.HttpEndpoint, StringComparer.OrdinalIgnoreCase))
                    continue;

                try
                {
                    var queryResults = await resourceEndpoint.QueryAsync<T>(cancellationToken, filter);
                    
                    results.AddRange(queryResults.Results);
                    
                    usedHttpEndpoints.Add(resourceEndpoint.HttpEndpoint);
                }
                catch (Exception error)
                {
                    var message = "Failed to retrieve '" + typeof(T).Name + "' from endpoint '" + resourceEndpoint.HttpEndpoint + "'";
                    Logger.System.Error(message, error);
                    exceptions.Add(new Exception(message, error));
                }
            }

            if (exceptions.Any())
                throw new AggregateException($"Failed to query any available resource endpoints for resource type '{typeof(T)}'", exceptions);

            return new ReadOnlyCollection<T>(results);
        }

        public async Task<T> CreateAsync<T>(T resource, CancellationToken cancellationToken = default) where T : McmaObject
        {
            if (!Services.Any())
                await InitAsync();

            var resourceEndpoint =
                Services.Where(s => s.HasResourceEndpointClient<T>())
                    .Select(s => s.GetResourceEndpointClient<T>())
                    .FirstOrDefault();
            if (resourceEndpoint != null)
                return await resourceEndpoint.PostAsync<T>(resource, cancellationToken: cancellationToken);

            if (resource is not McmaResource mcmaResource || string.IsNullOrWhiteSpace(mcmaResource.Id))
                throw new McmaException($"There is no endpoint available for creating resources of type '{typeof(T).Name}', and the provided resource does not specify an endpoint in its 'id' property.");

            return await McmaHttpClient.PostAsync<T>(mcmaResource.Id, mcmaResource, cancellationToken);
        }

        public async Task<T> UpdateAsync<T>(string resourceId, T resource, CancellationToken cancellationToken = default) where T : McmaObject
        {
            if (!Services.Any())
                await InitAsync();

            var resourceEndpoint =
                Services.Where(s => s.HasResourceEndpointClient<T>())
                    .Select(s => s.GetResourceEndpointClient<T>())
                    .FirstOrDefault(re => resourceId.StartsWith(re.HttpEndpoint, StringComparison.OrdinalIgnoreCase));
            if (resourceEndpoint != null)
                return await resourceEndpoint.PutAsync<T>(resource, resourceId, cancellationToken);

            return await McmaHttpClient.PutAsync<T>(resourceId, resource, cancellationToken);
        }

        public async Task DeleteAsync<T>(string resourceId, CancellationToken cancellationToken = default) where T : McmaObject
        {
            if (!Services.Any())
                await InitAsync();

            var resourceEndpoint =
                Services.Where(s => s.HasResourceEndpointClient<T>())
                        .Select(s => s.GetResourceEndpointClient<T>())
                        .FirstOrDefault(re => resourceId.StartsWith(re.HttpEndpoint, StringComparison.OrdinalIgnoreCase));

            if (resourceEndpoint != null)
                await resourceEndpoint.DeleteAsync(resourceId, cancellationToken);
            else
                await McmaHttpClient.DeleteAsync(resourceId, cancellationToken);
        }

        public async Task<T> GetAsync<T>(string resourceId, CancellationToken cancellationToken = default) where T : McmaObject
        {
            var resourceEndpoint = await GetResourceEndpointAsync(resourceId);

            return resourceEndpoint != null
                ? await resourceEndpoint.GetAsync<T>(resourceId, cancellationToken)
                : await McmaHttpClient.GetAsync<T>(resourceId, true, cancellationToken);
        }

        public async Task SendNotificationAsync<T>(string resourceId, T resource, NotificationEndpoint notificationEndpoint, CancellationToken cancellationToken = default)
            where T : McmaObject
        {
            if (string.IsNullOrWhiteSpace(notificationEndpoint?.HttpEndpoint))
                return;

            // create a notification from the provided resource
            var notification = new Notification {Source = resourceId, Content = resource.ToMcmaJson()};

            // get the resource endpoint for the notification url
            var resourceEndpoint = await GetResourceEndpointAsync(notificationEndpoint.HttpEndpoint);

            // send the notification via the ResourceEndpointClient, if found, or just via regular http otherwise
            if (resourceEndpoint != null)
                await resourceEndpoint.PostAsync(notification, notificationEndpoint.HttpEndpoint, cancellationToken);
            else
                await McmaHttpClient.PostAsync(notificationEndpoint.HttpEndpoint, notification, cancellationToken);
        }
    }
}