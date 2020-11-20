﻿using Mcma.Api.Routes;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public interface IDefaultQueryRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
    {
    }
}