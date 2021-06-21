using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Worker.Hangfire
{
    public static class HangfireWorkerServiceCollectionExtensions
    {
        public static IServiceCollection AddHangfireWorker(this IServiceCollection services,
                                                           Action<McmaWorkerBuilder> buildWorker,
                                                           Action<IGlobalConfiguration> configureHangfire)
            => services.AddHangfire(configureHangfire)
                       .AddHangfireServer()
                       .AddMcmaWorker(buildWorker);
    }
}
