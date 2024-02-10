using Mcma.Api.Http;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Update;

// ReSharper disable once UnusedTypeParameter - used to map resource types to routes 
public interface IDefaultUpdateRouteStartedHandler<TResource> where TResource : McmaResource
{
    Task<bool> OnStartedAsync(McmaApiRequestContext requestContext);
}