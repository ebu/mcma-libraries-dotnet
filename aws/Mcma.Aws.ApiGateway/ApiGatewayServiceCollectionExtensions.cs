using System;
using Mcma.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Aws.ApiGateway
{
    public static class ApiGatewayServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaApiGatewayApi(this IServiceCollection services, Action<McmaApiBuilder> build)
            => services.AddMcmaApi(build)
                       .AddSingleton<IApiGatewayApiController, ApiGatewayApiController>();
    }
}