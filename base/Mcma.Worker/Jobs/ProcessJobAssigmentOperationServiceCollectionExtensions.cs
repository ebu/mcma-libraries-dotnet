using System;

namespace Mcma.Worker
{
    public static class ProcessJobAssigmentOperationServiceCollectionExtensions
    {
        public static WorkerBuilder AddProcessJobAssignmentOperation<TJob>(this WorkerBuilder workerBuilder,
                                                                           Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles)
            where TJob : Job
        {
            workerBuilder.AddOperation<ProcessJobAssignmentOperation<TJob>>();
            
            var builder = new ProcessJobAssignmentOperationBuilder<TJob>(workerBuilder.Services);
            addProfiles(builder);

            return workerBuilder;
        }
    }
}