﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultQueryRouteBuilder<TResource> where TResource : McmaResource
    {
        internal DefaultQueryRouteBuilder(IServiceCollection services)
        {
            Services = services;
        }
            
        private IServiceCollection Services { get; }
        
        internal bool Removed { get; private set; }

        public void HandleWith<TOverride>() where TOverride : class, IDefaultQueryRoute<TResource>
            => Services.AddSingleton<IDefaultQueryRoute<TResource>, TOverride>();

        public DefaultQueryRouteBuilder<TResource> AddQueryExecutor<TQueryExecutor>() where TQueryExecutor : class, IDefaultRouteQueryExecutor<TResource>
        {
            Services.AddSingleton<IDefaultRouteQueryExecutor<TResource>, TQueryExecutor>();
            return this;
        }

        public DefaultQueryRouteBuilder<TResource> AddRouteStartedHandler<TRouteStartedHandler>()
            where TRouteStartedHandler : class, IDefaultQueryRouteStartedHandler<TResource>
        {
            Services.AddSingleton<IDefaultQueryRouteStartedHandler<TResource>, TRouteStartedHandler>();
            return this;
        }

        public DefaultQueryRouteBuilder<TResource> AddRouteCompletedHandler<TRouteCompletedHandler>()
            where TRouteCompletedHandler : class, IDefaultQueryRouteCompletedHandler<TResource>
        {
            Services.AddSingleton<IDefaultQueryRouteCompletedHandler<TResource>, TRouteCompletedHandler>();
            return this;
        }

        public void Remove() => Removed = true;

        internal void AddDefaults()
        {
            Services.TryAddSingleton<IDefaultQueryRouteStartedHandler<TResource>, NoOpDefaultQueryRouteStartedHandler>();
            Services.TryAddSingleton<IDefaultQueryRouteCompletedHandler<TResource>, NoOpDefaultQueryRouteCompletedHandler>();
            Services.TryAddSingleton<IDefaultRouteQueryExecutor<TResource>, DefaultRouteQueryExecutor<TResource>>();
            Services.TryAddSingleton<IDefaultQueryRoute<TResource>, DefaultQueryRoute<TResource>>();
        }

        internal class NoOpDefaultQueryRouteStartedHandler : IDefaultQueryRouteStartedHandler<TResource>
        {
            public Task<bool> OnStartedAsync(McmaApiRequestContext requestContext) => Task.FromResult(true);
        }

        internal class NoOpDefaultQueryRouteCompletedHandler : IDefaultQueryRouteCompletedHandler<TResource>
        {
            public Task OnCompletedAsync(McmaApiRequestContext requestContext, QueryResults<TResource> queryResults) => Task.CompletedTask;
        }
    }
}