using System;
using System.Collections.Generic;
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
        => new()
        {
            Id = Guid.NewGuid().ToString(),
            HttpMethod = new HttpMethod(httpRequest.Method),
            Path = httpRequest.Path,
            Headers = httpRequest.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
            PathVariables = new Dictionary<string, object>(),
            QueryStringParameters = httpRequest.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
            Body = await httpRequest.Body.ReadAllBytesAsync()
        };

    public static Task SetHttpResponseAsync(this HttpContext httpContext, McmaApiResponse mcmaApiResponse)
        => httpContext.Response.FromMcmaApiResponseAsync(mcmaApiResponse);

    public static async Task FromMcmaApiResponseAsync(this HttpResponse httpResponse, McmaApiResponse mcmaApiResponse)
    {
        httpResponse.StatusCode = mcmaApiResponse.StatusCode;

        foreach (var header in mcmaApiResponse.Headers)
            httpResponse.Headers[header.Key] = header.Value;

        await httpResponse.Body.WriteAsync(mcmaApiResponse.Body, 0, mcmaApiResponse.Body.Length);
    }
}