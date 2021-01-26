using Mcma.Api.Routes;
using Mcma.Logging;

namespace Mcma.Aws.ApiGateway
{
    public static class RouteCollectionExtensions
    {
        public static ApiGatewayApiController ToApiGatewayApiController(this McmaApiRouteCollection routes, ILoggerProvider loggerProvider = null)
            => new ApiGatewayApiController(routes, loggerProvider);
    }
}