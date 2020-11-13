using System;
using Mcma.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.GoogleCloud.HttpFunctionsApi
{
    public static class HttpFunctionServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaHttpFunctionApi(this IServiceCollection services, Action<McmaApiBuilder> buildApi)
            => services.AddMcmaApi(buildApi)
                       .AddSingleton<IHttpFunctionApiController, HttpFunctionApiController>();
    }
}