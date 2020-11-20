﻿namespace Mcma.Client
{
    public interface IServiceClient
    {
        bool HasResourceEndpointClient<T>();

        IResourceEndpointClient GetResourceEndpointClient<T>();
    }
}