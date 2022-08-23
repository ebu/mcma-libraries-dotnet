using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Data.DocumentDatabase;
using Mcma.Model;
using Microsoft.Extensions.Options;

namespace Mcma.Api.Routing.Defaults.Delete;

public class DefaultDeleteRoute<TResource> : McmaApiRoute, IDefaultDeleteRoute<TResource> where TResource : McmaResource
{
    public DefaultDeleteRoute(IDocumentDatabaseTable dbTable,
                              IDefaultDeleteRouteStartedHandler<TResource> startedHandler,
                              IDefaultDeleteRouteCompletedHandler<TResource> completedHandler,
                              IOptions<DefaultRouteCollectionOptions<TResource>> options)
        : base(HttpMethod.Delete, (options.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root + "/{id}")
    {
        DbTable = dbTable ?? throw new ArgumentNullException(nameof(dbTable));
        StartedHandler = startedHandler;
        CompletedHandler = completedHandler;
    }
        
    private IDocumentDatabaseTable DbTable { get; }

    private IDefaultDeleteRouteStartedHandler<TResource> StartedHandler { get; }

    private IDefaultDeleteRouteCompletedHandler<TResource> CompletedHandler { get; }
        
    public override async Task HandleAsync(McmaApiRequestContext requestContext)
    {
        if (StartedHandler != null && !await StartedHandler.OnStartedAsync(requestContext))
            return;

        // build id from the root public url and the path
        var id = requestContext.Request.Path;

        // get the resource from the db
        var resource = await DbTable.GetAsync<TResource>(id);

        // if the resource doesn't exist, return a 404
        if (resource == null)
        {
            requestContext.SetResponseResourceNotFound();
            return;
        }

        // delete the resource from the db
        await DbTable.DeleteAsync(id);

        // invoke the completion handler, if any
        if (CompletedHandler != null)
            await CompletedHandler.OnCompletedAsync(requestContext, resource);
    }
}