using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api;
using Mcma.Api.Http;
using Mcma.Logging;
using Mcma.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;

namespace Mcma.Api.Azure.FunctionApp
{
    public static class HttpExtensions
    {
        public static async Task<McmaApiRequestContext> ToMcmaApiRequestContextAsync(
            this HttpRequest request,
            ILoggerProvider loggerProvider,
            ExecutionContext executionContext)
            => new McmaApiRequestContext(loggerProvider, await request.ToMcmaApiRequestAsync(executionContext));

        public static async Task<McmaApiRequest> ToMcmaApiRequestAsync(this HttpRequest request, ExecutionContext executionContext)
            => new McmaApiRequest
            {
                Id = executionContext.InvocationId.ToString(),
                Path = request.Path,
                HttpMethod = new HttpMethod(request.Method),
                Headers = request.Headers.Keys.ToDictionary(k => k, k => request.Headers[k].ToString()),
                PathVariables = new Dictionary<string, object>(),
                QueryStringParameters = request.Query.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()),
                Body = await request.Body.ReadAllBytesAsync()
            };

        public static IActionResult ToActionResult(this McmaApiRequestContext requestContext)
            => requestContext.Response.ToActionResult();

        public static IActionResult ToActionResult(this McmaApiResponse response)
            => new McmaApiActionResult(response);

        public static async Task CopyToHttpResponseAsync(this McmaApiResponse mcmaResponse, HttpResponse httpResponse)
        {
            httpResponse.StatusCode = mcmaResponse.StatusCode;

            if (mcmaResponse.Headers != null)
                foreach (var header in mcmaResponse.Headers)
                    httpResponse.Headers[header.Key] = header.Value;

            if (mcmaResponse.Body != null)
                await httpResponse.Body.WriteAsync(mcmaResponse.Body, 0, mcmaResponse.Body.Length);
        }
    }
}
