namespace Mcma.Client.Resources;

public interface IServiceClient
{
    string Name { get; }
    
    bool HasResourceEndpointClient<T>();

    IResourceEndpointClient? GetResourceEndpointClient<T>();
}