namespace Mcma.Client.Resources
{
    public interface IServiceClient
    {
        bool HasResourceEndpointClient<T>();

        IResourceEndpointClient GetResourceEndpointClient<T>();
    }
}