using System;
using Mcma.Api;
using Mcma.Api.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Api.Google.CloudFunctions
{
    public static class HttpFunctionServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaCloudFunctionApi(this IServiceCollection services, Action<McmaApiBuilder> buildApi)
            => services.AddMcmaApi(buildApi)
                       .AddSingleton<IHttpFunctionApiController, HttpFunctionApiController>();
    }
}