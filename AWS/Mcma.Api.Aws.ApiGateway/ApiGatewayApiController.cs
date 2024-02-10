using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Mcma.Api.Http;
using Mcma.Logging;

namespace Mcma.Api.Aws.ApiGateway;

public class ApiGatewayApiController : IApiGatewayApiController
{
    public ApiGatewayApiController(ILoggerProvider loggerProvider, IMcmaApiController controller)
    {
        LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        Controller = controller ?? throw new ArgumentNullException(nameof(controller));
    }

    private ILoggerProvider LoggerProvider { get; }

    private IMcmaApiController Controller { get; }

    public async Task<APIGatewayHttpApiV2ProxyResponse> HandleRequestAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        var requestContext =
            new McmaApiRequestContext(LoggerProvider,
                                      new McmaApiRequest(
                                          context.AwsRequestId,
                                          request.RequestContext.Http.Path[(request.RequestContext.Stage.Length + 1)..],
                                          new HttpMethod(request.RequestContext.Http.Method),
                                          request.Headers,
                                          request.QueryStringParameters ?? new Dictionary<string, string>(),
                                          !string.IsNullOrWhiteSpace(request.Body) ? Encoding.UTF8.GetBytes(request.Body) : null));
            
        await Controller.HandleRequestAsync(requestContext);

        var responseBodyString =
            requestContext.Response.JsonBody?.ToString() ??
            (requestContext.Response.Body != null ? Convert.ToBase64String(requestContext.Response.Body) : null);

        var isBase64Encoded = responseBodyString != null && requestContext.Response.JsonBody == null;

        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = requestContext.Response.StatusCode,
            Headers = requestContext.Response.Headers,
            Body = responseBodyString,
            IsBase64Encoded = isBase64Encoded
        };
    }

    public async Task<APIGatewayProxyResponse> HandleRequestAsync(APIGatewayProxyRequest request, ILambdaContext context)
    {
        var requestContext =
            new McmaApiRequestContext(LoggerProvider,
                                      new McmaApiRequest(
                                          context.AwsRequestId,
                                          request.Path,
                                          new HttpMethod(request.HttpMethod),
                                          request.Headers,
                                          request.QueryStringParameters ?? new Dictionary<string, string>(),
                                          !string.IsNullOrWhiteSpace(request.Body) ? Encoding.UTF8.GetBytes(request.Body) : null));
            
        await Controller.HandleRequestAsync(requestContext);

        var responseBodyString =
            requestContext.Response.JsonBody?.ToString() ??
            (requestContext.Response.Body != null ? Convert.ToBase64String(requestContext.Response.Body) : null);

        var isBase64Encoded = responseBodyString != null && requestContext.Response.JsonBody == null;

        return new APIGatewayProxyResponse
        {
            StatusCode = requestContext.Response.StatusCode,
            Headers = requestContext.Response.Headers,
            Body = responseBodyString,
            IsBase64Encoded = isBase64Encoded
        };
    }
}