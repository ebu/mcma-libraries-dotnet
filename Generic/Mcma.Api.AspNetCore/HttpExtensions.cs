using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Mcma.Api.AspNetCore;

public static class HttpExtensions
{
    public static IDictionary<string, string> ToDictionary(this IEnumerable<KeyValuePair<string, StringValues>> stringValuesCollection)
        => stringValuesCollection?.ToDictionary<KeyValuePair<string, StringValues>, string, string>(x => x.Key, x => x.Value);

    public static async Task<McmaApiRequest> GetMcmaApiRequestAsync(this HttpContext httpContext, string basePath)
        => new(
            httpContext.TraceIdentifier,
            httpContext.Request.Path.ToString().Replace(basePath, string.Empty),
            new HttpMethod(httpContext.Request.Method),
            httpContext.Request.Headers?.ToDictionary(),
            httpContext.Request.Query?.ToDictionary() ?? new Dictionary<string, string>(),
            await httpContext.Request.Body.ReadAllBytesAsync());

    public static async Task CopyToHttpResponseAsync(this McmaApiResponse mcmaResponse, HttpResponse httpResponse)
    {
        httpResponse.StatusCode = mcmaResponse.StatusCode;

        if (mcmaResponse.Headers != null)
            foreach (var (key, value) in mcmaResponse.Headers)
                httpResponse.Headers[key] = value;

        if (mcmaResponse.Body != null)
        {
            httpResponse.ContentLength = mcmaResponse.Body.Length;
            await httpResponse.Body.WriteAsync(mcmaResponse.Body.AsMemory(0, mcmaResponse.Body.Length));
        }
    }
}