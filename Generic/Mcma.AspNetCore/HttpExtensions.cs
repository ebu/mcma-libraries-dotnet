using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api;
using Mcma.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Mcma.AspNetCore
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
                foreach (var header in mcmaResponse.Headers)
                    httpResponse.Headers[header.Key] = header.Value;

            if (mcmaResponse.Body != null)
                await httpResponse.Body.WriteAsync(mcmaResponse.Body, 0, mcmaResponse.Body.Length);
        }
    }
}