﻿using System;
using System.Threading.Tasks;
using Mcma.Api.Routes;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public interface IDefaultCreateRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
    {
    }
}