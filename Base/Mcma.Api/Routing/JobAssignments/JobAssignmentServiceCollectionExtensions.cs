using Mcma.Api.Http;
using Mcma.Api.Routing.Defaults;
using Mcma.Model;

namespace Mcma.Api.Routing.JobAssignments
{
    public static class JobAssignmentServiceCollectionExtensions
    {
        public static McmaApiBuilder AddDefaultJobAssignmentRoutes(this McmaApiBuilder apiBuilder)
            => apiBuilder
                .AddDefaultRoutes<JobAssignment>(
                    "job-assignments",
                    routeBuilder =>
                    {
                        routeBuilder.Create
                                    .AddRouteStartedHandler<SetTrackerOnCreationStarted>()
                                    .AddRouteCompletedHandler<InvokeWorkerOnCreationCompleted>();
                        
                        routeBuilder.Update.Remove();
                    })
                .AddRoute<JobAssignmentCancelRoute>();
    }
}
