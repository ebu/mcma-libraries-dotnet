using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Update
{
    public interface IDefaultUpdateRouteStartedHandler<TResource> where TResource : McmaResource
    {
        Task<bool> OnStartedAsync(McmaApiRequestContext requestContext);
    }
}