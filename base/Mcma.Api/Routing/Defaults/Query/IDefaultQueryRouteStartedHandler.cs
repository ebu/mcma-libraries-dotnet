using System.Threading.Tasks;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public interface IDefaultQueryRouteStartedHandler<TResource> where TResource : McmaResource
    {
        Task<bool> OnStartedAsync(McmaApiRequestContext requestContext);
    }
}