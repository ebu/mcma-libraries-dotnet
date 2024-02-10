using System.Net;
using System.Text;
using Mcma.Api.Routing;
using Mcma.Logging;
using Mcma.Serialization;
using Microsoft.Net.Http.Headers;

namespace Mcma.Api.Http;

public class McmaApiController : IMcmaApiController
{
    private const string JsonContentType = "application/json";

    public McmaApiController(IEnumerable<IMcmaApiRouteCollection> routeCollections, IEnumerable<IMcmaApiRoute> routes)
    {
        Routes =
            new McmaApiRouteCollection(
                (routeCollections ?? Array.Empty<IMcmaApiRouteCollection>())
                .SelectMany(rc => rc)
                .Concat(routes ?? Array.Empty<IMcmaApiRoute>()));
    }

    public McmaApiRouteCollection Routes { get; }

    private static IDictionary<string, string> GetDefaultResponseHeaders()
        => new Dictionary<string, string>
        {
            ["Date"] = DateTimeOffset.UtcNow.ToString("R"),
            ["Content-Type"] = "application/json",
            ["Access-Control-Allow-Origin"] = "*"
        };

    public async Task HandleRequestAsync(McmaApiRequestContext requestContext)
    {
        var logger = requestContext.Logger ?? Logger.System;

        var request = requestContext.Request;
        var response = requestContext.Response;
            
        response.Headers = GetDefaultResponseHeaders();

        var pathMatched = false;
        var methodMatched = false;

        try
        {
            var contentType = requestContext.GetRequestHeaderValue(HeaderNames.ContentType);
            if (requestContext.MethodSupportsRequestBody &&
                contentType != null &&
                contentType.StartsWith(JsonContentType, StringComparison.OrdinalIgnoreCase) &&
                !requestContext.HasJsonRequestBody)
            {
                requestContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                requestContext.Response.JsonBody =
                    new McmaApiError(requestContext.Response.StatusCode,
                                     requestContext.Request.JsonBodyException?.ToString() ?? "Expected json in request body",
                                     requestContext.Request.Path)
                        .ToMcmaJson();
                return;
            }

            var methodsAllowed = string.Empty;

            foreach (var route in Routes)
            {
                if (!route.IsMatch(request.Path, out var pathVariables))
                    continue;
                
                pathMatched = true;

                if (methodsAllowed.Length > 0)
                    methodsAllowed += ", ";
                methodsAllowed += request.HttpMethod;

                if (route.HttpMethod != request.HttpMethod)
                    continue;
                
                methodMatched = true;

                request.PathVariables = pathVariables;
                            
                await route.HandleAsync(requestContext);
                break;
            }

            if (!pathMatched)
            {
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Headers = GetDefaultResponseHeaders();
                response.JsonBody = new McmaApiError(response.StatusCode, "Resource not found on path '" + request.Path + "'.", request.Path).ToMcmaJson();
            }
            else if (!methodMatched)
            {
                if (!methodsAllowed.Contains("OPTIONS"))
                {
                    if (methodsAllowed.Length > 0)
                        methodsAllowed += ", ";
                    methodsAllowed += "OPTIONS";
                }

                if (request.HttpMethod == HttpMethod.Options)
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.Headers = GetDefaultResponseHeaders();

                    string? corsMethod = null;
                    string? corsHeaders = null;

                    foreach (var prop in request.Headers.Keys)
                    {
                        switch (prop.ToLower())
                        {
                            case "access-control-request-method":
                                corsMethod = request.Headers[prop];
                                break;
                            case "access-control-request-headers":
                                corsHeaders = request.Headers[prop];
                                break;
                        }
                    }

                    if (corsMethod != null)
                    {
                        response.Headers["Access-Control-Allow-Methods"] = methodsAllowed;

                        if (corsHeaders != null)
                            response.Headers["Access-Control-Allow-Headers"] = corsHeaders;
                    }
                    else
                        response.Headers["Allow"] = methodsAllowed;
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    response.Headers = GetDefaultResponseHeaders();
                    response.Headers["Allow"] = methodsAllowed;
                    response.JsonBody = new McmaApiError(response.StatusCode, "Method '" + request.HttpMethod + "' not allowed on path '" + request.Path, request.Path).ToMcmaJson();
                }
            }
            else if ((response.StatusCode / 200 << 0) * 200 == 400)
            {
                if (response.Body == null && response.JsonBody == null)
                    response.JsonBody = new McmaApiError(response.StatusCode, response.ErrorMessage, request.Path).ToMcmaJson();
            }
            else if (response.StatusCode == 0)
            {
                response.StatusCode = (int)HttpStatusCode.OK;
            }

            if (response.JsonBody != null)
                response.Body = Encoding.UTF8.GetBytes(response.JsonBody.ToString());
        }
        catch (Exception ex)
        {
            logger.Error($"{request.HttpMethod} {request.Path} encountered an exception", ex.Message, ex.StackTrace);
                
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Headers = GetDefaultResponseHeaders();
            response.JsonBody = new McmaApiError(response.StatusCode, ex.ToString(), request.Path).ToMcmaJson();
        }

        if (response.StatusCode >= 400)
            logger.Error($"{request.HttpMethod} {request.Path} finished with error status of {response.StatusCode}", request.ToMcmaJson(), response.ToMcmaJson());

    }
}