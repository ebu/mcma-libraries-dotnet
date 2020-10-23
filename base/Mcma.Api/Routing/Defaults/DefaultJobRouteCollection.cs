using System;
using System.Threading.Tasks;
using Mcma.Data;
using Mcma.WorkerInvoker;

namespace Mcma.Api.Routing.Defaults
{
    public class DefaultJobRouteCollection : DefaultRouteCollection<JobAssignment>
    {
        public DefaultJobRouteCollection(IDocumentDatabaseTableProvider dbTableProvider, IWorkerInvoker workerInvoker, string root = null)
            : base(dbTableProvider, root)
        {
            WorkerInvoker = workerInvoker ?? throw new ArgumentNullException(nameof(workerInvoker));

            Create.OnStartedAsync = SetJobTracker;
            Create.OnCompletedAsync = RunJob;
        }
        
        private IWorkerInvoker WorkerInvoker { get; }

        private static Task<bool> SetJobTracker(McmaApiRequestContext requestContext)
        {
            var jobAssignment = requestContext.GetRequestBody<JobAssignment>();
            if (jobAssignment.Tracker == null)
                jobAssignment.Tracker = new McmaTracker { Id = Guid.NewGuid().ToString(), Label = jobAssignment.Type };

            return Task.FromResult(true);
        }

        private async Task RunJob(McmaApiRequestContext requestContext, JobAssignment jobAssignment)
        {
            await WorkerInvoker
                .InvokeAsync(
                    requestContext.EnvironmentVariables.WorkerFunctionId(),
                    "ProcessJobAssignment",
                    new
                    {
                        jobAssignmentDatabaseId = jobAssignment.Id.Replace(requestContext.EnvironmentVariables.PublicUrl(), string.Empty)
                    },
                    jobAssignment.Tracker
                );
        }
    }
}
