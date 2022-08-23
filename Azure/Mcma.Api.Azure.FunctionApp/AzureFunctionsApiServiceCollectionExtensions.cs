using System;
using Mcma.Api.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Api.Azure.FunctionApp;

public static class AzureFunctionsApiServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaAzureFunctionApi(this IServiceCollection services, Action<McmaApiBuilder> build)
        => services.AddMcmaApi(build)
                   .AddSingleton<IAzureFunctionApiController, AzureFunctionApiController>();
}