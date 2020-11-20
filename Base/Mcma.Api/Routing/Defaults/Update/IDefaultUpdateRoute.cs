﻿using System;
using System.Threading.Tasks;
using Mcma.Api.Routes;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public interface IDefaultUpdateRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
    {
    }
}