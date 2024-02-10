using Mcma.Api.Http;
using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Delete;

public interface IDefaultDeleteRouteCompletedHandler<in TResource> where TResource : McmaResource
{
    Task OnCompletedAsync(McmaApiRequestContext requestContext, TResource createdResource);
}