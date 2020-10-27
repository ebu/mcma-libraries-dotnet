using System;
using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routes;
using Mcma.Data;
using Microsoft.Extensions.Options;

namespace Mcma.Api.Routing.Defaults.Routes
{
    public class DefaultCreateRoute<TResource> : McmaApiRoute, IDefaultCreateRoute<TResource> where TResource : McmaResource
    {
        public DefaultCreateRoute(IDocumentDatabaseTableProvider dbTableProvider,
                                  IDefaultCreateRouteCompletedHandler<TResource> completedHandler,
                                  IDefaultCreateRouteStartedHandler<TResource> startedHandler,
                                  IOptions<DefaultRouteCollectionOptions<TResource>> options,
                                  IOptions<McmaApiOptions> apiOptions)
            : base(HttpMethod.Post, (options.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root)
        {
            DbTableProvider = dbTableProvider ?? throw new ArgumentNullException(nameof(dbTableProvider));
            CompletedHandler = completedHandler;
            StartedHandler = startedHandler;

            Root = (options.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root;
            ApiOptions = apiOptions.ValidateAndGet();
        }
        
        private IDocumentDatabaseTableProvider DbTableProvider { get; }

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

            var table = await DbTableProvider.GetAsync(ApiOptions.TableName);
            await table.PutAsync(resourcePath, resource);

            if (CompletedHandler != null)
                await CompletedHandler.OnCompletedAsync(requestContext, resource);

            requestContext.SetResponseResourceCreated(resource);
        }
    }
}