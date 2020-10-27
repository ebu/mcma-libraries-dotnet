using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Worker
{
    public static class ProcessJobAssigmentOperationServiceCollectionExtensions
    {
        public static IServiceCollection AddProcessJobAssignmentOperation<TJob>(this IServiceCollection services,
                                                                                Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles)
            where TJob : Job
        {
            services.AddSingleton<IWorkerOperation, ProcessJobAssignmentOperation<TJob>>();
            
            var builder = new ProcessJobAssignmentOperationBuilder<TJob>(services);
            addProfiles(builder);

            return services;
        }
    }
}