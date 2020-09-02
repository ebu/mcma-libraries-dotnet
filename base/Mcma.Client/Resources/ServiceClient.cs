using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Mcma;

namespace Mcma.Client
{
    public class ServiceClient
    {
        internal ServiceClient(HttpClient httpClient, Service service, IAuthProvider authProvider = null)
        {
            Data = service;

            ResourcesByType =
                service.Resources != null
                    ? service.Resources.ToDictionary(r => r.ResourceType, r => new ResourceEndpointClient(httpClient, authProvider, r, service.AuthType, service.AuthContext))
                    : new Dictionary<string, ResourceEndpointClient>();
        }

        public Service Data { get; }

        private IDictionary<string, ResourceEndpointClient> ResourcesByType { get; }

        public IEnumerable<ResourceEndpointClient> Resources => ResourcesByType.Values;

        public bool HasResourceEndpointClient(string resourceType)
            => ResourcesByType.ContainsKey(resourceType);

        public bool HasResourceEndpointClient(Type resourceType)
            => HasResourceEndpointClient(resourceType.Name);

        public bool HasResourceEndpointClient<T>()
            => HasResourceEndpointClient(typeof(T));

        public ResourceEndpointClient GetResourceEndpointClient(string resourceType)
            => ResourcesByType.ContainsKey(resourceType) ? ResourcesByType[resourceType] : null;
            
        public ResourceEndpointClient GetResourceEndpointClient(Type resourceType)
            => GetResourceEndpointClient(resourceType.Name);
            
        public ResourceEndpointClient GetResourceEndpointClient<T>()
            => GetResourceEndpointClient(typeof(T));

        public ResourceEndpointClient[] GetAllResourceEndpointClients()
            => ResourcesByType.Values.ToArray();
    }
}