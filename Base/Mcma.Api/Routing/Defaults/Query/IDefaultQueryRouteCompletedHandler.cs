using Mcma.Api.Http;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Query;

public interface IDefaultQueryRouteCompletedHandler<TResource> where TResource : McmaResource
{
    Task OnCompletedAsync(McmaApiRequestContext requestContext, QueryResults<TResource> queryResults);
}