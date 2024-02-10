using Mcma.Api.Http;
using Mcma.Api.Routing.Defaults.Create;
using Mcma.Model;

namespace Mcma.Api.Routing.JobAssignments;

public class SetTrackerOnCreationStarted : IDefaultCreateRouteStartedHandler<JobAssignment>
{
    public Task<bool> OnStartedAsync(McmaApiRequestContext requestContext)
    {
        var jobAssignment = requestContext.GetRequestBody<JobAssignment>();
        if (jobAssignment is null)
            return Task.FromResult(true);

        if (jobAssignment.Tracker is not null)
            return Task.FromResult(true);

        var trackerId = Guid.NewGuid().ToString();
        var trackerLabel = jobAssignment.Type ?? trackerId;

        jobAssignment.Tracker = new McmaTracker { Id = trackerId, Label = trackerLabel };

        return Task.FromResult(true);
    }
}