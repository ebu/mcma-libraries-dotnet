﻿using Mcma.Api.Routes;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public interface IDefaultGetRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
    {
    }
}