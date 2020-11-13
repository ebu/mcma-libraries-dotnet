using System;
using System.Threading.Tasks;
using Mcma.Api;
using Mcma.Logging;
using Microsoft.AspNetCore.Http;

namespace Mcma.GoogleCloud.HttpFunctionsApi
{
    public class HttpFunctionApiController : IHttpFunctionApiController
    {
        public HttpFunctionApiController(IMcmaApiController controller, ILoggerProvider loggerProvider)
        {
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));
            LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        }

        private IMcmaApiController Controller { get; }

        private ILoggerProvider LoggerProvider { get; }

        public async Task HandleRequestAsync(HttpContext httpContext)
        {
            var requestContext = new McmaApiRequestContext(LoggerProvider, await httpContext.GetMcmaApiRequestAsync());

            await Controller.HandleRequestAsync(requestContext);

            await httpContext.SetHttpResponseAsync(requestContext.Response);
        }
    }
}