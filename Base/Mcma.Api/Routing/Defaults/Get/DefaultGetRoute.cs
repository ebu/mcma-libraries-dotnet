using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routes;
using Mcma.Data;
using Microsoft.Extensions.Options;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultGetRoute<TResource> : McmaApiRoute, IDefaultGetRoute<TResource> where TResource : McmaResource
    {
        public DefaultGetRoute(IDocumentDatabaseTable dbTable,
                               IDefaultGetRouteStartedHandler<TResource> startedHandler,
                               IDefaultGetRouteCompletedHandler<TResource> completedHandler,
                               IOptions<DefaultRouteCollectionOptions<TResource>> options)
            : base(HttpMethod.Get, (options.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root + "/{id}")
        {
            DbTable = dbTable ?? throw new ArgumentNullException(nameof(dbTable));
            StartedHandler = startedHandler;
            CompletedHandler = completedHandler;
        }
        
        private IDocumentDatabaseTable DbTable { get; }

        private IDefaultGetRouteStartedHandler<TResource> StartedHandler { get; }

        private IDefaultGetRouteCompletedHandler<TResource> CompletedHandler { get; }

        public override async Task HandleAsync(McmaApiRequestContext requestContext)
        {
            if (StartedHandler != null && !await StartedHandler.OnStartedAsync(requestContext))
                return;

            var resource = await DbTable.GetAsync<TResource>(requestContext.Request.Path);

            if (CompletedHandler != null)
                await CompletedHandler.OnCompletedAsync(requestContext, resource);

            if (resource != null)
                requestContext.SetResponseBody(resource);
            else
                requestContext.SetResponseResourceNotFound();
        }
    }
}