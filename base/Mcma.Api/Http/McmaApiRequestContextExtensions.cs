using System.Net;

namespace Mcma.Api
{
    public static class McmaApiRequestContextExtensions
    {
        public static string CurrentRequestPublicUrl(this McmaApiRequestContext requestContext)
            => requestContext.PublicUrlForPath(requestContext.Request.Path);

        public static string PublicUrlForPath(this McmaApiRequestContext requestContext, string path)
            => requestContext.EnvironmentVariables.PublicUrl().TrimEnd('/') + "/" + (path?.TrimStart('/') ?? string.Empty);

        public static void SetResponseBadRequestDueToMissingBody(this McmaApiRequestContext requestContext)
            => requestContext.SetResponseStatusCode(HttpStatusCode.BadRequest, "Missing request body.");

        public static void SetResponseResourceCreated(this McmaApiRequestContext requestContext, McmaResource resource)
        {
            requestContext.SetResponseStatusCode(HttpStatusCode.Created);
            requestContext.SetResponseHeader("Location", resource.Id);
            requestContext.SetResponseBody(resource);
        }

        public static void SetResponseResourceNotFound(this McmaApiRequestContext requestContext)
            => requestContext.SetResponseStatusCode(HttpStatusCode.NotFound, "No resource found on path '" + requestContext.Request.Path + "'.");
    }
}