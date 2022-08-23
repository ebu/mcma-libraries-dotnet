using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Data.DocumentDatabase;
using Mcma.Model;
using Mcma.WorkerInvoker;

namespace Mcma.Api.Routing.JobAssignments;

internal class JobAssignmentCancelRoute : McmaApiRoute
{
    public JobAssignmentCancelRoute(IDocumentDatabaseTable dbTable, IMcmaWorkerInvoker workerInvoker)
        : base(HttpMethod.Post, "/job-assignments/{id}/cancel")
    {
        DbTable = dbTable ?? throw new ArgumentNullException(nameof(dbTable));
        WorkerInvoker = workerInvoker ?? throw new ArgumentNullException(nameof(workerInvoker));
    }

    private IDocumentDatabaseTable DbTable { get; }

    private IMcmaWorkerInvoker WorkerInvoker { get; }

    public override async Task HandleAsync(McmaApiRequestContext requestContext)
    {
        var jobAssignmentDatabaseId = $"/job-assignments/" + requestContext.Request.PathVariables["id"];
        var jobAssignment = await DbTable.GetAsync<JobAssignment>(jobAssignmentDatabaseId);
        if (jobAssignment == null)
        {
            requestContext.SetResponseResourceNotFound();
            return;
        }

        if (jobAssignment.Status == JobStatus.Completed ||
            jobAssignment.Status == JobStatus.Failed ||
            jobAssignment.Status == JobStatus.Canceled)
        {
            requestContext.SetResponseError(HttpStatusCode.Conflict, "JobAssignment is already in a final state.");
            return;
        }

        await WorkerInvoker.InvokeAsync("ProcessCancel", new {jobAssignmentDatabaseId}, jobAssignment.Tracker);
    }
}