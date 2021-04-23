using System;
using System.Threading.Tasks;
using Mcma.Api;
using Mcma.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Mcma.AspNetCore
{
    public class McmaApiHandlerMiddleware
    {
        /// <summary>
        /// Instantiates a <see cref="McmaApiHandlerMiddleware"/>
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerProvider"></param>
        /// <param name="controller"></param>
        /// <param name="options"></param>
        public McmaApiHandlerMiddleware(RequestDelegate next, ILoggerProvider loggerProvider, IMcmaApiController controller, IOptions<McmaApiHandlerMiddlewareOptions> options)
        {
            Next = next ?? throw new ArgumentNullException(nameof(next));
            LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));
            Options = options.Value ?? new McmaApiHandlerMiddlewareOptions();
            BasePath = $"/{Options.BasePath.Trim('/')}";
        }

        private RequestDelegate Next { get; }

        private ILoggerProvider LoggerProvider { get; }

        private IMcmaApiController Controller { get; }
        
        private McmaApiHandlerMiddlewareOptions Options { get; }
        
        private string BasePath { get; }

        /// <summary>
        /// Handles the request by passing it on to an <see cref="IMcmaApiController"/>
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(BasePath))
            {
                var requestContext = new McmaApiRequestContext(LoggerProvider, await httpContext.GetMcmaApiRequestAsync(BasePath));

                await Controller.HandleRequestAsync(requestContext);

                await requestContext.Response.CopyToHttpResponseAsync(httpContext.Response);
            }
            else
                await Next(httpContext);
        }
    }
}