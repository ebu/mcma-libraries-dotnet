using System.Threading.Tasks;
using Mcma.Api;
using Mcma.Api.Routes;
using Mcma.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;

namespace Mcma.Azure.FunctionsApi
{
    public class AzureFunctionApiController
    {
        public AzureFunctionApiController(McmaApiRouteCollection routeCollection, ILoggerProvider loggerProvider = null, IEnvironmentVariables environmentVariables = null)
        {
            McmaApiController = new McmaApiController(routeCollection, loggerProvider);
            LoggerProvider = loggerProvider;
            EnvironmentVariables = environmentVariables ?? Mcma.EnvironmentVariables.Instance;
        }

        private McmaApiController McmaApiController { get; }

        private ILoggerProvider LoggerProvider { get; }
        
        private IEnvironmentVariables EnvironmentVariables { get; }

        public async Task<IActionResult> HandleRequestAsync(HttpRequest request, ExecutionContext executionContext)
        {
            var requestContext = await request.ToMcmaApiRequestContextAsync(executionContext, LoggerProvider, EnvironmentVariables);

            var logger = LoggerProvider?.Get(requestContext.RequestId, requestContext.GetTracker());

            logger?.Debug($"Starting {request.Method} request to {request.Path}...");

            await McmaApiController.HandleRequestAsync(requestContext);

            logger?.Debug($"{request.Method} request to {request.Path} finished with status {requestContext.Response.StatusCode}");

            return requestContext.ToActionResult();
        }
    }
}
