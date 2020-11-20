using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routes;
using Mcma.Data;
using Microsoft.Extensions.Options;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultUpdateRoute<TResource> : McmaApiRoute, IDefaultUpdateRoute<TResource> where TResource : McmaResource
    {
        public DefaultUpdateRoute(IDocumentDatabaseTable dbTable,
                                  IDefaultUpdateRouteStartedHandler<TResource> startedHandler,
                                  IDefaultUpdateRouteCompletedHandler<TResource> completedHandler,
                                  IOptions<DefaultRouteCollectionOptions<TResource>> defaultRouteOptions,
                                  IOptions<McmaApiOptions> apiOptions)
            : base(HttpMethod.Put, (defaultRouteOptions?.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root + "/{id}")
        {
            DbTable = dbTable ?? throw new ArgumentNullException(nameof(dbTable));
            StartedHandler = startedHandler ?? throw new ArgumentNullException(nameof(startedHandler));
            CompletedHandler = completedHandler ?? throw new ArgumentNullException(nameof(completedHandler));
            ApiOptions = apiOptions.Value ?? new McmaApiOptions();
        }
        
        private IDocumentDatabaseTable DbTable { get; }

        private IDefaultUpdateRouteStartedHandler<TResource> StartedHandler { get; }

        private IDefaultUpdateRouteCompletedHandler<TResource> CompletedHandler { get; }
        
        private McmaApiOptions ApiOptions { get; }

        public override async Task HandleAsync(McmaApiRequestContext requestContext)
        {
            if (!await StartedHandler.OnStartedAsync(requestContext))
                return;

            var resource = requestContext.GetRequestBody<TResource>();
            if (resource == null)
            {
                requestContext.SetResponseBadRequestDueToMissingBody();
                return;
            }

            resource.OnUpsert(ApiOptions.PublicUrlForCurrentRequest(requestContext));

            await DbTable.PutAsync(requestContext.Request.Path, resource);

            await CompletedHandler.OnCompletedAsync(requestContext, resource);

            requestContext.SetResponseBody(resource);
        }
    }
}