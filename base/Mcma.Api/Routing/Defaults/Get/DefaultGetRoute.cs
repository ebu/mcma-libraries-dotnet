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
        public DefaultGetRoute(IDocumentDatabaseTableProvider dbTableProvider,
                               IDefaultGetRouteStartedHandler<TResource> startedHandler,
                               IDefaultGetRouteCompletedHandler<TResource> completedHandler,
                               IOptions<DefaultRouteCollectionOptions<TResource>> options,
                               IOptions<McmaApiOptions> apiOptions)
            : base(HttpMethod.Get, (options?.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root + "/{id}")
        {
            DbTableProvider = dbTableProvider ?? throw new ArgumentNullException(nameof(dbTableProvider));
            StartedHandler = startedHandler;
            CompletedHandler = completedHandler;
            ApiOptions = apiOptions.ValidateAndGet();
        }
        
        private IDocumentDatabaseTableProvider DbTableProvider { get; }

        private IDefaultGetRouteStartedHandler<TResource> StartedHandler { get; }

        private IDefaultGetRouteCompletedHandler<TResource> CompletedHandler { get; }
        
        private McmaApiOptions ApiOptions { get; }

        public override async Task HandleAsync(McmaApiRequestContext requestContext)
        {
            if (StartedHandler != null && !await StartedHandler.OnStartedAsync(requestContext))
                return;

            var table = await DbTableProvider.GetAsync(ApiOptions.TableName);

            var resource = await table.GetAsync<TResource>(requestContext.Request.Path);

            if (CompletedHandler != null)
                await CompletedHandler.OnCompletedAsync(requestContext, resource);

            if (resource != null)
                requestContext.SetResponseBody(resource);
            else
                requestContext.SetResponseResourceNotFound();
        }
    }
}