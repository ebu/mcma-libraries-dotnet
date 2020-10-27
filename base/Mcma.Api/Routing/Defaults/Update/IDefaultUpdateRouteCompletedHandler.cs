using System.Threading.Tasks;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public interface IDefaultUpdateRouteCompletedHandler<in TResource> where TResource : McmaResource
    {
        Task OnCompletedAsync(McmaApiRequestContext requestContext, TResource createdResource);
    }
}