using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Mcma.Api.Aws.ApiGateway;
using Mcma.Functions.Aws;
using Mcma.Logging;

namespace Mcma.Functions.Aws.ApiHandler
{  
    public class McmaLambdaApiHandler : IMcmaLambdaFunctionHandler<APIGatewayProxyRequest, APIGatewayProxyResponse>
    {
        public McmaLambdaApiHandler(ILoggerProvider loggerProvider, IApiGatewayApiController apiGatewayApiController)
        {
            LoggerProvider = loggerProvider;
            ApiController = apiGatewayApiController;
        }
        
        private ILoggerProvider LoggerProvider { get; }

        private IApiGatewayApiController ApiController { get; }

        public async Task<APIGatewayHttpApiV2ProxyResponse> ExecuteAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            var logger = LoggerProvider.Get(context.AwsRequestId);
            try
            {
                logger.FunctionStart(context.AwsRequestId);
                logger.Debug(request);
                logger.Debug(context);
                
                return await ApiController.HandleRequestAsync(request, context);
            }
            finally
            {
                logger.FunctionEnd(context.AwsRequestId);
                await LoggerProvider.FlushAsync();
            }
        }

        public async Task<APIGatewayProxyResponse> ExecuteAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var logger = LoggerProvider.Get(context.AwsRequestId);
            try
            {
                logger.FunctionStart(context.AwsRequestId);
                logger.Debug(request);
                logger.Debug(context);
                
                return await ApiController.HandleRequestAsync(request, context);
            }
            finally
            {
                logger.FunctionEnd(context.AwsRequestId);
                await LoggerProvider.FlushAsync();
            }
        }
    }
}