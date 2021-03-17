using Mcma.Api.Routing.Defaults.Routes;

namespace Mcma.Api.Routing.Defaults
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
