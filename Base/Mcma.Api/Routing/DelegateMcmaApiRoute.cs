﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mcma.Api.Routes
{
    public class DelegateMcmaApiRoute : McmaApiRoute
    {
        public DelegateMcmaApiRoute(HttpMethod httpMethod, string path, Func<McmaApiRequestContext, Task> handler)
            : base(httpMethod, path)
        {
            Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        private Func<McmaApiRequestContext, Task> Handler { get; }

        public override Task HandleAsync(McmaApiRequestContext requestContext) => Handler(requestContext);
    }
}