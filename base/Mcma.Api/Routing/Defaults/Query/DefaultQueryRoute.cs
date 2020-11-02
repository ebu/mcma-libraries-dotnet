using System.Net.Http;
using System.Threading.Tasks;
using Mcma.Api.Routes;
using Mcma.Data;
using Microsoft.Extensions.Options;

namespace Mcma.Api.Routing.Defaults.Routes
{
    internal class DefaultQueryRoute<TResource> : McmaApiRoute, IDefaultQueryRoute<TResource> where TResource : McmaResource
    {
        public DefaultQueryRoute(
            IDocumentDatabaseTable dbTable,
            IDefaultQueryRouteStartedHandler<TResource> startedHandler,
            IDefaultRouteQueryExecutor<TResource> queryExecutor,
            IDefaultQueryRouteCompletedHandler<TResource> completedHandler,
            IOptions<DefaultRouteCollectionOptions<TResource>> options)
            : base(HttpMethod.Get, (options.Value ?? new DefaultRouteCollectionOptions<TResource>()).Root)
        {
            StartedHandler = startedHandler;
            QueryExecutor = queryExecutor;
            CompletedHandler = completedHandler;
            DbTable = dbTable;
        }
        
        public HttpMethod Method => HttpMethod.Get;

        private IDefaultQueryRouteStartedHandler<TResource> StartedHandler { get; }

        private IDefaultRouteQueryExecutor<TResource> QueryExecutor { get; }

        private IDefaultQueryRouteCompletedHandler<TResource> CompletedHandler { get; }

        private IDocumentDatabaseTable DbTable { get; }

        public override async Task HandleAsync(McmaApiRequestContext requestContext)
        {
            if (StartedHandler != null && !await StartedHandler.OnStartedAsync(requestContext))
                return;

            var results = await QueryExecutor.ExecuteQueryAsync(requestContext, DbTable);

            if (CompletedHandler != null)
                await CompletedHandler.OnCompletedAsync(requestContext, results);

            requestContext.SetResponseBody(results);
        }
    }
}