using System;
using Mcma.Worker.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Worker;

public static class WorkerServiceCollectionExtensions
{
    public static IServiceCollection AddMcmaWorker(this IServiceCollection services, Action<McmaWorkerBuilder> buildWorker)
    {
        var builder = new McmaWorkerBuilder(services);
        buildWorker(builder);
            
        return services.AddSingleton<IMcmaWorker, McmaWorker>();
    }
}