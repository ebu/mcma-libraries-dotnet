﻿using System.Threading.Tasks;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public interface IDefaultDeleteRouteCompletedHandler<in TResource> where TResource : McmaResource
    {
        Task OnCompletedAsync(McmaApiRequestContext requestContext, TResource createdResource);
    }
}