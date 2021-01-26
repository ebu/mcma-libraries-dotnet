using Mcma.Api.Routes;
using Mcma.Logging;

namespace Mcma.Azure.FunctionsApi
{
    public static class RouteCollectionExtensions
    {
        public static AzureFunctionApiController ToAzureFunctionApiController(this McmaApiRouteCollection routeCollection, ILoggerProvider loggerProvider = null)
            => new AzureFunctionApiController(routeCollection, loggerProvider);
    }
}
