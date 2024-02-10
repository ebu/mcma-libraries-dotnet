using Mcma.Api.Http;
using Mcma.Api.Routing.Defaults.Create;
using Mcma.Api.Routing.Defaults.Delete;
using Mcma.Api.Routing.Defaults.Get;
using Mcma.Api.Routing.Defaults.Query;
using Mcma.Api.Routing.Defaults.Update;
using Mcma.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Mcma.Api.Routing.Defaults;

public class DefaultRouteCollectionBuilder<TResource> where TResource : McmaResource
{
    internal DefaultRouteCollectionBuilder(McmaApiBuilder apiBuilder)
    {
        Query = new DefaultQueryRouteBuilder<TResource>(apiBuilder.Services);
        Create = new DefaultCreateRouteBuilder<TResource>(apiBuilder.Services);
        Get = new DefaultGetRouteBuilder<TResource>(apiBuilder.Services);
        Update = new DefaultUpdateRouteBuilder<TResource>(apiBuilder.Services);
        Delete = new DefaultDeleteRouteBuilder<TResource>(apiBuilder.Services);

        apiBuilder.AddRouteCollection(Build);
    }

    public DefaultQueryRouteBuilder<TResource> Query { get; }

    public DefaultCreateRouteBuilder<TResource> Create { get; }

    public DefaultGetRouteBuilder<TResource> Get { get; }

    public DefaultUpdateRouteBuilder<TResource> Update { get; }

    public DefaultDeleteRouteBuilder<TResource> Delete { get; }

    internal void AddDefaults()
    {
        Query.AddDefaults();
        Create.AddDefaults();
        Get.AddDefaults();
        Update.AddDefaults();
        Delete.AddDefaults();
    }

    internal IMcmaApiRouteCollection Build(IServiceProvider serviceProvider)
    {
        var routes = new List<IMcmaApiRoute>();
            
        if (!Query.Removed)
            routes.Add(serviceProvider.GetRequiredService<IDefaultQueryRoute<TResource>>());
            
        if (!Create.Removed)
            routes.Add(serviceProvider.GetRequiredService<IDefaultCreateRoute<TResource>>());
            
        if (!Get.Removed)
            routes.Add(serviceProvider.GetRequiredService<IDefaultGetRoute<TResource>>());
            
        if (!Update.Removed)
            routes.Add(serviceProvider.GetRequiredService<IDefaultUpdateRoute<TResource>>());
            
        if (!Delete.Removed)
            routes.Add(serviceProvider.GetRequiredService<IDefaultDeleteRoute<TResource>>());
            
        return new McmaApiRouteCollection(routes);
    }
}