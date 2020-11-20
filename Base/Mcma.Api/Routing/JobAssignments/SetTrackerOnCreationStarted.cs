﻿using System;
using System.Threading.Tasks;
using Mcma.Api.Routing.Defaults.Routes;

namespace Mcma.Api.Routing.Defaults
{
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
}