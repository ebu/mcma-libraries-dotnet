using System;
using Mcma.Api;
using Mcma.Api.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Api.Aws.ApiGateway
{
    public static class ApiGatewayServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaApiGatewayApi(this IServiceCollection services, Action<McmaApiBuilder> build)
            => services.AddMcmaApi(build)
                       .AddSingleton<IApiGatewayApiController, ApiGatewayApiController>();
    }
}