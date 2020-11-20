﻿using System.Threading.Tasks;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public interface IDefaultDeleteRouteStartedHandler<TResource> where TResource : McmaResource
    {
        Task<bool> OnStartedAsync(McmaApiRequestContext requestContext);
    }
}