using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultUpdateRouteBuilder<TResource> where TResource : McmaResource
    {
        internal DefaultUpdateRouteBuilder(IServiceCollection services)
        {
            Services = services;
        }
            
        private IServiceCollection Services { get; }
        
        internal bool Removed { get; private set; }

        public void HandleWith<TOverride>() where TOverride : class, IDefaultUpdateRoute<TResource>
            => Services.AddSingleton<IDefaultUpdateRoute<TResource>, TOverride>();

        public DefaultUpdateRouteBuilder<TResource> AddRouteStartedHandler<TRouteStartedHandler>()
            where TRouteStartedHandler : class, IDefaultUpdateRouteStartedHandler<TResource>
        {
            Services.AddSingleton<IDefaultUpdateRouteStartedHandler<TResource>, TRouteStartedHandler>();
            return this;
        }

        public DefaultUpdateRouteBuilder<TResource> AddRouteCompletedHandler<TRouteCompletedHandler>()
            where TRouteCompletedHandler : class, IDefaultUpdateRouteCompletedHandler<TResource>
        {
            Services.AddSingleton<IDefaultUpdateRouteCompletedHandler<TResource>, TRouteCompletedHandler>();
            return this;
        }

        public void Remove() => Removed = true;

        internal void AddDefaults()
        {
            Services.TryAddSingleton<IDefaultUpdateRouteStartedHandler<TResource>, NoOpDefaultUpdateRouteStartedHandler>();
            Services.TryAddSingleton<IDefaultUpdateRouteCompletedHandler<TResource>, NoOpDefaultUpdateRouteCompletedHandler>();
            Services.TryAddSingleton<IDefaultUpdateRoute<TResource>, DefaultUpdateRoute<TResource>>();
        }

        internal class NoOpDefaultUpdateRouteStartedHandler : IDefaultUpdateRouteStartedHandler<TResource>
        {
            public Task<bool> OnStartedAsync(McmaApiRequestContext requestContext) => Task.FromResult(true);
        }

        internal class NoOpDefaultUpdateRouteCompletedHandler : IDefaultUpdateRouteCompletedHandler<TResource>
        {
            public Task OnCompletedAsync(McmaApiRequestContext requestContext, TResource resource) => Task.CompletedTask;
        }
    }
}