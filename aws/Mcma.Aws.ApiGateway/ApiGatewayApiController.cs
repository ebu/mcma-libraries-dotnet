using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Mcma.Api;
using Mcma.Api.Routes;
using Mcma.Logging;

namespace Mcma.Aws.ApiGateway
{
    public class ApiGatewayApiController
    {
        public ApiGatewayApiController(McmaApiRouteCollection routes, ILoggerProvider loggerProvider = null)
        {
            McmaApiController = new McmaApiController(routes);
            LoggerProvider = loggerProvider;
        }

        private McmaApiController McmaApiController { get; }

        private ILoggerProvider LoggerProvider { get; }

        public async Task<APIGatewayHttpApiV2ProxyResponse> HandleRequestAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            var requestContext = new McmaApiRequestContext(
                new McmaApiRequest
                {
                    Id = context.AwsRequestId,
                    Path = request.RequestContext.Http.Path,
                    HttpMethod = new HttpMethod(request.RequestContext.Http.Method),
                    Headers = request.Headers,
                    PathVariables = new Dictionary<string, object>(),
                    QueryStringParameters = request.QueryStringParameters ?? new Dictionary<string, string>(),
                    Body = request.Body
                },
                request.StageVariables,
                LoggerProvider
            );
            
            await McmaApiController.HandleRequestAsync(requestContext);

            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = requestContext.Response.StatusCode,
                Headers = requestContext.Response.Headers,
                Body = requestContext.Response.Body
            };
        }

        public async Task<APIGatewayProxyResponse> HandleRequestAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var requestContext = new McmaApiRequestContext(
                new McmaApiRequest
                {
                    Id = context.AwsRequestId,
                    Path = request.Path,
                    HttpMethod = new HttpMethod(request.HttpMethod),
                    Headers = request.Headers,
                    PathVariables = new Dictionary<string, object>(),
                    QueryStringParameters = request.QueryStringParameters ?? new Dictionary<string, string>(),
                    Body = request.Body
                },
                request.StageVariables,
                LoggerProvider
            );
            
            await McmaApiController.HandleRequestAsync(requestContext);

            return new APIGatewayProxyResponse
            {
                StatusCode = requestContext.Response.StatusCode,
                Headers = requestContext.Response.Headers,
                Body = requestContext.Response.Body
            };
        }
    }
}