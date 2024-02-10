using Mcma.Api.Http;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Update;

public interface IDefaultUpdateRouteCompletedHandler<in TResource> where TResource : McmaResource
{
    Task OnCompletedAsync(McmaApiRequestContext requestContext, TResource createdResource);
}