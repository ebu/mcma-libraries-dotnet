using Mcma.Api.Http;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Delete;

// ReSharper disable once UnusedTypeParameter - used to map resource types to routes 
public interface IDefaultDeleteRouteStartedHandler<TResource> where TResource : McmaResource
{
    Task<bool> OnStartedAsync(McmaApiRequestContext requestContext);
}