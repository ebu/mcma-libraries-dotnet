using Microsoft.AspNetCore.Builder;

namespace Mcma.Api.AspNetCore;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseMcmaApiHandler(this IApplicationBuilder appBuilder)
        => appBuilder.UseMiddleware<McmaApiHandlerMiddleware>();
}