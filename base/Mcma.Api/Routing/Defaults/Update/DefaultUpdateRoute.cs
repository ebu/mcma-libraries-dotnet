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
        public DefaultUpdateRoute(IDocumentDatabaseTableProvider dbTableProvider,
                                  IDefaultUpdateRouteStartedHandler<TResource> startedHandler,
                                  IDefaultUpdateRouteCompletedHandler<TResource> completedHandler,
                                  IOptions<DefaultRouteCollectionOptions<TResource>> defaultRouteOptions,
                                  IOptions<McmaApiOptions> apiOptions)
            : base(HttpMethod.Put, (defaultRouteOptions?.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root + "/{id}")
        {
            DbTableProvider = dbTableProvider ?? throw new ArgumentNullException(nameof(dbTableProvider));
            StartedHandler = startedHandler ?? throw new ArgumentNullException(nameof(startedHandler));
            CompletedHandler = completedHandler ?? throw new ArgumentNullException(nameof(completedHandler));
            ApiOptions = apiOptions.ValidateAndGet();
        }
        
        private IDocumentDatabaseTableProvider DbTableProvider { get; }

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

            resource.OnUpsert(ApiOptions.CurrentRequestPublicUrl(requestContext));

            var table = await DbTableProvider.GetAsync(ApiOptions.TableName);
            await table.PutAsync(requestContext.Request.Path, resource);

            await CompletedHandler.OnCompletedAsync(requestContext, resource);

            requestContext.SetResponseBody(resource);
        }
    }
}