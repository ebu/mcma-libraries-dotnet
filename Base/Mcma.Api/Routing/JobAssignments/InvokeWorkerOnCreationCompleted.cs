using System;
using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Api.Routing.Defaults.Create;
using Mcma.Model;
using Mcma.WorkerInvoker;
using Microsoft.Extensions.Options;

namespace Mcma.Api.Routing.JobAssignments;

public class InvokeWorkerOnCreationCompleted : IDefaultCreateRouteCompletedHandler<JobAssignment>
{
    public InvokeWorkerOnCreationCompleted(IMcmaWorkerInvoker workerInvoker, IOptions<McmaApiOptions> options)
    {
        WorkerInvoker = workerInvoker ?? throw new ArgumentNullException(nameof(workerInvoker));
        Options = options.Value ?? new McmaApiOptions();
    }
        
    private IMcmaWorkerInvoker WorkerInvoker { get; }
        
    private McmaApiOptions Options { get; }

    public Task OnCompletedAsync(McmaApiRequestContext requestContext, JobAssignment createdResource)
        => WorkerInvoker
            .InvokeAsync(
                "ProcessJobAssignment",
                new
                {
                    jobAssignmentDatabaseId = createdResource.Id.Replace(Options.PublicUrl ?? string.Empty, string.Empty)
                },
                createdResource.Tracker
            );
}