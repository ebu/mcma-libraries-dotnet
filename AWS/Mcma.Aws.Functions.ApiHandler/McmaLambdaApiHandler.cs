﻿using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Mcma.Aws.ApiGateway;
using Mcma.Logging;

namespace Mcma.Aws.Functions.ApiHandler
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