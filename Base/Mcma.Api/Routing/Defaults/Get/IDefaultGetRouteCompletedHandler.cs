using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Get
{
    public interface IDefaultGetRouteCompletedHandler<in TResource> where TResource : McmaResource
    {
        Task OnCompletedAsync(McmaApiRequestContext requestContext, TResource createdResource);
    }
}