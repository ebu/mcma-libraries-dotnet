﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using System.Net.Http;

namespace Mcma.Api.Routes
{
    public class McmaApiRouteCollection : IMcmaApiRouteCollection
    {
        public McmaApiRouteCollection(params IMcmaApiRoute[] routes)
            : this((IEnumerable<IMcmaApiRoute>)routes)
        {
        }

        public McmaApiRouteCollection(IEnumerable<IMcmaApiRoute> routes)
        {
            Routes = routes?.ToList() ?? new List<IMcmaApiRoute>();
        }

        private List<IMcmaApiRoute> Routes { get; }

        public McmaApiRouteCollection AddRoute(HttpMethod method, string path, Func<McmaApiRequestContext, Task> handler)
        {
            Routes.Add(new DelegateMcmaApiRoute(method, path, handler));
            return this;
        }

        public McmaApiRouteCollection AddRoute(IMcmaApiRoute route)
        {
            Routes.Add(route);
            return this;
        }

        public McmaApiRouteCollection AddRoutes(IMcmaApiRouteCollection routes)
        {
            foreach (var route in routes)
                AddRoute(route);
            return this;
        }

        public IEnumerator<IMcmaApiRoute> GetEnumerator() => Routes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
