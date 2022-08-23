using System;
using Mcma.Api.Http;
using Mcma.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Api.Routing.Defaults;

public static class DefaultRouteServiceCollectionExtensions
{
    public static McmaApiBuilder AddDefaultRoutes<TResource>(this McmaApiBuilder apiBuilder,
                                                             string root = null,
                                                             Action<DefaultRouteCollectionBuilder<TResource>> configureRoutes = null)
        where TResource : McmaResource
    {
        if (root != null)
            apiBuilder.Services.Configure<DefaultRouteCollectionOptions<TResource>>(opts => opts.Root = root);
            
        var defaultRouteCollectionBuilder = new DefaultRouteCollectionBuilder<TResource>(apiBuilder);
        configureRoutes?.Invoke(defaultRouteCollectionBuilder);
        defaultRouteCollectionBuilder.AddDefaults();

        return apiBuilder;
    }
}