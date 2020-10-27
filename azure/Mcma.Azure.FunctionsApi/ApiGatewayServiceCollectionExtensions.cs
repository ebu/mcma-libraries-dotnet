using System;
using Mcma.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Azure.FunctionsApi
{
    public static class ApiGatewayServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureFunctionApi(this IServiceCollection services, Action<McmaApiBuilder> build)
            => services.AddMcmaApi(build)
                       .AddSingleton<IAzureFunctionApiController, AzureFunctionApiController>();
    }
}