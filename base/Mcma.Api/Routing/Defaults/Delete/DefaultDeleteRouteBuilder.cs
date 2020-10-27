using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultDeleteRouteBuilder<TResource> where TResource : McmaResource
    {
        internal DefaultDeleteRouteBuilder(IServiceCollection services)
        {
            Services = services;
        }
            
        private IServiceCollection Services { get; }
        
        internal bool Removed { get; private set; }

        public void HandleWith<TOverride>() where TOverride : class, IDefaultDeleteRoute<TResource>
            => Services.AddSingleton<IDefaultDeleteRoute<TResource>, TOverride>();

        public DefaultDeleteRouteBuilder<TResource> AddRouteStartedHandler<TRouteStartedHandler>()
            where TRouteStartedHandler : class, IDefaultDeleteRouteStartedHandler<TResource>
        {
            Services.AddSingleton<IDefaultDeleteRouteStartedHandler<TResource>, TRouteStartedHandler>();
            return this;
        }

        public DefaultDeleteRouteBuilder<TResource> AddRouteCompletedHandler<TRouteCompletedHandler>()
            where TRouteCompletedHandler : class, IDefaultDeleteRouteCompletedHandler<TResource>
        {
            Services.AddSingleton<IDefaultDeleteRouteCompletedHandler<TResource>, TRouteCompletedHandler>();
            return this;
        }

        public void Remove() => Removed = true;

        internal void AddDefaults()
        {
            Services.TryAddSingleton<IDefaultDeleteRouteStartedHandler<TResource>, NoOpDefaultDeleteRouteStartedHandler>();
            Services.TryAddSingleton<IDefaultDeleteRouteCompletedHandler<TResource>, NoOpDefaultDeleteRouteCompletedHandler>();
            Services.TryAddSingleton<IDefaultDeleteRoute<TResource>, DefaultDeleteRoute<TResource>>();
        }

        internal class NoOpDefaultDeleteRouteStartedHandler : IDefaultDeleteRouteStartedHandler<TResource>
        {
            public Task<bool> OnStartedAsync(McmaApiRequestContext requestContext) => Task.FromResult(true);
        }

        internal class NoOpDefaultDeleteRouteCompletedHandler : IDefaultDeleteRouteCompletedHandler<TResource>
        {
            public Task OnCompletedAsync(McmaApiRequestContext requestContext, TResource resource) => Task.CompletedTask;
        }
    }
}