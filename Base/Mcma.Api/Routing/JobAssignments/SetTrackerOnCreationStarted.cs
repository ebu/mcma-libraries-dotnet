using System;
using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Api.Routing.Defaults.Create;
using Mcma.Model;

namespace Mcma.Api.Routing.JobAssignments;

public class SetTrackerOnCreationStarted : IDefaultCreateRouteStartedHandler<JobAssignment>
{
    public Task<bool> OnStartedAsync(McmaApiRequestContext requestContext)
    {
        var jobAssignment = requestContext.GetRequestBody<JobAssignment>();
        if (jobAssignment.Tracker == null)
            jobAssignment.Tracker = new McmaTracker {Id = Guid.NewGuid().ToString(), Label = jobAssignment.Type};

        return Task.FromResult(true);
    }
}