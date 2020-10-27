using System.Threading.Tasks;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public interface IDefaultQueryRouteCompletedHandler<TResource> where TResource : McmaResource
    {
        Task OnCompletedAsync(McmaApiRequestContext requestContext, QueryResults<TResource> queryResults);
    }
}