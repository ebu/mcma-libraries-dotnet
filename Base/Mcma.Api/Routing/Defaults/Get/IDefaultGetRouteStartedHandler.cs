using Mcma.Api.Http;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Get;

// ReSharper disable once UnusedTypeParameter - used to map resource types to routes 
public interface IDefaultGetRouteStartedHandler<TResource> where TResource : McmaResource
{
    Task<bool> OnStartedAsync(McmaApiRequestContext requestContext);
}