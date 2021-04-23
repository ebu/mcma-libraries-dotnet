using System;
using Hangfire;
using Mcma.Worker;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.HangfireWorker
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
