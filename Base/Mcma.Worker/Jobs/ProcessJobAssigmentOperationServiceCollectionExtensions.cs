using System;
using Mcma.Model.Jobs;

namespace Mcma.Worker.Jobs;

public static class ProcessJobAssigmentOperationServiceCollectionExtensions
{
    public static McmaWorkerBuilder AddProcessJobAssignmentOperation<TJob>(this McmaWorkerBuilder mcmaWorkerBuilder,
                                                                           Action<ProcessJobAssignmentOperationBuilder<TJob>> addProfiles)
        where TJob : Job
    {
        mcmaWorkerBuilder.AddOperation<ProcessJobAssignmentOperation<TJob>>();
            
        var builder = new ProcessJobAssignmentOperationBuilder<TJob>(mcmaWorkerBuilder.Services);
        addProfiles(builder);

        return mcmaWorkerBuilder;
    }
}