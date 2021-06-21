using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Query
{
    public interface IDefaultQueryRouteStartedHandler<TResource> where TResource : McmaResource
    {
        Task<bool> OnStartedAsync(McmaApiRequestContext requestContext);
    }
}