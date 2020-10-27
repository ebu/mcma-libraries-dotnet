using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Worker
{
    public static class WorkerServiceCollectionExtensions
    {
        public static IServiceCollection AddMcmaWorker(this IServiceCollection services, Action<WorkerBuilder> buildWorker)
        {
            var builder = new WorkerBuilder(services);
            buildWorker(builder);
            
            return services.AddSingleton<IWorker, Worker>();
        }
    }
}