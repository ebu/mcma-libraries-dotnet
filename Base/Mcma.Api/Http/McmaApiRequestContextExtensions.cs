using System.Net;
using Mcma.Model;

namespace Mcma.Api.Http;

public static class McmaApiRequestContextExtensions
{
    public static void SetResponseBadRequestDueToMissingBody(this McmaApiRequestContext requestContext)
        => requestContext.SetResponseError(HttpStatusCode.BadRequest, "Missing request body.");

    public static void SetResponseResourceCreated(this McmaApiRequestContext requestContext, McmaResource resource)
    {
        requestContext.Response.Headers["Location"] = resource.Id;
        requestContext.Response.StatusCode = (int)HttpStatusCode.Created;
        requestContext.SetResponseBody(resource);
    }

    public static void SetResponseResourceNotFound(this McmaApiRequestContext requestContext)
        => requestContext.SetResponseError(HttpStatusCode.NotFound, "No resource found on path '" + requestContext.Request.Path + "'.");
}