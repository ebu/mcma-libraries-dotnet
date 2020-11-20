﻿using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace Mcma.Aws.ApiGateway
{
    public interface IApiGatewayApiController
    {
        Task<APIGatewayHttpApiV2ProxyResponse> HandleRequestAsync(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context);

        Task<APIGatewayProxyResponse> HandleRequestAsync(APIGatewayProxyRequest request, ILambdaContext context);
    }
}