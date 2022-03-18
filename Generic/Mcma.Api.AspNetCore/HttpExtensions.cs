using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api;
using Mcma.Api.Http;
using Mcma.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Mcma.Api.AspNetCore
{
    public static class HttpExtensions
    {
        public static IDictionary<string, string> ToDictionary(this IEnumerable<KeyValuePair<string, StringValues>> stringValuesCollection)
            => stringValuesCollection?.ToDictionary<KeyValuePair<string, StringValues>, string, string>(x => x.Key, x => x.Value);

        public static async Task<McmaApiRequest> GetMcmaApiRequestAsync(this HttpContext httpContext, string basePath)
            => new()
            {
                Id = httpContext.TraceIdentifier,
                Path = httpContext.Request.Path.ToString().Replace(basePath, string.Empty),
                HttpMethod = new HttpMethod(httpContext.Request.Method),
                Headers = httpContext.Request.Headers?.ToDictionary(),
                PathVariables = new Dictionary<string, object>(),
                QueryStringParameters = httpContext.Request.Query?.ToDictionary() ?? new Dictionary<string, string>(),
                Body = await httpContext.Request.Body.ReadAllBytesAsync()
            };

        public static async Task CopyToHttpResponseAsync(this McmaApiResponse mcmaResponse, HttpResponse httpResponse)
        {
            httpResponse.StatusCode = mcmaResponse.StatusCode;

            if (mcmaResponse.Headers != null)
                foreach (var (key, value) in mcmaResponse.Headers)
                    httpResponse.Headers[key] = value;

            if (mcmaResponse.Body != null)
            {
                httpResponse.ContentLength = mcmaResponse.Body.Length;
                await httpResponse.Body.WriteAsync(mcmaResponse.Body, 0, mcmaResponse.Body.Length);
            }
        }
    }
}