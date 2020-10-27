using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routes;
using Mcma.Data;
using Microsoft.Extensions.Options;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultDeleteRoute<TResource> : McmaApiRoute, IDefaultDeleteRoute<TResource> where TResource : McmaResource
    {
        public DefaultDeleteRoute(IDocumentDatabaseTableProvider dbTableProvider,
                                  IDefaultDeleteRouteStartedHandler<TResource> startedHandler,
                                  IDefaultDeleteRouteCompletedHandler<TResource> completedHandler,
                                  IOptions<DefaultRouteCollectionOptions<TResource>> options,
                                  IOptions<McmaApiOptions> apiOptions)
            : base(HttpMethod.Delete, (options.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root + "/{id}")
        {
            DbTableProvider = dbTableProvider ?? throw new ArgumentNullException(nameof(dbTableProvider));
            StartedHandler = startedHandler;
            CompletedHandler = completedHandler;
            ApiOptions = apiOptions.ValidateAndGet();
        }
        
        private IDocumentDatabaseTableProvider DbTableProvider { get; }

        private IDefaultDeleteRouteStartedHandler<TResource> StartedHandler { get; }

        private IDefaultDeleteRouteCompletedHandler<TResource> CompletedHandler { get; }
        
        private McmaApiOptions ApiOptions { get; }

        public override async Task HandleAsync(McmaApiRequestContext requestContext)
        {
            if (StartedHandler != null && !await StartedHandler.OnStartedAsync(requestContext))
                return;

            var table = await DbTableProvider.GetAsync(ApiOptions.TableName);

            // build id from the root public url and the path
            var id = requestContext.Request.Path;

            // get the resource from the db
            var resource = await table.GetAsync<TResource>(id);

            // if the resource doesn't exist, return a 404
            if (resource == null)
            {
                requestContext.SetResponseResourceNotFound();
                return;
            }

            // delete the resource from the db
            await table.DeleteAsync(id);

            // invoke the completion handler, if any
            if (CompletedHandler != null)
                await CompletedHandler.OnCompletedAsync(requestContext, resource);
        }
    }
}