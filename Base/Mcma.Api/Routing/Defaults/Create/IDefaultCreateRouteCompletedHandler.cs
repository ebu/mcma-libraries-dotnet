using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Create
{
    public interface IDefaultCreateRouteCompletedHandler<in TResource> where TResource : McmaResource
    {
        Task OnCompletedAsync(McmaApiRequestContext requestContext, TResource createdResource);
    }
}