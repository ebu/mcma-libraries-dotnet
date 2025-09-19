using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Utility;
using Microsoft.AspNetCore.Http;

namespace Mcma.Api.Google.CloudFunctions;

public static class HttpAspNetExtensions
{
    public static Task<McmaApiRequest> GetMcmaApiRequestAsync(this HttpContext httpContext)
        => httpContext.Request.ToMcmaApiRequestAsync();
        
    public static async Task<McmaApiRequest> ToMcmaApiRequestAsync(this HttpRequest httpRequest)
        => new(
            Guid.NewGuid().ToString(),
            httpRequest.Path,
            new HttpMethod(httpRequest.Method),
            httpRequest.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
            httpRequest.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
            await httpRequest.Body.ReadAllBytesAsync());

    public static Task SetHttpResponseAsync(this HttpContext httpContext, McmaApiResponse mcmaApiResponse)
        => httpContext.Response.FromMcmaApiResponseAsync(mcmaApiResponse);

    public static async Task FromMcmaApiResponseAsync(this HttpResponse httpResponse, McmaApiResponse mcmaApiResponse)
    {
        httpResponse.StatusCode = mcmaApiResponse.StatusCode;

        foreach (var header in mcmaApiResponse.Headers)
            httpResponse.Headers[header.Key] = header.Value;

        await httpResponse.Body.WriteAsync(mcmaApiResponse.Body.AsMemory(0, mcmaApiResponse.Body.Length));
    }
}