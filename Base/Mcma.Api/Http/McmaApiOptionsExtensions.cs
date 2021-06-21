namespace Mcma.Api.Http
{
    public static class McmaApiOptionsExtensions
    {
        public static string PublicUrlForCurrentRequest(this McmaApiOptions options, McmaApiRequestContext requestContext)
            => options.PublicUrlForPath(requestContext.Request.Path);

        public static string PublicUrlForPath(this McmaApiOptions options, string path)
            => options.PublicUrl.TrimEnd('/') + "/" + (path?.TrimStart('/') ?? string.Empty);
    }
}