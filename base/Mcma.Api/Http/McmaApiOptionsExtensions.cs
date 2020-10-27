using Microsoft.Extensions.Options;

namespace Mcma.Api
{
    public static class McmaApiOptionsExtensions
    {
        public static McmaApiOptions ValidateAndGet(this IOptions<McmaApiOptions> apiOptions)
        {
            if (apiOptions == null)
                throw new McmaException("API options not configured.");
            if (string.IsNullOrWhiteSpace(apiOptions.Value?.TableName))
                throw new McmaException("TableName not configured for API.");
            if (string.IsNullOrWhiteSpace(apiOptions.Value?.PublicUrl))
                throw new McmaException("PublicUrl not configured for API.");
            
            return apiOptions.Value;
        }
        
        public static string CurrentRequestPublicUrl(this McmaApiOptions options, McmaApiRequestContext requestContext)
            => options.PublicUrlForPath(requestContext.Request.Path);

        public static string PublicUrlForPath(this McmaApiOptions options, string path)
            => options.PublicUrl.TrimEnd('/') + "/" + (path?.TrimStart('/') ?? string.Empty);
    }
}