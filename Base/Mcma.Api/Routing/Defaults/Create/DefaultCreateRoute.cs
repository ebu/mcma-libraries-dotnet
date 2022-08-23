using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Http;
using Mcma.Data.DocumentDatabase;
using Mcma.Model;
using Microsoft.Extensions.Options;

namespace Mcma.Api.Routing.Defaults.Create;

public class DefaultCreateRoute<TResource> : McmaApiRoute, IDefaultCreateRoute<TResource> where TResource : McmaResource
{
    public DefaultCreateRoute(IDocumentDatabaseTable dbTable,
                              IDefaultCreateRouteCompletedHandler<TResource> completedHandler,
                              IDefaultCreateRouteStartedHandler<TResource> startedHandler,
                              IOptions<DefaultRouteCollectionOptions<TResource>> options,
                              IOptions<McmaApiOptions> apiOptions)
        : base(HttpMethod.Post, (options.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root)
    {
        DbTable = dbTable ?? throw new ArgumentNullException(nameof(dbTable));
        CompletedHandler = completedHandler;
        StartedHandler = startedHandler;

        Root = (options.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root;
        ApiOptions = apiOptions.Value ?? new McmaApiOptions();
    }
        
    private IDocumentDatabaseTable DbTable { get; }

    public IDefaultCreateRouteStartedHandler<TResource> StartedHandler { get; }

    public IDefaultCreateRouteCompletedHandler<TResource> CompletedHandler { get; }

    private string Root { get; }

    private McmaApiOptions ApiOptions { get; }

    public override async Task HandleAsync(McmaApiRequestContext requestContext)
    {
        if (StartedHandler != null && !await StartedHandler.OnStartedAsync(requestContext))
            return;

        var resource = requestContext.GetRequestBody<TResource>();
        if (resource == null)
        {
            requestContext.SetResponseBadRequestDueToMissingBody();
            return;
        }

        var resourcePath = $"{Root}/{Guid.NewGuid()}";

        resource.OnCreate(ApiOptions.PublicUrlForPath(resourcePath));

        await DbTable.PutAsync(resourcePath, resource);

        if (CompletedHandler != null)
            await CompletedHandler.OnCompletedAsync(requestContext, resource);

        requestContext.SetResponseResourceCreated(resource);
    }
}