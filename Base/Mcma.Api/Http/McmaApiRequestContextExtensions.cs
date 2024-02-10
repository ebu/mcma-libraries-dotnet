using System.Net;
using Mcma.Model;

namespace Mcma.Api.Http;

public static class McmaApiRequestContextExtensions
{
    public static void SetResponseBadRequestDueToMissingBody(this McmaApiRequestContext requestContext)
        => requestContext.SetResponseError(HttpStatusCode.BadRequest, "Missing request body.");

    public static void SetResponseResourceCreated(this McmaApiRequestContext requestContext, McmaResource resource)
    {
        if (string.IsNullOrWhiteSpace(resource.Id))
            throw new McmaException("Cannot set response to 201 Created for resource because it does not have an ID", context: resource);

        requestContext.Response.Headers["Location"] = resource.Id;
        requestContext.Response.StatusCode = (int)HttpStatusCode.Created;
        requestContext.SetResponseBody(resource);
    }

    public static void SetResponseResourceNotFound(this McmaApiRequestContext requestContext)
        => requestContext.SetResponseError(HttpStatusCode.NotFound, "No resource found on path '" + requestContext.Request.Path + "'.");
}