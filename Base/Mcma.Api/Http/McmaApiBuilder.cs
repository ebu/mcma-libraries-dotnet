using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Api.Http;

public class McmaApiBuilder
{
    internal McmaApiBuilder(IServiceCollection services)
    {
        Services = services;
        Services.AddSingleton<IMcmaApiController, McmaApiController>();
    }
        
    public IServiceCollection Services { get; }

    public McmaApiBuilder Configure(Action<McmaApiOptions> configureOptions)
    {
        Services.Configure(configureOptions);
        return this;
    }

    public McmaApiBuilder AddRoute<T>() where T : class, IMcmaApiRoute
    {
        Services.AddSingleton<IMcmaApiRoute, T>();
        return this;
    }

    public McmaApiBuilder AddRoute(IMcmaApiRoute route)
    {
        Services.AddSingleton(route);
        return this;
    }

    public McmaApiBuilder AddRoute(Func<IServiceProvider, IMcmaApiRoute> createRoute)
    {
        Services.AddSingleton(createRoute);
        return this;
    }

    public McmaApiBuilder AddRoute(HttpMethod httpMethod, string path, Func<McmaApiRequestContext, Task> handler)
    {
        Services.AddSingleton<IMcmaApiRoute>(new DelegateMcmaApiRoute(httpMethod, path, handler));
        return this;
    }

    public McmaApiBuilder AddRouteCollection<T>() where T : class, IMcmaApiRouteCollection
    {
        Services.AddSingleton<IMcmaApiRouteCollection, T>();
        return this;
    }

    public McmaApiBuilder AddRouteCollection(IMcmaApiRouteCollection routeCollection)
    {
        Services.AddSingleton(routeCollection);
        return this;
    }

    public McmaApiBuilder AddRouteCollection(Func<IServiceProvider, IMcmaApiRouteCollection> createRouteCollection)
    {
        Services.AddSingleton(createRouteCollection);
        return this;
    }
}