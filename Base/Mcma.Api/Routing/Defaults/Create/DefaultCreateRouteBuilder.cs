using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Api.Routing.Defaults.Create
{
    public class DefaultCreateRouteBuilder<TResource> where TResource : McmaResource
    {
        internal DefaultCreateRouteBuilder(IServiceCollection services)
        {
            Services = services;
        }
            
        private IServiceCollection Services { get; }
        
        internal bool Removed { get; private set; }

        public void HandleWith<TOverride>() where TOverride : class, IDefaultCreateRoute<TResource>
            => Services.AddSingleton<IDefaultCreateRoute<TResource>, TOverride>();

        public DefaultCreateRouteBuilder<TResource> AddRouteStartedHandler<TRouteStartedHandler>()
            where TRouteStartedHandler : class, IDefaultCreateRouteStartedHandler<TResource>
        {
            Services.AddSingleton<IDefaultCreateRouteStartedHandler<TResource>, TRouteStartedHandler>();
            return this;
        }

        public DefaultCreateRouteBuilder<TResource> AddRouteCompletedHandler<TRouteCompletedHandler>()
            where TRouteCompletedHandler : class, IDefaultCreateRouteCompletedHandler<TResource>
        {
            Services.AddSingleton<IDefaultCreateRouteCompletedHandler<TResource>, TRouteCompletedHandler>();
            return this;
        }

        public void Remove() => Removed = true;

        internal void AddDefaults()
        {
            Services.TryAddSingleton<IDefaultCreateRouteStartedHandler<TResource>, NoOpDefaultCreateRouteStartedHandler>();
            Services.TryAddSingleton<IDefaultCreateRouteCompletedHandler<TResource>, NoOpDefaultCreateRouteCompletedHandler>();
            Services.TryAddSingleton<IDefaultCreateRoute<TResource>, DefaultCreateRoute<TResource>>();
        }

        internal class NoOpDefaultCreateRouteStartedHandler : IDefaultCreateRouteStartedHandler<TResource>
        {
            public Task<bool> OnStartedAsync(McmaApiRequestContext requestContext) => Task.FromResult(true);
        }

        internal class NoOpDefaultCreateRouteCompletedHandler : IDefaultCreateRouteCompletedHandler<TResource>
        {
            public Task OnCompletedAsync(McmaApiRequestContext requestContext, TResource resource) => Task.CompletedTask;
        }
    }
}