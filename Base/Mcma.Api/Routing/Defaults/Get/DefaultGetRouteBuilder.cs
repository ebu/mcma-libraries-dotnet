using Mcma.Api.Http;
using Mcma.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mcma.Api.Routing.Defaults.Get;

public class DefaultGetRouteBuilder<TResource> where TResource : McmaResource
{
    internal DefaultGetRouteBuilder(IServiceCollection services)
    {
        Services = services;
    }
            
    private IServiceCollection Services { get; }
        
    internal bool Removed { get; private set; }

    public void HandleWith<TOverride>() where TOverride : class, IDefaultGetRoute<TResource>
        => Services.AddSingleton<IDefaultGetRoute<TResource>, TOverride>();

    public DefaultGetRouteBuilder<TResource> AddRouteStartedHandler<TRouteStartedHandler>()
        where TRouteStartedHandler : class, IDefaultGetRouteStartedHandler<TResource>
    {
        Services.AddSingleton<IDefaultGetRouteStartedHandler<TResource>, TRouteStartedHandler>();
        return this;
    }

    public DefaultGetRouteBuilder<TResource> AddRouteCompletedHandler<TRouteCompletedHandler>()
        where TRouteCompletedHandler : class, IDefaultGetRouteCompletedHandler<TResource>
    {
        Services.AddSingleton<IDefaultGetRouteCompletedHandler<TResource>, TRouteCompletedHandler>();
        return this;
    }

    public void Remove() => Removed = true;

    internal void AddDefaults()
    {
        Services.TryAddSingleton<IDefaultGetRouteStartedHandler<TResource>, NoOpDefaultGetRouteStartedHandler>();
        Services.TryAddSingleton<IDefaultGetRouteCompletedHandler<TResource>, NoOpDefaultGetRouteCompletedHandler>();
        Services.TryAddSingleton<IDefaultGetRoute<TResource>, DefaultGetRoute<TResource>>();
    }

    internal class NoOpDefaultGetRouteStartedHandler : IDefaultGetRouteStartedHandler<TResource>
    {
        public Task<bool> OnStartedAsync(McmaApiRequestContext requestContext) => Task.FromResult(true);
    }

    internal class NoOpDefaultGetRouteCompletedHandler : IDefaultGetRouteCompletedHandler<TResource>
    {
        public Task OnCompletedAsync(McmaApiRequestContext requestContext, TResource resource) => Task.CompletedTask;
    }
}